--%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
--
--  File Name:              test_div.lua
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
--    Tests the division of Big Numbers - (BigNum) library.
--
--$.%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

require("BigNum.lua") ;

RADIX = 10 ;
RADIX_LEN = 1 ;

--Samples
print( "Some division samples:" ) ;
--Create return variables
c = BigNum.new( ) ;
d = BigNum.new( ) ;
a = BigNum.new( "68452" ) ;
b = BigNum.new( "21" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " & " .. BigNum.mt.tostring(d)) ;
a = BigNum.new( "123000" ) ;
b = BigNum.new( "4214" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " & " .. BigNum.mt.tostring(d)) ;
a = BigNum.new( "10000" ) ;
b = BigNum.new( "58" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " & " .. BigNum.mt.tostring(d)) ;
a = BigNum.new( "352165" ) ;
b = BigNum.new( "464" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " & " .. BigNum.mt.tostring(d)) ;
a = BigNum.new( "35026" ) ;
b = BigNum.new( "30461" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " & " .. BigNum.mt.tostring(d)) ;
a = BigNum.new( "1494" ) ;
b = BigNum.new( "83" ) ;
BigNum.div( a , b , c , d ) ;
print(BigNum.mt.tostring(a) .. " / " .. BigNum.mt.tostring(b) .. " = " .. BigNum.mt.tostring(c) .. " & " .. BigNum.mt.tostring(d)) ;

--Speed test
RADIX_LEN = 7
RADIX = 10000000
a = BigNum.new( "12345678901234567890123456789012345678901234567890" ) ;
b = BigNum.new( "12345678901234567890") ;
print( "\nTesting dividing a 50 digits number by a 20 digits number" ) ;
st = os.clock() ;
for i = 0 , 1000 do
   c = a / b ;
end
st = os.clock() - st ;
print( "Elapsed time: " , st ) ;
print( "Estimated time per division: ", st/1000 ) ;
print( "End of test file" ) ;
