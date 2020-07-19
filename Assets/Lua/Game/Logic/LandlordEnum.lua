---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                斗地主各种枚举定义
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
    ---@field 草花
    Club = 1,
    ---@field 方片
    Diamond = 2,
    ---@field 红桃
    Heart = 3,
    ---@field 黑桃
    Spade = 4,
    ---@field 其他无花色
    None = 5,
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

---@class 身份类别
LandlordEnum.Identity = {
    ---@field 农民
    Farmer = 1,
    ---@field 地主
    Landlord = 2,
}

---@class 出牌类型
LandlordEnum.CardsType = {
    ---@field 未知类型
    None = 1,
    ---@field 王炸
    JockerBoom = 2,
    ---@field 炸弹
    Boom = 3,
    ---@field 三个X
    OnlyThree = 4,
    ---@field 三带一
    ThreeAndOne = 5,
    ---@field 三带二
    ThreeAndTwo = 6,
    ---@field 顺子:五张或更多的连续单牌
    Straight = 7,
    ---@field 双顺:三对或更多的连续对牌
    DoubleStraight = 8,
    ---@field 三顺:二个或更多的连续三张牌
    TripleStraight = 9,
    ---@field 对子
    Double = 10,
    ---@field 单个
    Single = 11,
}