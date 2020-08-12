---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 用于控制卡牌显示的模块
---

---用于控制卡牌显示的类
---@class CardSprite
local CardSprite = Class("CardSprite")
local CharacterType = LandlordEnum.CharacterType

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
    ---@type string
    self.ResPath = ""
end

---获得sprite所装载的card
---@return Card
function CardSprite:GetPoker()
    return self.Poker
end

---设置sprite所装载的card
---@param Poker Card
function CardSprite:SetPoker(Poker)
    self.Poker = Poker
    self.Poker:SetMakedSprite(true)
    self:SetSprite()
end

---是否被选中
---@return boolean
function CardSprite:IsSelect()
    return self.isSelected
end

---设置是否被选中
---@param isSelected boolean
function CardSprite:SetSelect(isSelected)
    self.isSelected = isSelected
end

---设置UISprite的显示
function CardSprite:SetSprite()
    local charType = self.Poker:GetAttribution()
    if charType == CharacterType.Player or charType == CharacterType.Desk then
        Util.UI.SetImageSpriteFromAtlas(self.Sprite, self.Poker:GetCardName())
    else
        Util.UI.SetImageSpriteFromAtlas(self.Sprite, "SmallCardBack1")
    end
end

---销毁
function CardSprite:Destroy()
    ---精灵化false
    self.Poker:SetMakedSprite(false)
    CommonUtil.DiscardGameObject(self.ResPath, self.GameObject)
end

---调整位置
---@param parent UnityEngine.GameObject
---@param index number
function CardSprite:GoToPosition(parent,index)
    
end

---卡牌点击
function CardSprite:OnClick()
    if self.Poker:GetAttribution() == CharacterType.Player then
        if self.isSelected then
        else

        end
    end
end

return CardSprite