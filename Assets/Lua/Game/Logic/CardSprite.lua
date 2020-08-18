---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 用于控制卡牌显示的模块
---

---用于控制卡牌显示的类
---@class CardSprite
local CardSprite = Class("CardSprite")
local CharacterType = LandlordEnum.CharacterType
local vector3 = UnityEngine.Vector3

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
    ---@type SorterTag
    self.sorterTag = self.GameObject:AddSingleComponent(typeof(SorterTag))
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
---@param rootPos UnityEngine.Vector3
---@param index number
function CardSprite:GoToPosition(rootPos, index)
    self.sorterTag:SetSorter(index)
    local charType = self.Poker:GetAttribution()
    if CharacterType.Player == charType then
        local pos = rootPos + vector3.right * 25 * index
        self.GameObject.transform.localPosition = pos
        if self.isSelected then
            pos = self.GameObject.transform.localPosition + vector3.up * 10
            self.GameObject.transform.localPosition = pos
        end
    elseif CharacterType.ComputerOne == charType or CharacterType.ComputerTwo == charType then
        local pos = rootPos - vector3.up * 25 * index
        self.GameObject.transform.localPosition = pos
    elseif CharacterType.Desk == charType then
        local pos = rootPos + vector3.right * 25 * index
        self.GameObject.transform.localPosition = pos
    end
end

---卡牌点击
function CardSprite:OnClick()
    if self.Poker:GetAttribution() == CharacterType.Player then
        local pos = self.GameObject.transform.localPosition
        if self.isSelected then
            pos = pos - vector3.up * 10
            self.GameObject.transform.localPosition = pos
            self.isSelected = false
        else
            pos = pos + vector3.up * 10
            self.GameObject.transform.localPosition = pos
            self.isSelected = true
        end
    end
end

return CardSprite