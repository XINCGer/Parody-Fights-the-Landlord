---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 数据配置管理器
---

local ConfigMgr = {}

---@field 数据配置文件的路径
local cfgPath = "LuaConfigs/%s.lua"
---@field 缓存表格数据
local _cacheConfig = {}
---@field 具有id的表的快速索引缓存，结构__fastIndexConfig["LanguageCfg"][100]
local _quickIndexConfig = {}

---获取对应的表格数据
---@param name string
function ConfigMgr.GetConfig(name)
	local tmpCfg = _cacheConfig[name]
	if nil ~= tmpCfg then
		return tmpCfg
	else
		local fileName = string.format(cfgPath, name)
		--print("----------->Read Config File"..fileName)
		-- 读取配置文件
		local cfgData = dofile(fileName)

		-- 对读取到的配置做缓存处理
		_cacheConfig[name] = {}
		_cacheConfig[name].items = cfgData
		return _cacheConfig[name]
	end
	return nil
end

---获取表格中指定的ID项
---@param name string
---@param id number
function ConfigMgr.GetItem(name, id)
	if nil == _quickIndexConfig[name] then
		local cfgData = ConfigMgr.GetConfig(name)
		if cfgData and cfgData.items and cfgData.items[1] then
			-- 如果是空表的话不做处理
			local _id = cfgData.items[1].id
			if _id then
				-- 数据填充
				_quickIndexConfig[name] = {}
				for _, v in ipairs(cfgData.items) do
					_quickIndexConfig[name][v.id] = v
				end
			else
				print(string.format("Config: %s don't contain id: %d!", name, id))
			end
		end
	end
	if _quickIndexConfig[name] then
		return _quickIndexConfig[name][id]
	end
	return nil
end

return ConfigMgr
