---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 数据配置管理器
---

local ConfigMgr = Class("ConfigMgr")

-- 数据配置文件的路径
local cfgPath = "LuaConfigs/%s.lua"


--实例对象
ConfigMgr._instance = nil

-- 获取单例
function ConfigMgr.Instance()
	if ConfigMgr._instance == nil then
		ConfigMgr._instance = ConfigMgr:new()
	end
	return ConfigMgr._instance
end

-- override 初始化各种数据
function ConfigMgr:initialize()
	--缓存表格数据
	self._cacheConfig = {}
	--具有id的表的快速索引缓存，结构__fastIndexConfig["LanguageCfg"][100]
	self._quickIndexConfig = {}
end

-- 获取对应的表格数据
function ConfigMgr:GetConfig(name)
	local tmpCfg = self._cacheConfig[name]
	if nil ~= tmpCfg then
		return tmpCfg
	else 
		local fileName = string.format(cfgPath,name)
		--print("----------->Read Config File"..fileName)
		-- 读取配置文件
		local cfgData = dofile(fileName)
		
		-- 对读取到的配置做缓存处理
		self._cacheConfig[name] = {}
		self._cacheConfig[name].items = cfgData
		return self._cacheConfig[name]
	end
	return nil
end

-- 获取表格中指定的ID项
function ConfigMgr:GetItem(name,id)
	if nil == self._quickIndexConfig[name] then
		local cfgData = self:GetConfig(name)
		if cfgData and cfgData.items and cfgData.items[1] then
			-- 如果是空表的话不做处理
			local _id = cfgData.items[1].id
			if _id then
				-- 数据填充
				self._quickIndexConfig[name] = {}
				for _,v in ipairs(cfgData.items) do 
					self._quickIndexConfig[name][v.id]= v
				end
			else
				print(string.format("Config: %s don't contain id: %d!",name,id))
			end
		end
	end
	if self._quickIndexConfig[name] then
		return self._quickIndexConfig[name][id]
	end
	return nil
end

return ConfigMgr
