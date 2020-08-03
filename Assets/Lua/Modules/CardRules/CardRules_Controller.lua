﻿---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              出牌规则 Controller业务逻辑
---
---
local LandlordEnum = require("Game.Logic.LandlordEnum")
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

---卡牌数组排序
---@param cards table<number,Card>
---@param ascending boolean
function public.SortCards(cards, ascending)
    table.sort(cards, function(a, b)
        if ascending then
            --按照权重升序
            return a:GetCardWeight() < b:GetCardWeight()
        else
            --先按照权重降序，再按花色升序
            if a:GetCardWeight() ~= b:GetCardWeight() then
                return a:GetCardWeight() > b:GetCardWeight()
            else
                return a:GetCardSuit() < b:GetCardSuit()
            end
        end
    end)
end

---是否是单
---@param cards table<number,Card>
---@return boolean
function public.IsSingle(cards)
    if 1 == #cards then
        return true
    else
        return false
    end
end

---是否是对
---@param cards table<number,Card>
---@return boolean
function public.IsDouble(cards)
    if 2 == #cards then
        if cards[1]:GetCardWeight() == cards[2]:GetCardWeight() then
            return true
        end
    end
    return false
end

---是否是顺子
---@param cards table<number,Card>
---@return boolean
function public.IsStraight(cards)
    local length = #cards
    if length < 5 or length > 12 then
        return false
    end
    for i = 1, length - 1 do
        local w = cards[i]:GetCardWeight()
        if 1 ~= cards[i + 1]:GetCardWeight() - w then
            return false
        end
        if w > LandlordEnum.Weight.One or cards[i + 1]:GetCardWeight() > LandlordEnum.Weight.One then
            return false
        end
    end
    return true
end

return public