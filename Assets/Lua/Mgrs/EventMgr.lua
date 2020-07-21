---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                  事件管理中心
---

local EventMgr = {}
local bit = require "bit"

---@field 观察者列表
local _listeners = {}

---注册事件
---@param moduleId table
---@param eventId table
---@param func table
---@param 实例 table
function EventMgr.RegisterEvent(moduleId, eventId, func, inst)
    local key = bit.lshift(moduleId, 16) + eventId
    EventMgr.AddEventListener(key, func, inst, nil)
end

function EventMgr.UnRegisterEvent(moduleId, eventId, func)
    local key = bit.lshift(moduleId, 16) + eventId
    EventMgr.RemoveEventListener(key, func)
end

function EventMgr.DispatchEvent(moduleId, eventId, param)
    local key = bit.lshift(moduleId, 16) + eventId
    local listeners = _listeners[key]
    if nil == listeners then
        return
    end
    for _, v in ipairs(listeners) do
        if v.p then
            if v.instance then
                v.f(v.instance, param, v.p)
            else
                v.f(param, v.p)
            end
        else
            if v.instance then
                v.f(v.instance, param)
            else
                v.f(param)
            end
        end
    end
end

function EventMgr.AddEventListener(eventId, func, inst, param)
    local listeners = _listeners[eventId]
    -- 获取key对应的监听者列表，结构为{func,para}，如果没有就新建
    if listeners == nil then
        listeners = {}
        _listeners[eventId] = listeners -- 保存监听者
    end
    --过滤掉已经注册过的消息，防止重复注册
    for _, v in pairs(listeners) do
        if (v and v.f == func) then
            return
        end
    end
    --if func == nil then
    --    print("func is nil!")
    --end
    --加入监听者的回调和参数
    table.insert(listeners, { f = func, instance = inst, p = param })
end

function EventMgr.RemoveEventListener(eventId, func)
    local listeners = _listeners[eventId]
    if nil == listeners then
        return
    end
    for k, v in pairs(listeners) do
        if (v and v.f == func) then
            table.remove(listeners, k)
            return
        end
    end
end

return EventMgr