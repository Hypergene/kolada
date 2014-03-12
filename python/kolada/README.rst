Usage
=====

Retrive a list with all kpis with 'skola' as a part of the title::

   import kolada.api as kolada
   kpis = list(kolada.KPI.list('skola'))

For each of those KPIs get data from one of the Swedish
municipalities, Ale (1440) and periods 2011-2013::

   ale = kolada.Municipality('1440')
   for rec in kolada.Value.get(kpis, ale, range(2011, 2014)):
      # do stuff with records
