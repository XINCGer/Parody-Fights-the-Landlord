---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              Deck_Module Module业务逻辑
---
local LandlordEnum = require("Game.Logic.LandlordEnum")
--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

---牌库
local library = nil
---角色类型
local cType = nil

--- Module模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()

end

--- Module模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()

end

---初始化牌库
function public.InitDeck()
    library = {}
    cType = LandlordEnum.CharacterType.Library
end

---创建一副牌
function private.CreateDeck()

end

---获取牌库中牌的数量
function public.CardsCount()
    return #library
end

return public