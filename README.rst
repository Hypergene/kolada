Kolada API
==========

Kolada provides a web-service for accessing standardized key
performance indicators (KPI) concerning Swedish municipalities.
This project describes that API and includes examples for accessing
it in different languages.


Data and metadata
-----------------

Key performance indicator values are referred to as **data**.
They can be filtered along four dimensions

* Key performance Indicators(KPI)
* Municipality
* Organizational unit (OU) and

Each dimension is described using **metadata** which is also
accessible via the service.


Query results
-------------

* Each response is in **JSON** format.
* Each result-set is limited to 100 items

Routes
------

The service is found at `http://api.kolada.se/v1/` and provides a
read only API. Before you can query for data you need to have
proper metadata to feed your query. Each response from the service
if it's correct returns a JSON structure like::

    {
        "values": [ {obj}, {obj}, ..., {obj}],
        "count": <int>,          // Length of values
        "previous": "<string>",  // Optional: Full URL to previous page, if any
        "next": "<string>"       // Optional: Full URL to next page, if any
    }

The **obj** structure differ a bit between metadata, find out in
the below section what it looks like for each.

Metadata
--------

For each query remember to `url-encode
<http://www.w3schools.com/tags/ref_urlencode.asp>`_ the SEARCH_STR.

KPI
___

Example
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
______________

Example
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

You cannot query organizational units across multiple
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

Queries are divided in two levels, municipality and
organizational unit level.
Once you know the id of a KPI you can query values
for that KPI over a period

http://api.kolada.se/v1/data/peryear/N00945/2009

Object structure::

    {
        "kpi":"N00945",
        "municipality":"0115",
        "period":"2009",
        "value":40.02198807,  // Both male and female
        "value_m":null,       // Male
        "value_f":null        // Female
    }
