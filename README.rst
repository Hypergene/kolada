What is it
==========

This is a simple wrapper for the public kolada web-api. Kolada
contains a collection of standardized KPIs (key performance indicator)
concerning Swedish municipalities. Read more at kolada_ (assuming you
understand swedish).

.. _kolada: http://www.kolada.se


Basic usage
===========

There are 4 basic dimensions in the kolada data.

1. KPI
2. Municipality
3. OU (Organizational unit)
4. Period

Retrive a list with all kpis with 'skola' as a part of the title::

   import kolada.api as kolada
   kpis = kolada.KPI.list('skola')

For each of those KPIs get data from one of the Swedish
municipalities, Ale (1440) and periods 2011-2013::

   ale = kolada.Municipality('1440')
   for rec in kolada.Value.get(kpis, ale, range(2011, 2014)):
      # do stuff with records
