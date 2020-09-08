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
            self:PlayCards(selectedCardsList, selectedSpriteList, type)
        elseif Mod.DeskCardsCache.GetRule() == CardType.None then
            self:PlayCards(selectedCardsList, selectedSpriteList, type)
        elseif type == CardType.Boom and rule ~= CardType.Boom then
            --炸弹
            Ctrl.World.SetMultiples(2)
            self:PlayCards(selectedCardsList, selectedSpriteList, type)
        elseif type == CardType.JokerBoom then
            --王炸
            Ctrl.World.SetMultiples(4)
            self:PlayCards(selectedCardsList, selectedSpriteList, type)
        elseif type == CardType.Boom and rule == CardType.Boom and Ctrl.World.GetWeight(selectedCardsList, type) > Mod.DeskCardsCache.GetTotalWeight() then
            Ctrl.World.SetMultiples(4)
            self:PlayCards(selectedCardsList, selectedSpriteList, type)
        elseif Ctrl.World.GetWeight(selectedCardsList, type) > Mod.DeskCardsCache.GetTotalWeight() then
            self:PlayCards(selectedCardsList, selectedSpriteList, type)
        end
    end
    return result
end

---玩家出牌
---@param selectedCardsList table<number,Card>
---@param selectedSpriteList table<number,CardSprite>
---@param type LandlordEnum.CardsType
function PlayerCard:PlayCards(selectedCardsList, selectedSpriteList, type)
    --如果符合将牌从手牌移到出牌缓存区
    Mod.DeskCardsCache.Clear()
    Mod.DeskCardsCache.SetRule(type)

    for _,v in ipairs(selectedSpriteList) do
        self.handCards:PopCard(v:GetPoker())
        Mod.DeskCardsCache.AddCard(v:GetPoker())
        --todo:Ctrl.World.GetDeskRootTF()
        v:SetParent(Ctrl.World.GetDeskRootTF())
    end

    Mod.DeskCardsCache.Sort()
    Ctrl.World.AdjustCardSpritsPosition(LandlordEnum.CharacterType.Desk)
    Ctrl.World.AdjustCardSpritsPosition(LandlordEnum.CharacterType.Player)
    Ctrl.World.UpdateLeftCardsCount(LandlordEnum.CharacterType.Player,self.handCards:CardCount())

    if self.handCards:CardCount() == 0 then
        --TODO:Dispatch Event GameOver!
    else
        Ctrl.Order.SetBiggestCharType(LandlordEnum.CharacterType.Player)
        Ctrl.Order.Turn()
    end
end

return PlayerCard
