---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              桌面扑克缓存区 Module业务逻辑
---

local LandlordEnum = require("Game.Logic.LandlordEnum")
--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

---牌库
---@type table<number,Card>
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
---@return Card
function public.Deal()
    local ret = library[#library]
    table.remove(library, #library)
    return ret
end

---向牌库中添加牌
---@param card Card
function public.AddCard(card)
    card:SetAttribution(cType)
    table.insert(library, card)
end

---清空桌面
function public.Clear()
    if #library > 0 then
        Ctrl.DeskCardsCache.Clear()
        while nil ~= next(library) do
            local card = library[#library]
            table.remove(library, #library)
            Mod.Desk.AddCard(card)
        end
        rule = LandlordEnum.CardsType.None
    end
end

---手牌排序
function public.Sort()
    Ctrl.CardRules.SortCards(library, true)
end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()

end

--- 获得出牌类型
---@return LandlordEnum.CardsType
function public.GetRule()
    return rule
end

---设置出牌类型
---@param newRule LandlordEnum.CardsType
function public.SetRule(newRule)
    rule = newRule
end

---获得牌库中牌的数量
---@return number
function public.CardsCount()
    return #library
end

---最小权值
---@return LandlordEnum.Weight
function public.GetMinWeight()
    return library[0]:GetCardWeight()
end

---总权值
function public.GetTotalWeight()
    ---TODO:return total weight
end

return public