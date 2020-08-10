---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              出牌顺序权限管理 Controller业务逻辑
---

--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}
local CharacterType = LandlordEnum.CharacterType

---最大出牌者
---@type LandlordEnum.CharacterType
local biggest

---当前出牌者
---@type LandlordEnum.CharacterType
local currentAuthority = LandlordEnum.CharacterType.Desk

--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()

end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()

end

---获取当前出牌者的类型
---@return LandlordEnum.CharacterType
function public.GetCurrCharType()
    return currentAuthority
end

---获取最大出牌者的类型
---@return LandlordEnum.CharacterType
function public.GetBiggestCharType()
    return biggest
end

---设置最大出牌者的类型
---@param value LandlordEnum.CharacterType
function public.SetBiggestCharType(value)
    biggest = value
end

---初始化
---@param type LandlordEnum.CharacterType
function public.Init(type)
    currentAuthority = type
    biggest = type

    if CharacterType.Player == currentAuthority then
        --初始为玩家，玩家必须出牌
        --TODO:activeButton(false)
    else
        --电脑自动出牌
        --TODO:smartCard(true)
    end
end

---出牌轮转
function public.Turn()
    currentAuthority = currentAuthority +1
    if CharacterType.Desk  == currentAuthority then
        currentAuthority = CharacterType.Player
    end
    if CharacterType.ComputerOne == currentAuthority or CharacterType.ComputerTwo == currentAuthority then
        --TODO:smartCard(biggest == currentAuthority)
    elseif CharacterType.Player == currentAuthority then
        --TODO:activeButton(biggest ~= currentAuthority)
    end
end

function public.ResetButton()
    --TODO:activeButton = null
end

function public.ResetButton()
    --TODO:smartCard = null
end

return public