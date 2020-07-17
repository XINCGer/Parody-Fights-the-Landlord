---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              Login_Module Module业务逻辑
---

--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

--- Module模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()
    print("------------->Login Module 启动")
end

--- Module模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()
    print("-------------->Login Module 关闭")
end

--- 测试函数如无需要可以删除
function private.Test()

end

return public