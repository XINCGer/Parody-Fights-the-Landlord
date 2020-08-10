---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              桌面扑克缓存区 Controller业务逻辑
---

--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()

end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()

end


---清空桌面
function private.Clear()
    --TODO:待完善
    local cardSprites = GameObject.Find("Desk").GetComponentsInChildren("CardSprite");
    for i = 1, cardSprites.Length do
        cardSprites[i].transform.parent = nil;
        cardSprites[i].Destroy();
    end
end

return public