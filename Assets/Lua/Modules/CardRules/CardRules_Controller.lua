---
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

---是否是双顺子
---@param cards table<number,Card>
---@return boolean
function public.IsDoubleStraight(cards)
    local length = #cards
    if length < 6 or length % 2 ~= 0 then
        return false
    end

    for i = 1, length, 2 do
        if cards[i + 1]:GetCardWeight() ~= cards[i]:GetCardWeight() then
            return false
        end
        if i <= length - 2 then
            if cards[i + 2]:GetCardWeight() - cards[i]:GetCardWeight() ~= 1 then
                return false
            end
            --不能超过A
            if cards[i]:GetCardWeight() > LandlordEnum.Weight.One or cards[i + 2]:GetCardWeight() > LandlordEnum.Weight.One then
                return false
            end
        end
    end
    return true
end

---是否飞机不带
---@param cards table<number,Card>
---@return boolean
function public.IsTripleStraight(cards)
    local length = #cards
    if length < 6 or length % 3 ~= 0 then
        return false
    end

    for i = 1, length, 3 do
        if cards[i + 1]:GetCardWeight() ~= cards[i]:GetCardWeight() then
            return false
        end
        if cards[i + 2]:GetCardWeight() ~= cards[i]:GetCardWeight() then
            return false
        end
        if cards[i + 1]:GetCardWeight() ~= cards[i + 2]:GetCardWeight() then
            return false
        end
        if i <= length - 3 then
            if cards[i + 3]:GetCardWeight() - cards[i]:GetCardWeight() ~= 1 then
                return false
            end

            --不能超过A
            if cards[i]:GetCardWeight() > LandlordEnum.Weight.One or cards[i + 3]:GetCardWeight() > LandlordEnum.Weight.One then
                return false
            end
        end
    end
    return true
end

---是否三不带
---@param cards table<number,Card>
---@return boolean
function public.IsOnlyThree(cards)
    if #cards % 3 ~= 0 then
        return false
    end
    if cards[1]:GetCardWeight() == cards[2]:GetCardWeight() and cards[2]:GetCardWeight() == cards[3]:GetCardWeight() then
        return true
    end
    return false
end

---是否三带一
---@param cards table<number,Card>
---@return boolean
function public.IsThreeAndOne(cards)
    if #cards ~= 4 then
        return false
    end
    if cards[1]:GetCardWeight() == cards[2]:GetCardWeight() and cards[2]:GetCardWeight() == cards[3]:GetCardWeight() then
        return true
    elseif cards[2]:GetCardWeight() == cards[3]:GetCardWeight() and cards[3]:GetCardWeight() == cards[4]:GetCardWeight() then
        return true
    end
    return false
end

---是否是三带二
---@param cards table<number,Card>
---@return boolean
function public.IsThreeAndTwo(cards)
    if #cards ~= 5 then
        return false
    end
    if cards[1]:GetCardWeight() == cards[2]:GetCardWeight() and cards[2]:GetCardWeight() == cards[3]:GetCardWeight() then
        if cards[4]:GetCardWeight() == cards[5]:GetCardWeight() then
            return true
        end
    elseif cards[3]:GetCardWeight() == cards[4]:GetCardWeight() and cards[4]:GetCardWeight() == cards[5]:GetCardWeight() then
        if cards[1]:GetCardWeight() == cards[2]:GetCardWeight() then
            return true
        end
    end
    return false
end

---是否是炸弹
---@param cards table<number,Card>
---@return boolean
function public.IsBoom(cards)
    if #cards ~= 4 then
        return false
    end
    local _card = cards[1]
    for i = 2, #cards do
        if cards[i] ~= _card then
            return false
        end
    end
    return true
end

---是否是王炸
---@param cards table<number,Card>
---@return boolean
function public.IsJokerBoom(cards)
    if #cards ~= 2 then
        return false
    end
    if cards[1]:GetCardWeight() == LandlordEnum.Weight.SJoker then
        if cards[2]:GetCardWeight() == LandlordEnum.Weight.LJoker then
            return true
        end
        return false
    elseif cards[1]:GetCardWeight() == LandlordEnum.Weight.LJoker then
        if cards[2]:GetCardWeight() == LandlordEnum.Weight.SJoker then
            return true
        end
        return false
    end
    return false
end

---判断是否符合出牌规则
---@param cards table<number,Card>
---@return boolean,LandlordEnum.CardsType
function public.PopEnable(cards)
    local type = LandlordEnum.CardsType.None
    local isRule = false
    local length = #cards

    if 1 == length then
        isRule = true
        type = LandlordEnum.CardsType.Single
    elseif 2 == length then
        if public.IsDouble(cards) then
            isRule = true
            type = LandlordEnum.CardsType.Double
        elseif public.IsJokerBoom(cards) then
            isRule = true
            type = LandlordEnum.CardsType.JokerBoom
        end
    elseif 3 == length then
        if public.IsOnlyThree(cards) then
            isRule = true
            type = LandlordEnum.CardsType.OnlyThree

        end
    elseif 4 == length then
        if public.IsBoom(cards) then
            isRule = true
            type = LandlordEnum.CardsType.Boom
        elseif public.IsThreeAndOne(cards) then
            isRule = true
            type = LandlordEnum.CardsType.ThreeAndOne
        end
    elseif 5 == length then
        if public.IsStraight(cards) then
            isRule = true
            type = LandlordEnum.CardsType.Straight
        elseif public.IsThreeAndTwo(cards) then
            isRule = true
            type = LandlordEnum.CardsType.ThreeAndTwo
        end
    elseif 6 == length then
        if public.IsStraight(cards) then
            isRule = true
            type = LandlordEnum.CardsType.Straight
        elseif public.IsTripleStraight(cards) then
            isRule = true
            type = LandlordEnum.CardsType.TripleStraight
        elseif public.IsDoubleStraight(cards) then
            isRule = true
            type = LandlordEnum.CardsType.DoubleStraight
        end
    elseif 7 == length then
        if public.IsStraight(cards) then
            isRule = true
            type = LandlordEnum.CardsType.Straight
        end
    elseif 8 == length then
        if public.IsStraight(cards) then
            isRule = true
            type = LandlordEnum.CardsType.Straight
        elseif public.IsDoubleStraight(cards) then
            isRule=true
            type = LandlordEnum.CardsType.DoubleStraight
        end
    end
end

return public