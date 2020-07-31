---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              桌面扑克缓存区 Controller业务逻辑
---

local LandlordEnum = require("Game.Logic.LandlordEnum")
--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

---牌库
local library = {}
---角色类型
---@type LandlordEnum.CharacterType
local cType = nil
---出牌类型
---@type LandlordEnum.CardsType
local rule = nil

--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()

end

---初始化桌面扑克缓冲区
function public.InitDeskCardsCache()
    library = {}
    cType = LandlordEnum.CharacterType.Desk
    rule = LandlordEnum.CardsType.None
end

---发牌
function public.Deal()

end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()

end

--- 测试函数如无需要可以删除
function private.Test()

end

return public