# -*- coding: utf-8 -*-


import kolada.api as kolada
import unittest


class KoladaAPITest(unittest.TestCase):

    def test_caching(self):
        default_caching = kolada.USE_CACHE 
        kolada.use_cache(True)
        cache = kolada.urlcache()
        self.assertTrue(isinstance(cache, kolada.SqliteCache), 'When caching is on, we should have an sqlite cache implementation')
        kolada.use_cache(False)
        cache = kolada.urlcache()
        self.assertTrue(isinstance(cache, kolada.NoCache), 'When caching is off, we should have a non-caching "cache" implementation')
        kolada.use_cache(default_caching)
        
    def test_entity_list(self):
        for Entity in (kolada.KPI, kolada.Municipality, kolada.OU):
            ## grab all entities into a local dictionary
            ents = dict((ent.id, ent) for ent in Entity.list())
            ## filter result for 'skola'
            filter_for = u'skola'
            for ent in Entity.list(filter_for):
                ## ok, so this is a stupid test ;-)
                ## test that we don't
                ## receive some completly different if we have a
                ## filter criteria
                self.assertTrue(ent.id in ents)
                ## also assert that we get correct results
                self.assertTrue(filter_for in ent.title.lower())

    def test_chunker(self):
        r1 = range(100)
        chunks = list(kolada.chunker(r1, 12))
        self.assertEqual(9, len(chunks))
        self.assertEqual(12, len(chunks[0]))
        self.assertEqual(4, len(chunks[-1]))

    def test_data_get(self):
        ## get all kpis which have 'skola' as a part of the title
        school_kpi = list(kolada.KPI.list('skola'))
        kpi_lookup = dict((kpi.id, kpi) for kpi in school_kpi)
        ## get a municipality (Malmö)
        mlm = list(kolada.Municipality.list(u'Malmö'))
        ## get all values for 2011 and 2012
        data = list(kolada.Value.get(school_kpi, mlm, range(2011,2013)))
        for rec in data:
            kpi = kpi_lookup[rec.kpi]
            #print kpi, rec.period, rec.value

    def test_simple_data_get(self):
        ## All municipalities
        municipalities = list(kolada.Municipality.list())
        ## Antal invånare, totalt (kommun, landsting)
        kpis = [kolada.KPI('N01951'), kolada.KPI('N61907')]
        ## period 2012
        period = 2012
        ## build a hash of the data-recods... 
        data = {}
        for rec in kolada.Value.get(kpis, municipalities, period):
            data[rec.kpi,rec.municipality, rec.period] = (rec.value, rec.value_f, rec.value_m)
        ## ...and test for Ale (assumed to be known)
        self.assertEqual(27842, data['N01951','1440', '2012'][0])

    def test_simple_oudata_get(self):
        ale = kolada.Municipality('1440')
        ous = list(kolada.OU.list(municipality=ale))
        kpis = list(kolada.KPI.list())
        testdata = {}
        testkpi = None
        testou = None
        for rec in kolada.OUValue.get(kpis, ous, range(2010,2013)):
            testdata[rec.kpi,rec.ou,rec.period] = rec.value
            if testkpi is None:
                testkpi = rec.kpi
                testou = rec.ou
        for rec in kolada.OUValue.perou(kolada.KPI(testkpi), kolada.OU(ale, testou)):
            index = (rec.kpi, rec.ou, rec.period)
            if index in testdata:
                ## TODO: test small, we're comparing floats here
                self.assertEqual(rec.value, testdata[index])
        
if __name__ == '__main__':
    import logging
    #logging.basicConfig(level=logging.DEBUG)
    unittest.main(verbosity=2)
