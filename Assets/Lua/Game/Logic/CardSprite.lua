---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 用于控制卡牌显示的模块
---

---用于控制卡牌显示的类
---@class CardSprite
local CardSprite = Class("CardSprite")

---构造器
function CardSprite:initialize()
    ---@type UnityEngine.GameObject
    self.GameObject = nil
    ---@type Card
    self.Poker = nil
    ---@type UnityEngine.Sprite
    self.Sprite = nil
    ---@type boolean
    self.isSelected = false
end

---获得sprite所装载的card
---@return Card
function CardSprite:GetPoker()
    return self.Poker
end

return CardSprite