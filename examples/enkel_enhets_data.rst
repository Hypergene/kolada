Enhetsdata för ett givet nyckeltal
==================================

På enhetsnivå finns data bara på några nyckeltal. Det finns ett fält
på metadatan för nyckeltal om det kan finnas data på enhetsnivå
(has_ou_data). Nedan kommer ett exempel på detta.


Ta fram data
-------------

Tidigare har vi sett hur vi kan få fram data från en
nyckeltalsgruppering. Vi tittade på kommunens kvalitet i korthet. Nu
tittar vi på en skolnyckeltal som har data på enhetnivå, "Meritvärde i
åk. 9 lägeskommun, genomsnitt" med id N15501. Vi fortsätter med att
titta på Helsingborg som exempel. (kom ihåg att Helsingborg har id=1283).

    `<http://api.kolada.se/v2/oudata/kpi/U21468/year/2013?municipality=1283>`_

När vi får fram detta data kommer
det ju en mängd enheter som vi antagligen inte har någon aning om vad dessa
är för några. Vi kan fråga om OU 

    `<http://api.kolada.se/v2/ou/V15E128300201>`_

etc. för en mängd id:n, men i detta fallet är det väldigt många, så vi kan
lika bra ta fram alla enheter för Helsingborg på en gång

   `<http://api.kolada.se/v2/ou?municipality=1283>`_

och hitta de som vi fått.
