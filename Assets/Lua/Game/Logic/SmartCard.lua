---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 电脑AI出牌模块
---

---@class SmartCard
---电脑AI出牌类
local SmartCard = Class("SmartCard")
local CardType = LandlordEnum.CardsType
local HandCards = require("Game.Logic.HandCards")

--- 构造器
function SmartCard:initialize()
    --TODO:computerNotice = transform.Find("ComputerNotice").gameObject;
    --TODO:OrderController.Instance.smartCard += AutoDiscardCard;
    ---@type HandCards
    self.handCards = HandCards:new()
end

---自动出牌
---@param isNone boolean
function SmartCard:AutoDiscardCard(isNone)
    if Ctrl.Order.GetCurrCharType() == self.handCards.cType then
        self:DelayDiscardCard(isNone);
    end
end

---一手牌
---@return table<number,Card>
function SmartCard:GetFirstCard()
    return nil
end

---延时出牌
---@param isNone boolean
function SmartCard:DelayDiscardCard(isNone)
    local fuc = function()
        local rule = isNone and CardType.None or Mod.DeskCardsCache.GetRule()
        local deskWeight = Mod.DeskCardsCache.GetTotalWeight()

        --根据桌面牌的类型和权值大小出牌
        if rule == CardType.None then
            
        end
    end
end

return SmartCard