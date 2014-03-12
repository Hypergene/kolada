Kolada API
==========

`Kolada <http://www.kolada.se>`_ provides a web-service for accessing standardized `key
performance indicators <http://en.wikipedia.org/wiki/Performance_indicator>`_ (KPI) concerning Swedish municipalities.
This project describes that API and includes examples for accessing
it in

* `python <https://github.com/Hypergene/kolada/tree/master/python>`_
* `javascript <https://github.com/Hypergene/kolada/tree/master/javascript>`_


Data and metadata
-----------------

Key performance indicator values are referred to as **data** whereas **metadata** describes

* Key performance indicator
* Municipality
* Organizational unit (OU)

For a proper query URL you need metadata such as id of a KPI and municipality or organizational unit. See the examples below.
For each query the result is

* in **JSON** format
* limited to 100 items

Routes
------

The service is found at **http://api.kolada.se/v1/** and provides a
read only API. Each response from the service
if it's correct returns a JSON structure like::

    {
        "values": [ {obj}, {obj}, ..., {obj}],
        "count": <int>,          // Length of values
        "previous": "<string>",  // Optional: Full URL to previous page, if any
        "next": "<string>"       // Optional: Full URL to next page, if any
    }

The **obj** structure differs between metadata, find out in
the below section what it looks like for each.

Metadata
--------

For each query remember to `url-encode
<http://www.w3schools.com/tags/ref_urlencode.asp>`_ the SEARCH_STR.

KPI
    * SEARCH_STR = Män som tar ut tillfällig föräldrapenning

    `<http://api.kolada.se/v1/kpi/M%C3%A4n%20som%20tar%20ut%20tillf%C3%A4llig%20f%C3%B6r%C3%A4ldrapenning>`_

Object structure::

    {
        "id": "<string>",
        "title": "<string>",
        "description": "<string>",
        "definition": "<string>",
        "municipality_type": "L|K",
        "divided_by_gender": <bool>
    }



Municipality
    * SEARCH_STR = lund

    `<http://api.kolada.se/v1/municipality/lund>`_

Object structure::

    {
        "id": "<string>",
        "title": "<string>",
        "type": "L|K"
    }

type
    - **L** is short for County Council `(swedish: Landsting)`
    - **K** is short for municipality  `(swedish: Kommun)`




Organizational units within a municipality
__________________________________________

Note! You cannot query organizational units across multiple
municipalities.

Example
    * MUNICIPLAITY_ID = 1281
    * SEARCH_STR = skola

    `<http://api.kolada.se/v1/ou/1281/skola>`_

Object structure::

    {
        "id": "<string>",
        "title": "<string>"
    }


Query data
----------

Data queries are divided in two levels, municipality and organizational
unit level. Once you know the metadata for KPI, municipality or
organizational unit  you can query values for that KPI. For the
municipality level

/v1/data/exact/KPI/MUNICIPALITY_ID/PERIOD
    Example: http://api.kolada.se/v1/data/exact/N00945/1860/2009,2007

    * Note! KPI, MUNICIPALITY_ID and PERIOD can all be comma separated strings. The URL length is the limit which differs across browsers.


/v1/data/peryear/KPI/PERIOD
    Example: http://api.kolada.se/v1/data/peryear/N00945/2009

/v1/data/permunicipality/KPI/MUNICIPALITY_ID
    Example: http://api.kolada.se/v1/data/permunicipality/N00945/1860

Object structure::

    {
        "kpi": "<string>",
        "municipality": "<string>",
        "period": "<string>",
        "value": <float>,     // Both male and female
        "value_m": <float>,   // Male, null if no value exists
        "value_f": <float>    // Female, null if no value exists
    }

For the organizational unit level

/v1/ou/data/exact/KPI/OU_ID/PERIOD
    * Example: http://api.kolada.se/v1/ou/data/exact/N15033/V15E144001301/2009,2007
    * Example with multiple KPI's and OU_ID's http://api.kolada.se/v1/ou/data/exact/N15033,N15030/V15E144001301,V15E144001101/2009,2008,2007

/v1/ou/data/peryear/KPI/PERIOD
    Example: http://api.kolada.se/v1/ou/data/peryear/N15033/2007

/v1/ou/data/perou/KPI/OU_ID
    Example: http://api.kolada.se/v1/ou/data/perou/N15033/V15E144001301



Object structure::

    {
        "kpi": "<string>",
        "out": "<string>",
        "period": "<string>",
        "value": <float>,
        "value_m": <float>,
        "value_f": <float>
    }

