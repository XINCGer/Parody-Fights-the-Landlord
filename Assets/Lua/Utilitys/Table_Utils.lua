---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                Lua的table工具类
---

local Table_Utils = Class("Table_Utils")

-- override 初始化各种数据
function Table_Utils.initialize()

end

-- 深拷贝一个table
function Table_Utils.DeepCopy(object)
    local SearchTable = {}

    local function Func(object)
        if type(object) ~= "table" then
            return object
        end
        local NewTable = {}
        SearchTable[object] = NewTable
        for k, v in pairs(object) do
            NewTable[Func(k)] = Func(v)
        end

        return setmetatable(NewTable, getmetatable(object))
    end
    return Func(object)
end

-- 清空一个Table
function Table_Utils.ClearTable(t)
    for _, v in pairs(t) do
        v = nil
    end
    t = nil
end

-- 判断table中是否包含某个值
function Table_Utils.Contains(list, value)
    local isContain = false
    if nil == list or nil == value then
        return false
    end
    if "table" ~= type(list) then
        return false
    end
    for _, v in ipairs(list) do
        if v == value then
            isContain = true
        end
    end
    return isContain
end

return Table_Utils