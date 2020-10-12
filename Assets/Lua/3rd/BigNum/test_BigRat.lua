--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
--
--  File Name:              test_BigRat.lua
--  Package Name:           BigRat
--
--  Project:    Big Numbers library for Lua 
--  Mantainers: fmp - Frederico Macedo Pessoa
--              msm - Marco Serpa Molinaro
--
--  History:
--     Version      Autor       Date            Notes
--      alfa     fmp/msm    03/22/2003   Start of Development
--      beta     fmp/msm    07/11/2003   Release
--
--  Description:
--    Tests the Big Rationals library.
--
--$.%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

require( "BigRat.lua" ) ;

--Testa new{{{1
print( "Testing BigRat.mew:" ) ;
----string s/ sinal
a = BigRat.new( "1234" , 32 ) ;
print( a ) ;
----bigrat s/ sinal
a = BigRat.new( a ) ;
print( a ) ;
----string c/ sinal -
a = BigRat.new( "4567" , "-12" ) ;
print( a ) ;
----bigrat c/ sinal -
a = BigRat.new( a ) ;
print( a ) ;
----string c/ sinal +
a = BigRat.new( "+65413" , "13" ) ;
print( a ) ;
----bigrat c/ sinal +
a = BigRat.new( a ) ;
print( a ) ;

--Testa funções de comparação{{{1
print( "\nTesting relational operators" ) ;
a = BigRat.new( "3452" , "45" ) ;
b = BigRat.new( "1543" , "40" ) ;
c = BigRat.new( "53231" , "100" ) ;
d = BigRat.new( "3452" , "45" ) ;
print( BigRat.mt.tostring( a ) .. " > " .. BigRat.mt.tostring( b ) .. " " ) ;
assert( ( a > b ) ) ;
print( BigRat.mt.tostring( a ) .. " < " .. BigRat.mt.tostring( c ) .. " " ) ;
assert( ( a < c ) ) ;
print( BigRat.mt.tostring( a ) .. " >= " .. BigRat.mt.tostring( b ) .. " " ) ;
assert( ( a >= b ) ) ;
print( BigRat.mt.tostring( a ) .. " <= " .. BigRat.mt.tostring( d ) .. " " ) ;
assert( ( a <= d ) ) ;
print( "--Relational operators OK" ) ;

--Testa unary minus{{{1
print( "\nTesting unary minus:" ) ;
a = BigRat.new( "35135" , "6542" ) ;
c = BigRat.new( "-35135" , "6542" ) ;
b = -a ;
print( b ) ;
print(b == c);
a = BigRat.new( "-0029871" , "1165" ) ;
c = BigRat.new( "29871" , "1165" ) ;
b = -a ;
print( b ) ;
assert( b == c ) ;
print( "--Unary minus OK" ) ;

--Testa adição(metatable){{{1
print( "\nTesting addition:" ) ;
----positivo+positivo{{{2
a = BigRat.new( "3456" , "21" ) ;
b = BigRat.new( "50" , "41" ) ;
r = BigRat.new( "47582" , "287" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "999" , "13" ) ;
b = BigRat.new( "99999" , "555" ) ;
r = BigRat.new( "618144" , "2405" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "999" , "10" ) ;
r = BigRat.new( "1019" , "10" ) ;
c = a + 2 ;
print(BigRat.mt.tostring(a) .. " + 2 = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
b = BigRat.new( "99999" , "422" ) ;
r = BigRat.new( "100843" , "422" ) ;
c = 2 + b ;
print("2 + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;

----positivo+negativo{{{2
------a>b{{{3
a = BigRat.new( "621364" , "23" ) ;
b = BigRat.new( "-45556" , "987" ) ;
r = BigRat.new( "87462640" , "3243" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "99999" , "122" ) ;
b = BigRat.new( "-1000" , "34" ) ;
r = BigRat.new( "1638983" , "2074" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "85961" , "311" ) ;
b = BigRat.new( "-0001" , "41" ) ;
r = BigRat.new( "3524090" , "12751" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigRat.new( "3456" , "21" ) ;
b = BigRat.new( "-54286" , "444" ) ;
r = BigRat.new( "65743" , "1554" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "99999" , "412" ) ;
b = BigRat.new( "-100000" , "67" ) ;
r = BigRat.new( "-34500067" , "27604" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----negativo+positivo{{{2
------a>b{{{3
a = BigRat.new( "-652489" , "23" ) ;
b = BigRat.new( "110025" , "412" ) ;
r = BigRat.new( "-266294893" , "9476" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-99999" , "461" ) ;
b = BigRat.new( "999" , "74" ) ;
r = BigRat.new( "-187551" , "922" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigRat.new( "-65001" ) ;
b = BigRat.new( "205165" ) ;
r = BigRat.new( "140164" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-684" ) ;
b = BigRat.new( "12035" ) ;
r = BigRat.new( "11351" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----negativo+negativo{{{2
------a>b{{{3
a = BigRat.new( "-654315" , "31" ) ;
b = BigRat.new( "-8465" , "543" ) ;
r = BigRat.new( "-355555460" , "16833" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-99999" , "134" ) ;
b = BigRat.new( "-999" , "0018" ) ;
r = BigRat.new( "-53718" , "67" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
-----b>a{{{3
a = BigRat.new( "-854632" ) ;
b = BigRat.new( "-998854" , "344" ) ;
r = BigRat.new( "-147496131" , "172" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-1235" , "-1233" ) ;
b = BigRat.new( "-365421" , "31" ) ;
r = BigRat.new( "-450525808" , "38223" ) ;
c = a + b ;
print(BigRat.mt.tostring(a) .. " + " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Addition OK" ) ;
--Testa subtracao(metatable){{{1
print( "\nTesting subtraction:" ) ;
----positivo-positivo{{{2
a = BigRat.new( "3456" , "32" ) ;
b = BigRat.new( "50" , "11" ) ;
r = BigRat.new( "1138" , "11" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "999" , "98" ) ;
b = BigRat.new( "99999" , "33" ) ;
r = BigRat.new( "-3255645" , "1078" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----positivo-negativo{{{2
------a>b{{{3
a = BigRat.new( "621364" , "123" ) ;
b = BigRat.new( "-45556" , "41" ) ;
r = BigRat.new( "758032" , "123" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "99999" , "444" ) ;
b = BigRat.new( "-1000" , "1234" ) ;
r = BigRat.new( "20640461" , "91316" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "85961" , "645" ) ;
b = BigRat.new( "-1" , "61" ) ;
r = BigRat.new( "5244266" , "39345" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigRat.new( "3456" , "414" ) ;
b = BigRat.new( "-54286" , "221" ) ;
r = BigRat.new( "1291010" , "5083" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "99999" , "6433" ) ;
b = BigRat.new( "-100000" , "175" ) ;
r = BigRat.new( "3775999" , "6433" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----negativo-positivo{{{2
------a>b{{{3
a = BigRat.new( "-652489" , "8707" ) ;
b = BigRat.new( "110025" , "70666" ) ;
r = BigRat.new( "-47066775349" , "615288862" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-99999" , "123" ) ;
b = BigRat.new( "999" , "31" ) ;
r = BigRat.new( "-26202" , "31" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigRat.new( "-65001" ) ;
b = BigRat.new( "205165" ) ;
r = BigRat.new( "-270166" , "1" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-684" ) ;
b = BigRat.new( "12035" ) ;
r = BigRat.new( "-12719" , "1" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----negativo-negativo{{{2
------a>b{{{3
a = BigRat.new( "-654315" , "-233" ) ;
b = BigRat.new( "-8465" , "123" ) ;
r = BigRat.new( "82453090" , "28659" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-99999" , "416" ) ;
b = BigRat.new( "-999" , "55551" ) ;
r = BigRat.new( "-1851542955" , "7703072" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Subtraction OK" ) ;
------b>a{{{3
a = BigRat.new( "-854632" , "-123" ) ;
b = BigRat.new( "-998854" , "1341" ) ;
r = BigRat.new( "422973518" , "54981" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-1235" , "416" ) ;
b = BigRat.new( "-365421" , "757" ) ;
r = BigRat.new( "11621557" , "24224" ) ;
c = a - b ;
print(BigRat.mt.tostring(a) .. " - " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
--Multiplicação(metatable){{{1
print( "\nTesting multiplication:" ) ;
----positivo*positivo{{{2
a = BigRat.new( "3456" , "-123" ) ;
b = BigRat.new( "50" , "313" ) ;
r = BigRat.new( "-57600" , "12833" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "999" , "121" ) ;
b = BigRat.new( "99999" , "55" ) ;
r = BigRat.new( "99899001" , "6655" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----positivo*negativo{{{2
a = BigRat.new( "6532" , "21" ) ;
b = BigRat.new( "-3152" , "3" ) ;
r = BigRat.new( "-20588864" , "63" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "1548" , "132" ) ;
b = BigRat.new( "-99999" , "2" ) ;
r = BigRat.new( "-12899871" , "22" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert ( c == r ) ;
----negativo*positivo{{{2
a = BigRat.new( "-52489" , "11" ) ;
b = BigRat.new( "2300" , "1290" ) ;
r = BigRat.new( "-12072470" , "1419" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-785" , "33" ) ;
b = BigRat.new( "45632" , "100" ) ;
r = BigRat.new( "-1791056" , "165" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
----negativo*negativo{{{2
a = BigRat.new( "-3456" , "234" ) ;
b = BigRat.new( "-46587" , "21" ) ;
r = BigRat.new( "2981568" , "91" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "-99900" , "21" ) ;
b = BigRat.new( "-9999" , "342" ) ;
r = BigRat.new( "18498150" , "133" ) ;
c = a * b ;
print(BigRat.mt.tostring(a) .. " * " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Multiplication OK" ) ;
--Divisão(metatable){{{1
print( "\nTesting division:" ) ;
a = BigRat.new( "68452" , "3" ) ;
b = BigRat.new( "21" , "34" ) ;
c = BigRat.new( ) ;
r = BigRat.new( "2327368" , "63" ) ;
BigRat.div( a , b , c ) ;
print(BigRat.mt.tostring(a) .. "  /  " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c) ) ;
assert( c == r ) ;
a = BigRat.new( "-123000" ) ;
b = BigRat.new( "4214" , "16" ) ;
r = BigRat.new( "-984000" , "2107" ) ;
BigRat.div( a , b , c ) ;
print(BigRat.mt.tostring(a) .. "  /  " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c) ) ;
assert( c == r ) ;
a = BigRat.new( "10000" , "45" ) ;
b = BigRat.new( "-58" , "551" ) ;
r = BigRat.new( "-19000" , "9" ) ;
BigRat.div( a , b , c ) ;
print(BigRat.mt.tostring(a) .. "  /  " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c) ) ;
assert( c == r ) ;
a = BigRat.new( "-352165" , "41" ) ;
b = BigRat.new( "-464" , "512" ) ;
r = BigRat.new( "11269280" , "1189" ) ;
BigRat.div( a , b , c ) ;
print(BigRat.mt.tostring(a) .. "  /  " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c) ) ;
assert( c == r ) ;
a = BigRat.new( "35026" , "11" ) ;
b = BigRat.new( "30461" , "90" ) ;
r = BigRat.new( "37980" , "4037" ) ;
BigRat.div( a , b , c ) ;
print(BigRat.mt.tostring(a) .. "  /  " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c) ) ;
assert( c == r ) ;
a = BigRat.new( "1494" ) ;
b = BigRat.new( "83" ) ;
r = BigRat.new( "18" , "1" ) ;
BigRat.div( a , b , c ) ;
print(BigRat.mt.tostring(a) .. "  /  " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c) ) ;
assert( c == r ) ;
print( "--Division OK" ) ;
--Pow{{{1
print( "\nTesting power:" ) ;
a = BigRat.new( "2" ) ;
b = BigRat.new( "50" ) ;
r = BigRat.new( "1125899906842624" , "1" ) ;
c = BigRat.pow( a , b ) ;

print(BigRat.mt.tostring(a) .. " ^ " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
a = BigRat.new( "13" ) ;
b = BigRat.new( "38" ) ;
r = BigRat.new( "2137210935411428674141543654682486133398329" , "1" ) ;
c = BigRat.pow( a , b ) ;
print(BigRat.mt.tostring(a) .. " ^ " .. BigRat.mt.tostring(b) .. " = " .. BigRat.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Power OK" ) ;
