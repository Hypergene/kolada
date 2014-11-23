Kommunens kvalitet i korthet
============================

Vad vi vill åstadkomma i detta skrivna exempel är att hitta data för
Helsingborg för alla nyckeltalen i nyckeltalsgrupperna 'Kommunens
kvalitet i korthet'. För att göra detta ska vi 

1. Leta upp vilka nyckeltal som finns i gruppen.
2. Leta upp vilket id Helsingborg har i kolada.
3. Ta ut data för de ingående nyckeltalen.

Naturligtvis är det så att man egentligen behöver ett
programmeringspråk för att använda API:et, men vi ska gå igenom detta
exempel helt utan språk, och bara använda en browser.

I detta exempel behöver vi inte ta hänsyn till att kolada har ett API
som inte med nödvändighet returnerar hela resultatsettet i ett HTTP
anrop. Anlendingen är att vi vet att varje anrop kommer att returnera
mycket färre än 5000 poster, vilket är det maximala för kolada för ett
HTTP svar.


Hitta nyckeltal
---------------

Nyckeltalsgruppernas bas-url är enligt beskrivningen::

    `<http://api.kolada.se/v2/kpi_groups>`_

och vi ska leta upp 'Kommunens kvalitet i korthet', så vi går till

    `<http://api.kolada.se/v2/kpi_groups?title=kommunens%20kvalitet%20i%20korthet>`_

(Man kan typiskt skriva in 'kommunens kvalitet i korthet' i browsern
utan att ange %20 för mellanslag.) 

Detta ger ett resultat med 40 nyckeltal, i ett JSON resultatset. Om
man inte har ett programmeringsspråk att jobba med kan det vara
lättare att kopiera in resultatsettet i någon av de många
JSON-formatterarna som finns på webben. Följande nyckeltal är med i gruppen:

    N00914,N00938,N05401,N11008,N11102,N15428,N15474,N15479,N17404,N21010,N23002,N31807,U00402,U00405,U00408,U00412,U00413,U00414,U00415,U00416,U00437,U00901,U07408,U07409,U07414,U07451,U07460,U09403,U09404,U11401,U11402,U11419,U15406,U15455,U17022,U21401,U21468,U23401,U23471,U31402,U33400


Hitta id för Helsingborg
------------------------

Id:et för Helsingborg hittar vi i municipality delen,:

    `<http://api.kolada.se/v2/municipality?title=helsingborg>`_

I resultatsettet ser vi att Helsingborg har id '1283'.



Data för alla nyckeltal
-----------------------

Eftersom vi är ute efter värden på nyckeltalsgruppen för ett specifikt år, 2013, kan vi nu sätta ihop vår data-URL som:

    `<http://api.kolada.se/v2/data/kpi/N00914,N00938,N05401,N11008,N11102,N15428,N15474,N15479,N17404,N21010,N23002,N31807,U00402,U00405,U00408,U00412,U00413,U00414,U00415,U00416,U00437,U00901,U07408,U07409,U07414,U07451,U07460,U09403,U09404,U11401,U11402,U11419,U15406,U15455,U17022,U21401,U21468,U23401,U23471,U31402,U33400/municipality/1283/year/2013>`_


Där vi har använt formen:

    http://api.kolada.se/v2/data/kpi/<kpi>/municipality/<municipality>/year/<year>

Kom ihåg att på alla positioner i URL:en ovan har man rätt att ange kommaseparerade värden. 



