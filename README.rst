Kolada API
==========

`Kolada <http://www.kolada.se>`_ provides a web-service for accessing
standardized `key performance indicators
<http://en.wikipedia.org/wiki/Performance_indicator>`_ (KPI)
concerning Swedish municipalities and organizational units. The
database is created and upheld by `RKA <http://www.rka.nu/>`_ This
project describes that API and includes examples for accessing it.


The kolada database
--------------------

Within the kolada database there are two different types of data. 

* The municipal data 
* The organizational unit data

The municipal data contains KPIs for the Swedish kommuner and
landsting, there are about 3500 KPIs within the database. On
organizational unit level the data is less comprehensive, but there
are still quite a few KPIs on this level. Each KPI is currently
measured once a year, and the data may be divided by gender. I.e. for
each KPI there may be at most three values, based on gender being
female, male and a total.

The general structure of the data is::

    KPI, municipal or organizational unit, year, gender: data

In other words, the database has only four basic dimension for each
dataset. 

To help client make relevant queries, there are two help-dimensions,
KPI groups and Municipal groups. The KPI groups consists of the groups
that RKA have chosen to publish as 'Färdiga presentationer' (readymade
presentations) on `kolada <http://www.kolada.se>`_. 

The municipal groups are groups present in koladas various
reports. Data is also present for each municipal group, this value is
an unweighted average of the members.


Below is a technical information about the URL:s in koladas Web
API. If you know swedish you may also be interested in the examples folder, 
e.g `examples/kommun_i_korthet.rst`_



Technical information about data and metadata
---------------------------------------------

Key performance indicator values are referred to as **data** whereas **metadata** describes

* Key performance indicator
* Municipality
* Organizational unit (OU)

For a proper query URL you need metadata such as id of a KPI and municipality or organizational unit. See the examples below.
For each query the result is

* in **JSON** format
* limited to 5000 items for each request

Note! To read all entries for a query you need to retrieve each page by following the URL in the **next_page** field, see the 
Routes section for more information.

Routes
------

The service is found at **api.kolada.se/v2/...** and provides a
read only API. Each response from the service
if it's correct returns a JSON structure like::

    {
        "values": [ {obj}, {obj}, ..., {obj}],
        "count": <int>,               // Length of values
        "previous_page": "<string>",  // Optional: Full URL to previous page, if any
        "next_page": "<string>"       // Optional: Full URL to next page, if any
    }

The **obj** structure differs between metadata, find out in
the below section what it looks like for each.

For all URL:s a parameter *per_page* may be given, which will limit
the number of posts in the result. The default value is the
maximum, 5000.

Metadata
--------

For each query remember to `url-encode
<http://www.w3schools.com/tags/ref_urlencode.asp>`_ the SEARCH_STR.

All metadata may be queried on the form

  * entity/id
  * entity?title=...

where entity may be 

  * kpi
  * kpi_groups
  * municipality
  * municipality_groups
  * ou

The id may be a comma separated string of many ids.


Examples
________

KPI
    * SEARCH_STR = Män som tar ut tillfällig föräldrapenning

    `<http://api.kolada.se/v2/kpi?title=M%C3%A4n%20som%20tar%20ut%20tillf%C3%A4llig%20f%C3%B6r%C3%A4ldrapenning>`_

Object structure::

    {
        "auspice": "<string>",
        "id": "<string>",
        "title": "<string>",
        "description": "<string>",
        "definition": "<string>",
        "municipality_type": "L|K",
        "is_divided_by_gender": <int>,
        "operating_area": "<string>",
        "ou_publication_date": "<string>" or null,
        "perspective": "<string>",
        "publication_date": "<string>" or null,
        "unit": "<string>"
    }



Municipality
    * SEARCH_STR = lund

    `<http://api.kolada.se/v2/municipality?title=lund>`_

Object structure::

    {
        "id": "<string>",
        "title": "<string>",
        "type": "L|K"
    }

type
    - **L** is short for County Council `(swedish: Landsting)`
    - **K** is short for municipality  `(swedish: Kommun)`




Organizational units 
_____________________


Example:
    * SEARCH_STR = skola

    `<http://api.kolada.se/v2/ou?title=skola>`_

Object structure::

    {
        "id": "<string>",
        "municipality": "<string>",
        "title": "<string>"
    }

you may optionally give a municipal as a parameter, e.g.:

    `<http://api.kolada.se/v2/ou?municipal=0114&title=skola>`_

which will return all OUs from municipal 'Upplands Väsby', where
'skola' is part of the title.
    


Groups
_______

There a two types of groups defined by the by the API, 

   * KPI groups
   * Municipality groups

Example:
    * SEARCH_STR = kostnad

    `<http://api.kolada.se/v2/kpi_groups?title=kostnad>`_

Object structure::

    {
        "id": "<string>",
        "title": "<string>",
        "members": [
            {"id": "<string>", "title": "<string>"}
            ...
        ]
    }



Query data
----------

Data queries are on the following forms, the form where all entities are given: 

/v2/data/kpi/<KPI>/municipality/<MUNICIPALITY_ID>/year/<PERIOD>

Here, the MUNICIPALITY_ID may be that of a group.

    Example: http://api.kolada.se/v2/data/kpi/N00945/municipality/1860/year/2009,2007

    * Note! KPI, MUNICIPALITY_ID and PERIOD can all be comma separated strings. The URL length is the limit which differs across browsers.


or where only two are given:

/v2/data/kpi/<KPI>/year/<PERIOD>
    Example: http://api.kolada.se/v2/data/kpi/N00945/year/2009

/v2/data/kpi/<KPI>/municipality/<MUNICIPALITY_ID>
    Example: http://api.kolada.se/v2/data/kpi/N00945/municipality/1860

/v2/data/municipality/<MUNICIPALITY_ID>/year/<PERIOD>
    Example: http://api.kolada.se/v2/data/municipality/1860/year/2009


Object structure::

    {
        "kpi": "<string>",
        "municipality": "<string>",
        "period": "<string>",
        "values: [
           {"count": <int>, "gender": "T|K|F", "status": "<string>", "value": <float> or null}
           ...
        ]
    }

The values array may at most contain three entries, one for each
gender. 'count' we only differ from 1 when the municipality is a
group. In this case the count will be the number of members in that
group which contributed to the value, which is an unweighted average.


For the organizational unit level, this are exacly the same as above
except we are working with ou instead of municipality.

/v2/oudata/kpi/<KPI>/ou/<OU_ID>/year/<PERIOD>
    * Example: http://api.kolada.se/v2/oudata/kpi/N15033/ou/V15E144001301/year/2009,2007
    * Example with multiple KPI's and OU_ID's http://api.kolada.se/v2/oudata/kpi/N15033,N15030/ou/V15E144001301,V15E144001101/year/2009,2008,2007

/v2/oudata/kpi/<KPI>/year/<PERIOD>
    Example: http://api.kolada.se/v2/oudata/kpi/N15033/year/2007

/v1/oudata/kpi/<KPI</ou/<OU_ID>
    Example: http://api.kolada.se/v2/oudata/kpi/N15033/ou/V15E144001301

/v1/oudata/ou/<KPI</year/<PERIOD>
    Example: http://api.kolada.se/v2/oudata/ou/V15E144001301/year/2007



Object structure::

    {
        "kpi": "<string>",
        "out": "<string>",
        "period": "<string>",
        "values": [
           {"count": <int>, "gender": "T|K|F", "status": "<string>", "value": <float> or null},
           ...
        ]
    }



Error-codes
-----------

Since this is a read-only API, and not a very strict one, there are
not many error you can encounter. But the following may happen

* HTTP 404 - the url requested did not match any of the URLs described above.
* HTTP 400 - Typically some or many of the paramaters given in the
  URL, were illegal. But a too long URL also generates a HTTP 400
  error.
* HTTP 500 - There are some error which will generate a
  500-code. Typically if you encounter this is should be reported to
  RKA.


