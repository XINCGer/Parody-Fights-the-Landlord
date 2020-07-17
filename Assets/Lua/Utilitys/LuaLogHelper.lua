---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 Lua端的Log助手
---

local rawprint = print
local rawerror = error
local logHelper = {}

local isLog = true  -- 是否打印日志
local tablePritDepth = 5 --table最深的打印层次
local logTraceTag = 3
local debugLogTag = 3
local warnLogTag = 2
local errorLogTag = 0

-- 普通日志
function logHelper.debug(...)
    if LogFunction then
        LogFunction(debugLogTag, debugLogTag <= logTraceTag, ...)
    end
end

-- 警告
function logHelper.warn(...)
    if LogFunction then
        LogFunction(warnLogTag, warnLogTag <= logTraceTag, ...)
    end
end

-- 错误
function logHelper.error(...)
    if LogFunction then
        LogFunction(errorLogTag, errorLogTag <= logTraceTag, ...)
    end
end

-- 初始化
function logHelper.initialize()

end

-- 函数注册到全局
rawset(_G, "print", logHelper.debug)
rawset(_G, "warn", logHelper.warn)
rawset(_G, "error", logHelper.error)
rawset(_G, "rawprint", rawprint)
rawset(_G, "rawerror", rawerror)

return logHelper