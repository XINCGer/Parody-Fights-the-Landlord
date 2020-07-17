--[[Notice:This lua config file is auto generate by Xls2Lua Tools，don't modify it manually! --]]
local fieldIdx = {}
fieldIdx.id = 1
fieldIdx.text = 2
local data = {
{1,[[零]]},
{2,[[一]]},
{3,[[二]]},
{4,[[三]]},
{5,[[四]]},
{6,[[五]]},
{7,[[六]]},
{8,[[七]]},
{9,[[八]]},
{10,[[九]]},
{10000,[[测试文字1]]},
{10001,[[测试文字2]]},}
local mt = {}
mt.__index = function(a,b)
	if fieldIdx[b] then
		return a[fieldIdx[b]]
	end
	return nil
end
mt.__newindex = function(t,k,v)
	error('do not edit config')
end
mt.__metatable = false
for _,v in ipairs(data) do
	setmetatable(v,mt)
end
return data