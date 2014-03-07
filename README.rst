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
* Period (year)

Each dimension is described using **metadata** which is also
accessible via the service.


Query results
-------------

* In each case the service will respond with **JSON** responses.
* Each response is limited to 100 items

Routes
------

The service is found at `http://api.kolada.se/v1/` and provides a
read only API. Before you can query for data you need to have
proper metadata to feed your query.
Each response from the service if it's correct returns a JSON structure like::

    {
        "values": [ {obj}, {obj}, ..., {obj}],
        "count": <int>,          // Length of values
        "previous": "<string>",  // Full URL to previous page, if any
        "next": "<string>"       // Full URL to next page, if any
    }



Metadata
________

For each query below remember to `url-encode <http://www.w3schools.com/tags/ref_urlencode.asp>`_ the SEARCH_STR.

KPI::

    http://api.kolada.se/v1/kpi/[SEARCH_STR]

returns object structure like this::

    {
        "id": "<string>",
        "title": "<string>",
        "description": "<string>",
        "definition": "<string>",
        "municipality_type": "L|K",
        "divided_by_gender": <bool>
    }

`Example SEARCH_STR = Män som tar ut tillfällig föräldrapenning <http://api.kolada.se/v1/kpi/M%C3%A4n%20som%20tar%20ut%20tillf%C3%A4llig%20f%C3%B6r%C3%A4ldrapenning>`_


Municipality::

    http://api.kolada.se/v1/municipality/[SEARCH_STR]

returns object structure like this::

    {
        "id": "<string>",
        "title": "<string>",
        "type": "L|K"
    }


Organizational units within a municipality::

    http://api.kolada.se/v1/out/[municipality id]/[SEARCH_STR]


