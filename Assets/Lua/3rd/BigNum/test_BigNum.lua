--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
--
--  File Name:              test_BigNum.lua
--  Package Name:           BigNum 
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
--    Tests the Big Numbers library.
--
--$.%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

require( "BigRat.lua" ) ;

--Testa new{{{1
print( "Testing BigNum.mew:" ) ;
----string s/ sinal
a = BigNum.new( "1234" ) ;
print( BigNum.mt.tostring( a ) ) ;
----bignum s/ sinal
a = BigNum.new( a ) ;
print( BigNum.mt.tostring( a ) ) ;
----string c/ sinal -
a = BigNum.new( "-4567" ) ;
print( BigNum.mt.tostring( a ) ) ;
----bignum c/ sinal -
a = BigNum.new( a ) ;
print( BigNum.mt.tostring( a ) ) ;
----string c/ sinal +
a = BigNum.new( "+65413" ) ;
print( BigNum.mt.tostring( a ) ) ;
----bignum c/ sinal +
a = BigNum.new( a ) ;
print( BigNum.mt.tostring( a ) ) ;

--Testa funções de comparação{{{1
print( "\nTesting relational operators" ) ;
a = BigNum.new( "3452" ) ;
b = BigNum.new( "1543" ) ;
c = BigNum.new( "53231" ) ;
d = BigNum.new( "3452" ) ;
print( BigNum.mt.tostring( a ) .. " > " .. BigNum.mt.tostring( b ) .. " " ) ;
assert( ( a > b ) ) ;
print( BigNum.mt.tostring( a ) .. " < " .. BigNum.mt.tostring( c ) .. " " ) ;
assert( ( a < c ) ) ;
print( BigNum.mt.tostring( a ) .. " >= " .. BigNum.mt.tostring( b ) .. " " ) ;
assert( ( a >= b ) ) ;
print( BigNum.mt.tostring( a ) .. " <= " .. BigNum.mt.tostring( d ) .. " " ) ;
assert( ( a <= d ) ) ;
print( "--Relational operators OK" ) ;

--Testa unary minus{{{1
print( "\nTesting unary minus:" ) ;
a = BigNum.new( "35135" ) ;
b = BigNum.new( "-35135" ) ;
c = -a ;
print( c ) ;
assert( b == c ) ;
a = BigNum.new( "-0029871" ) ;
b = BigNum.new( "29871" ) ;
c = -a ;
print( c ) ;
assert( b == c ) ;
print( "--Unary minus OK" ) ;

--Testa adição(metatable){{{1
print( "\nTesting addition:" ) ;
----positivo+positivo{{{2
a = BigNum.new( "3456" ) ;
b = BigNum.new( "50" ) ;
r = BigNum.new( "3506" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "999" ) ;
b = BigNum.new( "99999" ) ;
r = BigNum.new( "100998" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "999" ) ;
r = BigNum.new( "1001" ) ;
c = a + 2 ;
print(BigNum.mt.tostring(a) .. " + 2 = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
b = BigNum.new( "99999" ) ;
r = BigNum.new( "100001" ) ;
c = 2 + b ;
print("2 + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;

----positivo+negativo{{{2
------a>b{{{3
a = BigNum.new( "621364" ) ;
b = BigNum.new( "-45556" ) ;
r = BigNum.new( "575808" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "99999" ) ;
b = BigNum.new( "-1000" ) ;
r = BigNum.new( "98999" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "85961" ) ;
b = BigNum.new( "-0001" ) ;
r = BigNum.new( "85960" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigNum.new( "3456" ) ;
b = BigNum.new( "-54286" ) ;
r = BigNum.new( "-50830" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "99999" ) ;
b = BigNum.new( "-100000" ) ;
r = BigNum.new( "-1" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----negativo+positivo{{{2
------a>b{{{3
a = BigNum.new( "-652489" ) ;
b = BigNum.new( "110025" ) ;
r = BigNum.new( "-542464" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-99999" ) ;
b = BigNum.new( "999" ) ;
r = BigNum.new( "-99000" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigNum.new( "-65001" ) ;
b = BigNum.new( "205165" ) ;
r = BigNum.new( "140164" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-684" ) ;
b = BigNum.new( "12035" ) ;
r = BigNum.new( "11351" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----negativo+negativo{{{2
------a>b{{{3
a = BigNum.new( "-654315" ) ;
b = BigNum.new( "-8465" ) ;
r = BigNum.new( "-662780" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-99999" ) ;
b = BigNum.new( "-999" ) ;
r = BigNum.new( "-100998" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
-----b>a{{{3
a = BigNum.new( "-854632" ) ;
b = BigNum.new( "-998854" ) ;
r = BigNum.new( "-1853486" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-1235" ) ;
b = BigNum.new( "-365421" ) ;
r = BigNum.new( "-366656" ) ;
c = a + b ;
print(BigNum.mt.tostring(a) .. " + " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Addition OK" ) ;
--Testa subtracao(metatable){{{1
print( "\nTesting subtraction:" ) ;
----positivo-positivo{{{2
a = BigNum.new( "3456" ) ;
b = BigNum.new( "50" ) ;
r = BigNum.new( "3406" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "999" ) ;
b = BigNum.new( "99999" ) ;
r = BigNum.new( "-99000" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----positivo-negativo{{{2
------a>b{{{3
a = BigNum.new( "621364" ) ;
b = BigNum.new( "-45556" ) ;
r = BigNum.new( "666920" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "99999" ) ;
b = BigNum.new( "-1000" ) ;
r = BigNum.new( "100999" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "85961" ) ;
b = BigNum.new( "-0001" ) ;
r = BigNum.new( "85962" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigNum.new( "3456" ) ;
b = BigNum.new( "-54286" ) ;
r = BigNum.new( "57742" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "99999" ) ;
b = BigNum.new( "-100000" ) ;
r = BigNum.new( "199999" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----negativo-positivo{{{2
------a>b{{{3
a = BigNum.new( "-652489" ) ;
b = BigNum.new( "110025" ) ;
r = BigNum.new( "-762514" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-99999" ) ;
b = BigNum.new( "999" ) ;
r = BigNum.new( "-100998" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigNum.new( "-65001" ) ;
b = BigNum.new( "205165" ) ;
r = BigNum.new( "-270166" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-684" ) ;
b = BigNum.new( "12035" ) ;
r = BigNum.new( "-12719" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----negativo-negativo{{{2
------a>b{{{3
a = BigNum.new( "-654315" ) ;
b = BigNum.new( "-8465" ) ;
r = BigNum.new( "-645850" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-99999" ) ;
b = BigNum.new( "-999" ) ;
r = BigNum.new( "-99000" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
------b>a{{{3
a = BigNum.new( "-854632" ) ;
b = BigNum.new( "-998854" ) ;
r = BigNum.new( "144222" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-1235" ) ;
b = BigNum.new( "-365421" ) ;
r = BigNum.new( "364186" ) ;
c = a - b ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Subtraction OK" ) ;
--Multiplicação(metatable){{{1
print( "\nTesting multiplication:" ) ;
----positivo*positivo{{{2
a = BigNum.new( "3456" ) ;
b = BigNum.new( "50" ) ;
r = BigNum.new( "172800" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "999" ) ;
b = BigNum.new( "99999" ) ;
r = BigNum.new( "99899001" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----positivo*negativo{{{2
a = BigNum.new( "6532" ) ;
b = BigNum.new( "-3152" ) ;
r = BigNum.new( "-20588864" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "1548" ) ;
b = BigNum.new( "-99999" ) ;
r = BigNum.new( "-154798452" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----negativo*positivo{{{2
a = BigNum.new( "-52489" ) ;
b = BigNum.new( "2300" ) ;
r = BigNum.new( "-120724700" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-785" ) ;
b = BigNum.new( "45632" ) ;
r = BigNum.new( "-35821120" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
----negativo*negativo{{{2
a = BigNum.new( "-3456" ) ;
b = BigNum.new( "-46587" ) ;
r = BigNum.new( "161004672" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
a = BigNum.new( "-99900" ) ;
b = BigNum.new( "-9999" ) ;
r = BigNum.new( "998900100" ) ;
c = a * b ;
print(BigNum.mt.tostring(a) .. " * " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;
print( "--Multiplication OK" ) ;
--Divisão(metatable){{{1
print( "\nTesting division:" ) ;
a = BigNum.new( "68452" ) ;
b = BigNum.new( "21" ) ;
c = BigNum.new( ) ;
d = BigNum.new( ) ;
r1 = BigNum.new( "3259" ) ;
r2 = BigNum.new( "13" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " | " .. BigNum.mt.tostring(d)) ;
assert( c == r1 ) ;
assert( d == r2 ) ;
a = BigNum.new( "-123000" ) ;
b = BigNum.new( "4214" ) ;
r1 = BigNum.new( "-29" ) ;
r2 = BigNum.new( "794" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " | " .. BigNum.mt.tostring(d)) ;
assert( c == r1 ) ;
assert( d == r2 ) ;
a = BigNum.new( "10000" ) ;
b = BigNum.new( "-58" ) ;
r1 = BigNum.new( "-172" ) ;
r2 = BigNum.new( "24" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " | " .. BigNum.mt.tostring(d)) ;
assert( c == r1 ) ;
assert( d == r2 ) ;
a = BigNum.new( "-352165" ) ;
b = BigNum.new( "-464" ) ;
r1 = BigNum.new( "758" ) ;
r2 = BigNum.new( "453" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " | " .. BigNum.mt.tostring(d)) ;
assert( c == r1 ) ;
assert( d == r2 ) ;
a = BigNum.new( "35026" ) ;
b = BigNum.new( "30461" ) ;
r1 = BigNum.new( "1" ) ;
r2 = BigNum.new( "4565" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " | " .. BigNum.mt.tostring(d)) ;
assert( c == r1 ) ;
assert( d == r2 ) ;
a = BigNum.new( "1494" ) ;
b = BigNum.new( "83" ) ;
r1 = BigNum.new( "18" ) ;
r2 = BigNum.new( "0" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " | " .. BigNum.mt.tostring(d)) ;
assert( c == r1 ) ;
assert( d == r2 ) ;
print( "--Division OK" ) ;
--Exp{{{1
print( "\nTesting power:" ) ;
a = BigNum.new( "2" ) ;
b = BigNum.new( "50" ) ;
r = BigNum.new( "1125899906842624" ) ;
c = BigNum.exp( a , b ) ;
print(BigNum.mt.tostring(a) .. " ^ " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;

a = BigNum.new( "13" ) ;
b = BigNum.new( "38" ) ;
r = BigNum.new( "2137210935411428674141543654682486133398329" ) ;
c = BigNum.exp( a , b ) ;
print(BigNum.mt.tostring(a) .. " ^ " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
assert( c == r ) ;

print( "--Power OK" ) ;

--GCD{{{1
print( "\nTesting greatest common divisor:" ) ;
a = BigNum.new( "64" ) ;
b = BigNum.new( "20" ) ;
r = BigNum.new( "4" ) ;
c = BigNum.gcd( a , b ) ;
print( "GCD(" .. BigNum.mt.tostring( a ) .. "," .. BigNum.mt.tostring( b ) .. ") = " .. BigNum.mt.tostring( c ) ) ;
assert( c == r ) ;
a = BigNum.new( "213549" ) ;
b = BigNum.new( "32154" ) ;
r = BigNum.new( "3" ) ;
c = BigNum.gcd( a , b ) ;
print( "GCD(" .. BigNum.mt.tostring( a ) .. "," .. BigNum.mt.tostring( b ) .. ") = " .. BigNum.mt.tostring( c ) ) ;
assert( c == r ) ;
a = BigNum.new( "65487" ) ;
b = BigNum.new( "100513" ) ;
r = BigNum.new( "83" ) ;
c = BigNum.gcd( a , b ) ;
print( "GCD(" .. BigNum.mt.tostring( a ) .. "," .. BigNum.mt.tostring( b ) .. ") = " .. BigNum.mt.tostring( c ) ) ;
assert( c == r ) ;
print( "--GCD OK" ) ;
