---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 手牌模块
---

---手牌类
---@class HandCards
local HandCards = Class("HandCards")
local CharacterType = LandlordEnum.CharacterType

---构造器
function HandCards:initialize()
    ---@type LandlordEnum.CharacterType
    self.cType = nil
    ---@type table<number,Card>
    self.library = {}
    ---@type LandlordEnum.Identity
    self.identity = LandlordEnum.Identity.Farmer
    ---玩家倍数
    ---@type number
    self.multiples = 1
    ---积分
    ---@type number
    self.integration =0
end

---获得玩家倍数
---@return number
function HandCards:GetMultiples()
    return self.multiples
end

---设置玩家倍数
---@param value number
function HandCards:SetMultiples(value)
    self.multiples = self.multiples * value
end

---手牌数
function HandCards:CardCount()
    return #self.library
end

---获取手牌
---@param index number
---@return Card
function HandCards:GetCard(index)
    return self.library[index]
end

---获取牌的索引值
---@param card Card
---@return number
function HandCards:GetCardIndex(card)
    for k,v in ipairs(self.library) do
        if v == card then
            return k
        end
    end
    return -1
end

---添加手牌
---@param card Card
function HandCards:AddCard(card)
    card:SetAttribution(self.cType)
    table.insert(self.library,card)
end

---出牌
---@param card Card
function HandCards:PopCard(card)
    local index = self:GetCardIndex(card)
    if -1~= index then
        table.remove(self.library,index)
    else
        error("要移除的Card没有在牌库中找到！",card)
    end
end

---手牌排序
function HandCards:Sort()
    Ctrl.CardRules.SortCards(self.library,false)
end

return HandCards
