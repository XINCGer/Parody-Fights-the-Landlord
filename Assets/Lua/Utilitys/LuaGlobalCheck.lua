---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---               全局变量的一些检查与控制
---

_G.PCALL_ERROR_FUNCTION = function(message)
    error(message)
end

setmetatable(_G, {
    -- 控制新建全局变量
    __newindex = function(_, k)
        error("attempt to add a new value to global,key: " .. k, 2)
    end,

    -- 控制访问全局变量
    __index = function(_, k)
        error("attempt to index a global value,key: "..k,2)
    end
})