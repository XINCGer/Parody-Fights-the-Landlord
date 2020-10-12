--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
--
--  File Name:              test_side_effect.lua
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
--    Tests the side effects of Big Numbers functions - (BigNum) library.
--
--$.%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

--Tests side effects
require( "BigNum.lua" ) ;

--ADD
a = BigNum.new( 4364 ) ;
b = BigNum.new( 651 ) ;
c = BigNum.new( ) ;
BigNum.add( a , b , c ) ;
print( BigNum.mt.tostring( a ) .. " == 4364 " ) ;
print( BigNum.mt.tostring( b ) .. " == 651 " ) ;
a = BigNum.new( 765423 ) ;
b = BigNum.new( 1246845 ) ;
BigNum.add( a , b , c ) ;
print( BigNum.mt.tostring( a ) .. " == 765423 " ) ;
print( BigNum.mt.tostring( b ) .. " == 1246845 " ) ;

--SUB
a = BigNum.new( 2686 ) ;
b = BigNum.new( -1234 ) ;
BigNum.sub( a , b , c ) ;
print( BigNum.mt.tostring( a ) .. " == 2686 " ) ;
print( BigNum.mt.tostring( b ) .. " == -1234 " ) ;
a = BigNum.new( 5251 ) ;
b = BigNum.new( 456 ) ;
BigNum.sub( a , b , c ) ;
print( BigNum.mt.tostring( a ) .. " == 5251 " ) ;
print( BigNum.mt.tostring( b ) .. " == 456 " ) ;

--MUL
a = BigNum.new( 9845 ) ;
b = BigNum.new( -124 ) ;
BigNum.mul( a , b , c ) ;
print( BigNum.mt.tostring( a ) .. " == 9845 " ) ;
print( BigNum.mt.tostring( b ) .. " == -124 " ) ;
a = BigNum.new( 852 ) ;
b = BigNum.new( 1473 ) ;
BigNum.mul( a , b , c ) ;
print( BigNum.mt.tostring( a ) .. " == 852 " ) ;
print( BigNum.mt.tostring( b ) .. " == 1473 " ) ;

--DIV
d = BigNum.new( ) ;
a = BigNum.new( 987456 ) ;
b = BigNum.new( -852 ) ;
BigNum.div( a , b , c , d ) ;
print( BigNum.mt.tostring( a ) .. " == 987456 " ) ;
print( BigNum.mt.tostring( b ) .. " == -852 " ) ;
a = BigNum.new( 4836 ) ;
b = BigNum.new( 9756 ) ;
BigNum.div( a , b , c , d ) ;
print( BigNum.mt.tostring( a ) .. " == 4836 " ) ;
print( BigNum.mt.tostring( b ) .. " == 9756 " ) ;

--GCD
a = BigNum.new( 2686 ) ;
b = BigNum.new( 1234 ) ;
c = BigNum.gcd( a , b ) ;
print( BigNum.mt.tostring( a ) .. " == 2686 " ) ;
print( BigNum.mt.tostring( b ) .. " == -1234 " ) ;
print( c ) ;
a = BigNum.new( 5251 ) ;
b = BigNum.new( 456 ) ;
c = BigNum.gcd( a , b ) ;
print( BigNum.mt.tostring( a ) .. " == 5251 " ) ;
print( BigNum.mt.tostring( b ) .. " == 456 " ) ;
print( c ) ;

--EXP
a = BigNum.new( 2686 ) ;
b = BigNum.new( 0 ) ;
c = BigNum.exp( a , b ) ;
print( BigNum.mt.tostring( a ) .. " == 2686 " ) ;
print( BigNum.mt.tostring( b ) .. " == 0 " ) ;
print( c ) ;
a = BigNum.new( -5251 ) ;
b = BigNum.new( 5 ) ;
c = BigNum.exp( a , b ) ;
print( BigNum.mt.tostring( a ) .. " == -5251 " ) ;
print( BigNum.mt.tostring( b ) .. " == 5 " ) ;
print( c ) ;
