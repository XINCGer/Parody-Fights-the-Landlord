---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                斗地主模块的枚举定义
---

local LandlordEnum = {}
_G.LandlordEnum = LandlordEnum

---@class 角色类型
LandlordEnum.CharacterType = {
    Library = 1,
    Player = 2,
    ComputerOne = 3,
    ComputerTwo = 4,
    Desk = 5,
}

---@class 花色
LandlordEnum.Suits = {
    Club,
    Diamond,
    Heart,
    Spade,
    None,
}

---@class 卡牌权重
LandlordEnum.Weight = {
    Three = 1,
    Four = 2,
    Five = 3,
    Six = 4,
    Seven = 5,
    Eight = 6,
    Nine = 7,
    Ten = 8,
    Jack = 9,
    Queen = 10,
    King = 11,
    One = 12,
    Two = 13,
    sJocker = 14,
    LJocker = 15,
}