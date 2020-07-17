---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                数据结构栈 Lua实现
---

local LStack = {}
LStack.__index = LStack

function LStack:New()
    local t = {}
    setmetatable(t, LStack)
    t:init()
    return t

end

function LStack:init()
    self.stackList = {}
end

function LStack:clear()
    self:init()
end

function LStack:pop()
    if 0 == #self.stackList then
        error("Stack is Empty!")
        return nil
    end
    return table.remove(self.stackList)
end

function LStack:push(t)
    table.insert(self.stackList, t)
end

function LStack:count()
    return #self.stackList
end

function LStack:isEmpty()
    return 0 == #self.stackList
end

return LStack
