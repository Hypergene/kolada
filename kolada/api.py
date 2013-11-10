# -*- coding: utf-8 -*-

import json
import urllib
import itertools
import time


## logging setup. 
import logging
logger = logging.getLogger(__name__)
logger.addHandler(logging.NullHandler())


## Caching constants (globals)
USE_CACHE = 1
CACHE_TIME = 24 # in hours
BASE = 'http://api.kolada.se/v1/'


## utility functions
def asutf8(s):
    """
    Encoder-function to utf-8
    """
    if isinstance(s, unicode):
        return s.encode('utf-8')
    elif not isinstance(s, basestring):
        raise ValueError('Non string argument to SimpleURL: %r' % s)
    return s

def isiterable(item):
    """
    Simple iterable predicate. Look for __iter__.
    """
    return hasattr(item, '__iter__')

def chunker(lst, maxsize=100):
    """
    Splits a list into a list of lists with a maximum size. There will
    always be atleast one list in the result.
    """
    ptr = 0
    result = []
    while True:
        result.append(lst[ptr:ptr+maxsize])
        ptr += maxsize
        if ptr >= len(lst):
            break
    return result

class CacheBase(object):
    """
    Simple base for caching implementations
    """
    def loadurl(self, url):
        logger.debug('Loading: %s', url)
        handle = urllib.urlopen(url)
        result = handle.read()
        return self.return_result(url, result)

    def return_result(self, url, result):
        return result

    
class NoCache(CacheBase):
    """
    No caching
    """
    def get(self, url):
        return self.loadurl(url)

    
class SqliteCache(CacheBase):
    """
    Simple sqlite-based caching
    """
    dbname = 'kolada_cache.db'
    def __init__(self):
        import sqlite3 as db
        import os
        import tempfile
        dbfile = os.path.join(tempfile.gettempdir(), self.dbname)
        self.conn = db.connect(dbfile)
        self.cur = self.conn.cursor()
        self.assert_cache_table()
        self.delete_stale()

    def assert_cache_table(self):
        sql = u'''
CREATE TABLE IF NOT EXISTS cache (
    url     TEXT NOT NULL PRIMARY KEY,
    result  TEXT NOT NULL,
    stamp   DATETIME DEFAULT CURRENT_TIMESTAMP
)'''
        self.cur.execute(sql)

    def delete_stale(self):
        sql = u'''
DELETE FROM cache WHERE (strftime('%s', CURRENT_TIMESTAMP) - strftime('%s',stamp))/3600.0 > {0}
'''.format(CACHE_TIME)
        self.cur.execute(sql)
        self.conn.commit()

    def insert(self, url, result):
        sql = u'''
INSERT OR REPLACE INTO cache (url, result, stamp) VALUES(?, ?, CURRENT_TIMESTAMP)
'''

        self.cur.execute(sql, (url, unicode(result,'utf-8')))
        self.conn.commit()

    def select(self, url):
        sql = u'''
SELECT result FROM cache WHERE url = ? AND ((strftime('%s', CURRENT_TIMESTAMP) - strftime('%s',stamp))/3600.0 < {0})
 '''.format(CACHE_TIME)
        self.cur.execute(sql, (url, ))
        result = self.cur.fetchall()
        if result:
            logger.debug('Cache hit: %s', url)
            return result[0][0]
        return None

    def return_result(self, url, result):
        self.insert(url, result)
        return result

    def get(self, url):
        result = self.select(url)
        if result:
            return result
        return self.loadurl(url)

def urlcache(*args, **kwargs):
    """
    Simple forwarder to caching-implementation. 
    """
    if USE_CACHE:
        return SqliteCache(*args, **kwargs)
    return NoCache(*args, **kwargs)

def use_cache(which):
    global USE_CACHE
    USE_CACHE = bool(which)
    
        
## end utility

class SimpleURL(list):
    """
    Simple URL abstraction, supports basically nothing, but is handy for koladas urls.
    """
    def __init__(self, *args):
        super(SimpleURL, self).__init__(args)
        
    def get(self):
        return BASE + '/'.join(urllib.quote(asutf8(item)) for item in self if item)

    def __str__(self):
        return self.get()


class KoladaFetcher(object):
    """
    Abstraction for the url-cursor behaviour of kolada.

    Most instantiations should be with the get_iter constructor, since
    this will directly return an iterator.
    """
    def __init__(self, url):
        self.cache = urlcache()
        self.posts = {}
        if isinstance(url, SimpleURL):
            url = url.get()
        #print "URL: ", url
        self.retrive(url)

    @classmethod
    def get_iter(cls, url):
        return iter(cls(url))

    def retrive(self, url):
        result = self.cache.get(url)
        self.posts = json.loads(result)

    def nexturl(self):
        return self.posts.get('next', None)

    def retrive_next(self):
        nexturl = self.nexturl()
        if not nexturl:
            return False
        ## todo: fetch in worker?
        self.retrive(asutf8(nexturl))
        return True

    def __iter__(self):
        while True:
            for post in self.posts['values']:
                yield post
            if not self.retrive_next():
                break
    
class BaseE(object):
    """
    Entity base-class
    """
    def __init__(self, id):
        self.id = id
        
    @classmethod
    def from_record(cls, rec):
        """
        Construction of from a json-record from kolada.
        """
        item = cls(rec['id'])
        item.__dict__.update(rec)
        return item

    @classmethod
    def list(cls, search=None):
        ename = cls.__dict__.get('entityname', None) ## search only classes direct dict
        if not ename:
            raise ValueError('An entity must have a entityname')
        for rec in  iter(KoladaFetcher(SimpleURL(ename, search))):
            yield cls.from_record(rec)

    def __unicode__(self):
        return u'%s - %s' % (self.id, self.title)

    def __str__(self):
        return asutf8(unicode(self))

class KPI(BaseE):
    entityname = 'kpi'

class Municipality(BaseE):
    entityname = 'municipality'

class OU(BaseE):
    ## todo: remove inheritance? this is different
    ## todo: allow municipality to be fetched from ou id.
    def __init__(self, municipality, id):
        self.municipality = municipality
        self.id = id
        
    @classmethod
    def list(cls, search=None, municipality=None):
        if not municipality:
            ## search all if non are given
            municipalities = list(Municipality.list())
        else:
            municipalities = [municipality]
        for m in municipalities:
            iterator = KoladaFetcher.get_iter(SimpleURL('ou',m.id, search))
            for post in iterator:
                item = cls(m, post['id'])
                item.__dict__.update(post)
                yield item
                
class ValueBase(object):
    def __init__(self, post):
        self.__dict__.update(post)

    @staticmethod
    def pre():
        return u''
    
    @classmethod
    def get(cls, kpi, municipality_or_ou, year):
        """
        Get an iterator to values given kpi, municipality_or_ou and
        year, were all may be sequences.
        """
        kpis = kpi if isiterable(kpi) else [kpi]
        municipalities = municipality_or_ou if isiterable(municipality_or_ou) else [municipality_or_ou]
        years = year if isiterable(year) else [year]
        kpis = chunker([kpi.id for kpi in kpis], 100)
        municipalities = chunker([m.id for m in municipalities], 100)
        years = chunker([unicode(year) for year in years], 100)
        for k, m, y in itertools.product(kpis, municipalities, years):
            url = SimpleURL(cls.pre(), 'data','exact', u','.join(k), u','.join(m), u','.join(y))
            for post in KoladaFetcher.get_iter(url):
                yield cls(post)

    @classmethod
    def peryear(cls, kpi, year):
        """
        Get an iterator to values given a single kpi and year.
        """
        url = SimpleURL(cls.pre(), 'data', 'peryear', kpi.id, str(year))
        for post in KoladaFetcher.get_iter(url):
            yield cls(post)

            
    def __unicode__(self):
        values = [unicode(getattr(self, name)) for name in self.reprs]
        return u' '.join(values)

    def __str__(self):
        return asutf8(unicode(self))
    

                
class Value(ValueBase):
    """
    Value representation of koladas municipality data:

    members:
       kpi          - unicode
       municipality - unicode
       period       - unicode
       value        - float (or None)
       value_f      - float (or None)
       value_m      - float (or None)
    """ 
    reprs = ('kpi', 'municipality', 'period', 'value', 'value_f','value_m')
    
    @classmethod
    def permunicipality(cls, kpi, municipality):
        url = SimpleURL('data','permunicipality', kpi.id, municipality.id)
        for post in KoladaFetcher.get_iter(url):
            yield cls(post)


class OUValue(ValueBase):
    """
    Value representation of koladas municipality data:

    members:
       kpi          - unicode
       ou           - unicode
       period       - unicode
       value        - float (or None)
       value_f      - float (or None)
       value_m      - float (or None)
    """ 
    reprs = ('kpi', 'ou', 'period', 'value', 'value_f','value_m')
    
    @staticmethod
    def pre():
        return u'ou'
    
    @classmethod
    def perou(cls, kpi, ou):
        url = SimpleURL('ou', 'data','perou', kpi.id, ou.id)
        for post in KoladaFetcher.get_iter(url):
            yield cls(post)


    
if __name__ == '__main__':
    print "Fetching kpis"
    logging.basicConfig(level=logging.DEBUG)
    #logger.setLevel(logging.DEBUG)
    kpis =  list(KPI.list())
    municipalities = list(Municipality.list())
    
    #for value in Value.peryear(KPI('N61873'), 2011):
    #    print value
    #for value in Value.permunicipality(KPI('N01827'), Municipality('2262')):
    #    print value
    #for ou in OU.list('Blomman'):
    #    print ou
    #    pass
    #for value in Value.exact([KPI('N01827'), KPI('N01828')], municipalities, range(2012, 2013)):
    #     print value

    #for value in Value.get(kpis, municipalities, range(2011, 2013)):
    #    print value

    #for m in municipalities:
    #    ous = list(OU.list(municipality=m))
    #    for ou in ous:
    #        print m , ou

