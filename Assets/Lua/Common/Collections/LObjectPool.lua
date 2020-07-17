---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                Lua版本的ObjectPool
---

local LStack = require("Common.Collections.LStack")

local LObjectPool = {}
LObjectPool.__index = LObjectPool

function LObjectPool:New(CreateFunc, ReleaseFunc)
    local t = {}
    setmetatable(t, LObjectPool)
    t:init(CreateFunc, ReleaseFunc)
    return t
end

function LObjectPool:init(CreateFunc, ReleaseFunc)
    self.stack = LStack:New()
    if type(CreateFunc) == "function" then
        self.createFunc = CreateFunc
    end
    if type(ReleaseFunc) == "function" then
        self.releaseFunc = ReleaseFunc
    end
end

function LObjectPool:get()
    local obj = nil
    if self.stack:isEmpty() then
        if nil ~= self.createFunc then
            obj = self.createFunc()
        else
            error("LObjectPool Create Function is NULL!")
        end
    else
        obj = self.stack:pop()
    end
    return obj
end

function LObjectPool:release(obj)
    if nil == obj then
        error("LObject the object want to release is NULL!")
        return
    end
    if nil ~= self.releaseFunc then
        self.releaseFunc(obj)
    end
    self.stack:push(obj)
end

function LObjectPool:clear()
    if nil ~= self.stack then
        self.stack:clear()
    end
    self.createFunc = nil
    self.releaseFunc = nil
end

return LObjectPool