---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 玩家牌控制模块
---

---玩家牌控制类
---@class PlayerCard
local PlayerCard = Class("PlayerCard")
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

    for _,v in ipairs(self.cardSprites) do
        if v.isSelected then
            table.insert(selectedSpriteList,v)
            table.insert(selectedCardsList,v:GetPoker())
        end
    end

    --排好序
    Ctrl.CardRules.SortCards(selectedCardsList,true)
end

return PlayerCard
