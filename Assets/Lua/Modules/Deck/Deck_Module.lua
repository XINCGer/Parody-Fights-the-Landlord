---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              Deck_Module Module业务逻辑
---

local LandlordEnum = require("Game.Logic.LandlordEnum")
local Card = require("Game.Data.Card")

--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

---常量，四种花色
local COLOR_SIZE = 4
---牌的数目，每种花色13张
local CARD_NUM = 13

---牌库
local library = nil
---角色类型
local cType = nil

--- Module模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()

end

--- Module模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()

end

---初始化牌库
function public.InitDeck()
    library = {}
    cType = LandlordEnum.CharacterType.Library
    private.CreateDeck()
end

---创建一副牌
function private.CreateDeck()
    for color = 1, COLOR_SIZE do
        for value = 1, CARD_NUM do
            local name = LandlordEnum.SuitName[color] or LandlordEnum.SuitName[LandlordEnum.Suits.None]
            local card = Card:new(name, value, color, cType)
            table.insert(library, card)
        end
    end

    --- 创建大小王
    local smallJoker = Card:new("SJoker", LandlordEnum.Weight.SJoker, LandlordEnum.Suits.None, cType)
    local largeJocer = Card:new("LJoker", LandlordEnum.Weight.LJoker, LandlordEnum.Suits.None, cType)

    table.insert(smallJoker)
    table.insert(largeJocer)
end

function public.Shuffle()
    if public.CardsCount() == 54 then
        local shuffleList = {}
        for _, v in ipairs(library) do
            table.insert(shuffleList, math.random(#shuffleList + 1), v)
        end
        library = {}
        for _, v in ipairs(shuffleList) do
            table.insert(library, v)
        end
    end
end

---获取牌库中牌的数量
function public.CardsCount()
    return #library
end

return public