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
            local discardCards = self:GetFirstCard()
            if #discardCards ~= 0 then
                self:RemoveCards(discardCards)
                self:DiscardCards(discardCards, self:GetSprite(discardCards))
            end
        end
    end
end

---移除手牌
---@param cards table<number,Card>
function SmartCard:RemoveCards(cards)
    local allCards = self.handCards:GetAllCards()

    for _, v in ipairs(cards) do
        for _, m in ipairs(allCards) do
            if v == m then
                self.handCards:PopCard(m)
                break
            end
        end
    end
end

---出牌动画
---@param selectedCards table<number,Card>
---@param selectSprites table<number,CardSprite>
function SmartCard:DiscardCards(selectedCards, selectSprites)
    --检测是否符合出牌规范
    local result, type = Ctrl.CardRules.PopEnable(selectedCards)
    if result then
        --如果符合，将牌从手牌移到出牌缓存区
        Mod.DeskCardsCache.Clear()
        Mod.DeskCardsCache.SetRule(type)
        for _,v in ipairs(selectSprites) do
            Mod.DeskCardsCache.AddCard(v:GetPoker())
            --TODO:获取Desk的Root节点
            v:SetParent(Ctrl.World.GetDeskRootTF())
        end

        Mod.DeskCardsCache.Sort()
        Ctrl.World.AdjustCardSpritsPosition(LandlordEnum.CharacterType.Desk)
        Ctrl.World.AdjustCardSpritsPosition(self.handCards.cType)
        Ctrl.World.UpdateLeftCardsCount(self.handCards.cType,self.handCards:CardCount())

        if self.handCards:CardCount() == 0 then
            --TODO:Dispatch Event GameOver!
        else
            Ctrl.Order.SetBiggestCharType(self.handCards.cType)
            Ctrl.Order.Turn()
        end
    end
end

---获取card对应的精灵
---@param cards table<number,Card>
function SmartCard:GetSprite(cards)
    local cType = self.handCards.cType
    --TODO:待完善
    ---@type table<number,CardSprite>
    local sprites = Ctrl.World.GetAllCardSpritesByCharType(cType)

    local selectedSprites = {}

    for _, v in ipairs(sprites) do
        for _, m in ipairs(cards) do
            if m == v:GetPoker() then
                table.insert(selectedSprites, v)
                break
            end
        end
    end
    return selectedSprites
end

---找到手牌中符合要求的是单牌
---@param allCards table<number,Card>
---@param weight LandlordEnum.Weight
---@param equal boolean
---@return table<number,Card>
function SmartCard:FindSingle(allCards, weight, equal)
    local ret = {}
    for _, v in ipairs(allCards) do
        if equal then
            if v:GetCardWeight() >= weight then
                table.insert(ret, v)
                break
            end
        else
            if v:GetCardWeight() > weight then
                table.insert(ret, v)
                break
            end
        end
    end
    return ret
end

---三带一
---@param allCards table<number,Card>
---@param weight LandlordEnum.Weight
---@param equal boolean
---@return table<number,Card>
function SmartCard:FindThreeAndOne(allCards, weight, equal)
    local threeCards = self:FindOnlyThree(allCards, weight, equal)
    if 0 ~= #threeCards then
        local leftCards = self:GetAllCards(threeCards)
        local one = self:FindSingle(leftCards, LandlordEnum.Weight.Three, true)
        for _, v in ipairs(one) do
            table.insert(threeCards, v)
        end
    end
    return threeCards
end

---三带二
---@param allCards table<number,Card>
---@param weight LandlordEnum.Weight
---@param equal boolean
function SmartCard:FindThreeAndTwo(allCards, weight, equal)
    local threeCards = self:FindOnlyThree(allCards, weight, equal)
    if #threeCards ~= 0 then
        local leftCards = self:GetAllCards(threeCards)
        local two = self:FindDouble(leftCards, LandlordEnum.Weight.Three, true)
        for _, v in ipairs(two) do
            table.insert(threeCards, v)
        end
    end
    return threeCards
end

---找到手中的牌是三张的
---@param allCards table<number,Card>
---@param weight LandlordEnum.Weight
---@param equal boolean
---@return table<number,Card>
function SmartCard:FindOnlyThree(allCards, weight, equal)
    local ret = {}
    local length = #allCards
    for i = 0, length do
        if i <= length - 2 then
            if allCards[i]:GetCardWeight() == allCards[i + 1]:GetCardWeight() and allCards[i]:GetCardWeight() == allCards[i + 2]:GetCardWeight() then
                local totalWeight = allCards[i]:GetCardWeight() + allCards[i + 1]:GetCardWeight() + allCards[i + 2]:GetCardWeight()
                if equal then
                    if totalWeight >= weight then
                        table.insert(ret, allCards[i])
                        table.insert(ret, allCards[i + 1])
                        table.insert(ret, allCards[i + 2])
                        break
                    end
                else
                    if totalWeight > weight then
                        table.insert(ret, allCards[i])
                        table.insert(ret, allCards[i + 1])
                        table.insert(ret, allCards[i + 2])
                        break
                    end
                end
            end
        end
    end
    return ret
end

---找到手牌中符合要求的是顺子的
---@param allCards table<number,Card>
---@param minWeight number
---@param length number
---@param equal boolean
function SmartCard:FindStraight(allCards, minWeight, length, equal)
    local ret = {}
    local counter = 1
    local idxList = nil
    local cardsLength = #allCards

    for i = 1, cardsLength do
        if i <= cardsLength - 4 then
            local weight = allCards[i]:GetCardWeight()
            if equal then
                if weight >= minWeight then
                    counter = 1
                    idxList = {}

                    for j = i + 1, #allCards do
                        if allCards[j]:GetCardWeight() > LandlordEnum.Weight.One then
                            break
                        end

                        if allCards[j]:GetCardWeight() - weight == counter then
                            counter = counter + 1
                            table.insert(idxList, j)
                        end

                        if counter == length then
                            break
                        end
                    end
                end
            else
                if weight > minWeight then
                    counter = 1
                    idxList = {}

                    for j = i + 1, cardsLength do
                        if allCards[j]:GetCardWeight() > LandlordEnum.Weight.One then
                            break
                        end
                        if allCards[j]:GetCardWeight() - weight == counter then
                            counter = counter + 1
                            table.insert(idxList, j)
                        end
                        if counter == length then
                            break
                        end
                    end
                end
            end
        end

        if counter == length then
            table.insert(idxList, 1, i)
        end
    end

    if counter == length then
        for _, v in ipairs(idxList) do
            table.insert(ret, allCards[v])
        end
    end
    return ret
end

---找到手牌中符合要求对或更多的连续对牌
---@param allCards table<number,Card>
---@param minWeight number
---@param length number
function SmartCard:FindDoubleStraight(allCards, minWeight, length)
    local ret = {}
    local counter = 0
    local indexList = nil
    local cardsLength = #allCards

    for i = 1, cardsLength do
        if i <= cardsLength - 4 then
            local weight = allCards[i]:GetCardWeight()
            if weight > minWeight then
                counter = 0
                indexList = {}

                local circle = 0
                for j = i + 1, cardsLength do
                    if allCards[j]:GetCardWeight() > LandlordEnum.Weight.One then
                        break
                    end
                    if allCards[j]:GetCardWeight() - weight == counter then
                        circle = circle + 1
                        if circle % 2 == 1 then
                            counter = counter + 1
                        end
                        table.insert(indexList, j)
                    end
                    if counter == length / 2 then
                        break
                    end
                end
            end
        end
        if counter == length / 2 then
            table.insert(indexList, 1, i)
        end
    end
    if counter == length / 2 then
        for i = 1, #indexList do
            table.insert(ret, allCards[indexList[i]])
        end
    end
    return ret
end

---获取所有的手牌
---@param exclude table<number,Card>
---@return table<number,Card>
function SmartCard:GetAllCards(exclude)
    local cards = {}
    local isContinue = false
    local allCards = self.handCards:GetAllCards()
    for _, v in allCards do
        isContinue = false
        if next(exclude) ~= nil then
            for _, m in ipairs(exclude) do
                if v == m then
                    isContinue = true
                    break
                end
            end
        end
        if false == isContinue then
            table.insert(cards, v)
        end
    end
    Ctrl.CardRules.SortCards(cards, false)
    return cards
end

---找到手牌中符合要求的是对子
---@param allCards table<number,Card>
---@param weight LandlordEnum.Weight
---@param equal boolean
function SmartCard:FindDouble(allCards, weight, equal)
    local ret = {}
    local length = #allCards
    for i = 1, length do
        if i < length - 1 then
            if allCards[i]:GetCardWeight() == allCards[i + 1]:GetCardWeight() then
                local totalWeight = allCards[i]:GetCardWeight() + allCards[i + 1]:GetCardWeight()
                if equal then
                    if totalWeight >= weight then
                        table.insert(allCards[i])
                        table.insert(allCards[i + 1])
                        break
                    end
                else
                    if totalWeight > weight then
                        table.insert(allCards[i])
                        table.insert(allCards[i + 1])
                    end
                end
            end
        end
    end
    return ret
end

---找到手牌中符合要求的炸弹
---@param allCards table<number,Card>
---@param weight LandlordEnum.Weight
---@param equal boolean
---@return table<number,Card>
function SmartCard:FindBoom(allCards, weight, equal)
    local ret = {}
    local length = #allCards
    for i = 1, length do
        if i <= length - 4 then
            --先找出普通炸弹
            if allCards[i]:GetCardWeight() == allCards[i + 1]:GetCardWeight() and allCards[i]:GetCardWeight() == allCards[i + 2]:GetCardWeight() and allCards[i]:GetCardWeight() == allCards[i + 3]:GetCardWeight() then
                local totalWeight = allCards[i]:GetCardWeight() * 4
                if equal then
                    if totalWeight >= weight then
                        table.insert(ret, allCards[i])
                        table.insert(ret, allCards[i + 1])
                        table.insert(ret, allCards[i + 2])
                        table.insert(ret, allCards[i + 3])
                        break
                    end
                else
                    if totalWeight > weight then
                        table.insert(ret, allCards[i])
                        table.insert(ret, allCards[i + 1])
                        table.insert(ret, allCards[i + 2])
                        table.insert(ret, allCards[i + 3])
                        break
                    end
                end
            end
        end
    end

    --找王炸
    if #ret == 0 then
        for i = 1, length do
            if i < length then
                if allCards[i]:GetCardWeight() == LandlordEnum.Weight.SJoker and allCards[i + 1]:GetCardWeight() == LandlordEnum.Weight.LJoker then
                    table.insert(ret, allCards[i])
                    table.insert(ret, allCards[i + 1])
                end
            end
        end
    end

    return ret
end

return SmartCard