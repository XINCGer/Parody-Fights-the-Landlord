---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 玩家牌控制模块
---

---玩家牌控制类
---@class PlayerCard
local PlayerCard = Class("PlayerCard")
local CardType = LandlordEnum.CardsType
local HandCards = require("Game.Logic.HandCards")
local CardSprite = require("Game.Logic.CardSprite")

---构造器
function PlayerCard:initialize()
    ---玩家手中的CardSprite
    ---@type table<number,CardSprite>
    self.cardSprites = {}
    ---@type HandCards
    self.handCards = HandCards:new()
end

---遍历选中的牌和牌精灵
---@return boolean
function PlayerCard:CheckSelectCards()
    --找出所有选中的牌
    local selectedCardsList = {}
    local selectedSpriteList = {}

    for _, v in ipairs(self.cardSprites) do
        if v.isSelected then
            table.insert(selectedSpriteList, v)
            table.insert(selectedCardsList, v:GetPoker())
        end
    end

    --排好序
    Ctrl.CardRules.SortCards(selectedCardsList, true)
end

---检测玩家出牌
---@param selectedCardsList table<number,Card>
---@param selectedSpriteList table<number,CardSprite>
---@return boolean
function PlayerCard:CheckPlayCards(selectedCardsList, selectedSpriteList)
    --检测是否符合出牌规范
    local result, type = Ctrl.CardRules.PopEnable(selectedCardsList)
    if result then
        local rule = Mod.DeskCardsCache.GetRule()
        if Ctrl.Order.GetBiggestCharType() == Ctrl.Order.GetCurrCharType() then
            PlayerCard(selectedCardsList, selectedSpriteList, type)
        elseif Mod.DeskCardsCache.GetRule() == CardType.None then
            PlayerCard(selectedCardsList, selectedSpriteList, type)
        elseif type == CardType.Boom and rule ~= CardType.Boom then
            --炸弹
            Ctrl.World.SetMultiples(2)
            PlayerCard(selectedCardsList, selectedSpriteList, type)
        elseif type == CardType.JokerBoom then
            --王炸
            Ctrl.World.SetMultiples(4)
            PlayerCard(selectedCardsList, selectedSpriteList, type)
        elseif type == CardType.Boom and rule == CardType.Boom and Ctrl.World.GetWeight(selectedCardsList, type) > Mod.DeskCardsCache.GetTotalWeight() then
            Ctrl.World.SetMultiples(4)
            PlayerCard(selectedCardsList, selectedSpriteList, type)
        elseif Ctrl.World.GetWeight(selectedCardsList, type) > Mod.DeskCardsCache.GetTotalWeight() then
            PlayerCard(selectedCardsList, selectedSpriteList, type)
        end
    end
    return result
end

---玩家出牌
---@param selectedCardsList table<number,Card>
---@param selectedSpriteList table<number,CardSprite>
---@param type LandlordEnum.CardsType
function PlayerCard:PlayCard(selectedCardsList, selectedSpriteList, type)

end

return PlayerCard
