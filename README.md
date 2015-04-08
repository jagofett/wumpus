**Wumpus világ**
===================
##Leírás ##

A játék n X n-es, leggyakrabban 4 X 4-es, esetleg 6 X 6-os táblája egy barlang helyiségeit ábrázolja, amelyek egyikében rejtőzik egy szörnyeteg, a wumpus, aki felfalja a szobájába tévedő személyt. A táblán egy játékos közlekedik, akire a wumpus mellett kívül még egy veszély leselkedik: néhány mező – szintén a játék végét jelentő – csapdát tartalmaz. A nyerésre is két lehetősége van: nyilával megsemmisítheti a wumpust vagy rátalálhat az aranyat rejtő szobára.

A játékos tájékozódásában az segíti, hogy minden veszélyt keltő objektumok közelségét érzékeli a szomszédos szobákból. A wumpus szomszédságában bűzt érez, a csapda mellett szellőt érzékel. Az arany a legtöbb játékban nem jelzi közelségét (valamilyen fényeffektussal), tehát mintegy „véletlenül” lehet rátalálni.

##Specifikáció##
A játék egy n X n-es táblából (barlang) áll, az egyes mezők jelképezik a barlang szobáit. A barlangban az alábbiak találhatóak meg:

- Wumpus: a játék nevét adó szörny, amennyiben a játékos erre a mezőre lép, meghal, a játéknak vége. A közvetlenül szomszédos mezőkön bűz érzékelhető (átlósan tehát nem!). 
- Csapda: a játékban néhány (nehézségi szinttől függő, illetve paraméterezhető) csapda található – ezek aszintén a játékos halálát okozzák, ha ide lép. A csapdák számát a játékos nem ismeri. A csapdákal közvetlen szomszédos mezőkön szellő érzékelhető.
- Arany: egy szobában van arany, a játékos célja ezt megtalálni. Amennyiben ez sikerül, megnyeri a játékot. Az aranyat csak azon a helyen lehet érzékelni, ahol van (nincs kisugárzása)


A fenti kép egy 4 x 4-es barlang lehetséges kezdőállapotát mutatja.
A játékos a bal alsó szobában kezd (ez garantáltan biztonságos hely, bűz vagy szellő sem érzékelhető – kizárva így a szerencse faktort).


--
ELTE MSc Design and Implementation of an Algorithm - Wumpus World Game
