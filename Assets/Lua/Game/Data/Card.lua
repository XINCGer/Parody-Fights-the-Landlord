---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 XXXXX模块
---

---牌类
---@class Card
local Card = Class("Card")

---构造器
---@param name string
---@param weight LandlordEnum.Weight
---@param suits LandlordEnum.Suits
---@param belongTo LandlordEnum.CharacterType
function Card:initialize(name, weight, suits, belongTo)
    self.makedSprite = false
    self.name = name
    self.weight = weight
    self.suits = suits
    self.belongTo = belongTo
end

---返回牌名
---@return string
function Card:GetCardName()
    return self.name
end

---返回牌的权重
---@return LandlordEnum.Weight
function Card:GetCardWeight()
    return self.weight
end

---返回牌的花色
---@return LandlordEnum.Suits
function Card:GetCardSuit()
    return self.suits
end

---是否精灵化
---@return boolean
function Card:GetMakedSprite()
    return self.makedSprite
end

---设置是否精灵化
---@param makedSprite boolean
function Card:SetMakedSprite(makedSprite)
    self.makedSprite = makedSprite
end

---返回牌的归属
---@return LandlordEnum.CharacterType
function Card:GetAttribution()
    return self.belongTo
end

---设置牌的归属
---@param belongTo LandlordEnum.CharacterType
function Card:SetAttribution(belongTo)
    self.belongTo = belongTo
end


return Card