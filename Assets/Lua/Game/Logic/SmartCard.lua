---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 电脑AI出牌模块
---

---@class SmartCard
---电脑AI出牌类
local SmartCard = Class("SmartCard")
local CardType = LandlordEnum.CardsType
local HandCards = require("Game.Logic.HandCards")

--- 构造器
function SmartCard:initialize()
    --TODO:computerNotice = transform.Find("ComputerNotice").gameObject;
    --TODO:OrderController.Instance.smartCard += AutoDiscardCard;
    ---@type HandCards
    self.handCards = HandCards:new()
end

---自动出牌
---@param isNone boolean
function SmartCard:AutoDiscardCard(isNone)
    if Ctrl.Order.GetCurrCharType() == self.handCards.cType then
        self:DelayDiscardCard(isNone);
    end
end

---一手牌
---@return table<number,Card>
function SmartCard:GetFirstCard()
    return nil
end

---延时出牌
---@param isNone boolean
function SmartCard:DelayDiscardCard(isNone)
    local fuc = function()
        local rule = isNone and CardType.None or Mod.DeskCardsCache.GetRule()
        local deskWeight = Mod.DeskCardsCache.GetTotalWeight()

        --根据桌面牌的类型和权值大小出牌
        if rule == CardType.None then
            local discardCards = self:GetFirstCard()
            if #discardCards ~= 0 then
                self:RemoveCards(discardCards)
                self:DiscardCards(discardCards, self:GetSprite(discardCards))
            end
        end
    end
end

---移除手牌
---@param cards table<number,Card>
function SmartCard:RemoveCards(cards)
    local allCards = self.handCards:GetAllCards()

    for _, v in ipairs(cards) do
        for _, m in ipairs(allCards) do
            if v == m then
                self.handCards:PopCard(m)
                break
            end
        end
    end
end

---DiscardCards
---@param selectedCards table<number,Card>
---@param selectSprites table<number,CardSprite>
function SmartCard:DiscardCards(selectedCards, selectSprites)

end

---获取card对应的精灵
---@param cards table<number,Card>
function SmartCard:GetSprite(cards)
    local cType = self.handCards.cType
    --TODO:待完善
    ---@type table<number,CardSprite>
    local sprites = Ctrl.World.GetAllCardSpritesByCharType(cType)

    local selectedSprites = {}

    for _, v in ipairs(sprites) do
        for _, m in ipairs(cards) do
            if m == v:GetPoker() then
                table.insert(selectedSprites, v)
                break
            end
        end
    end
    return selectedSprites
end

---找到手牌中符合要求的是单牌
---@param allCards table<number,Card>
---@param weight number
---@param equal boolean
---@return table<number,Card>
function SmartCard:FindSingle(allCards, weight, equal)
    local ret = {}
    for _, v in ipairs(allCards) do
        if equal then
            if v:GetCardWeight() >= weight then
                table.insert(ret, v)
                break
            end
        else
            if v:GetCardWeight() > weight then
                table.insert(ret, v)
                break
            end
        end
    end
    return ret
end

return SmartCard