--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
--
--  File Name:              test_sub.lua
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
--    Tests the subtraction of Big Numbers - (BigNum) library.
--
--$.%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

require("BigNum.lua") ;

RADIX = 10 ;
RADIX_LEN = 1 ;

--Samples
print( "Some subtraction samples:" ) ;
--Create return variables
c = BigNum.new( ) ;
a = BigNum.new( "95785" ) ;
b = BigNum.new( "5286" ) ;
BigNum.sub( a , b , c ) ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
--c = BigNum.new( ) ;
a = BigNum.new( "5136452" ) ;
b = BigNum.new( "8587956" ) ;
BigNum.sub( a , b , c ) ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
--c = BigNum.new( ) ;
a = BigNum.new( "64854" ) ;
b = BigNum.new( "79785" ) ;
BigNum.sub( a , b , c ) ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
--c = BigNum.new( ) ;
a = BigNum.new( "352165" ) ;
b = BigNum.new( "464" ) ;
BigNum.sub( a , b , c ) ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
--c = BigNum.new( ) ;
a = BigNum.new( "35026" ) ;
b = BigNum.new( "-30461" ) ;
BigNum.sub( a , b , c ) ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;
--c = BigNum.new( ) ;
a = BigNum.new( "-1494" ) ;
b = BigNum.new( "83868" ) ;
BigNum.sub( a , b , c ) ;
print(BigNum.mt.tostring(a) .. " - " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c)) ;

--Speed test
RADIX_LEN = 7
RADIX = 10000000
a = BigNum.new( "12345678901234567890123456789012345678901234567890" ) ;
b = BigNum.new( "12345678901234567890123456789012345678901234567890") ;
print( "\nTesting subtracting two 50 digits numbers" ) ;
st = os.clock() ;
for i = 0 , 10000 do
   c = a + b ;
end
st = os.clock() - st ;
print( "Elapsed time: " , st ) ;
print( "Estimated time per subtaction: ", st/1000 ) ;
print( "End of test file" ) ;
