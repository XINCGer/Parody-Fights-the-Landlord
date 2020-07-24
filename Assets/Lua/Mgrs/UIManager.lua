---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                UIManager UI管理器
---

local GUICollections = require("Game.Main.GUICollections")
local UISorter = require("Mgrs.UISorter")
local UIManager = {}

---@field 存储打开的UI列表
local uiList = {}
---@field 用于存储参与点击其他地方关闭面板管理的UI的列表
local outTouchList = {}
---@field 存储要进行统一关闭面板的列表
local removeList = {}
---@field 存储统一隐藏恢复显示的UI列表
local recordList = {}
---@field UI层级排序Handler
local uiSorter = nil

---初始化
function UIManager.initialize()
    uiSorter = UISorter.Create(1, 3800)
    -- 注册事件
    UIManager.RegisterEvent()
end

---注册事件
function UIManager.RegisterEvent()
    -- 创建界面
    EventMgr.RegisterEvent(Modules.moduleId.Common, Modules.notifyId.Common.CREATE_PANEL, UIManager.Open)
    -- 销毁界面
    EventMgr.RegisterEvent(Modules.moduleId.Common, Modules.notifyId.Common.DESTROY_PANEL, UIManager.Close)
end

---设置一个UI界面参与点击其他地方关闭面板管理
---@param ui table
function UIManager.SetOutTouchDisappear(ui)
    -- 把UI加到outlist里面
    local isContain = false
    for _, v in ipairs(outTouchList) do
        if v == ui then
            isContain = true
        end
    end
    if not isContain then
        table.insert(outTouchList, 1, ui)
    end
end

---分发处理点击其他地方关闭面板
---@param panelName string
function UIManager.NotifyDisappear(panelName)
    removeList = {}
    for k, v in ipairs(outTouchList) do
        if nil ~= v and v.PanelName ~= panelName then
            v:Destroy()
            removeList[k] = true
            break  --每次只关闭一个界面
        end
    end
    -- 从outTouch列表中移除已经关闭的UI界面
    for i = #outTouchList, 1, -1 do
        if removeList[i] then
            table.remove(outTouchList, i)
        end
    end
end

---打开一个UI
---@param UIEnum ECEnumType.UIEnum
function UIManager.Open(UIEnum)
    if GUICollections and GUICollections[UIEnum] and GUICollections[UIEnum].Instance():IsExist() == false then
        GUICollections[UIEnum].Instance():Create()
        table.insert(uiList, GUICollections[UIEnum].Instance())
    else
        error("要打开的界面未在GUICollections注册或者已经打开！", UIEnum)
    end
end

---通过传入UI枚举关闭一个UI
---@param UIEnum number
function UIManager.Close(UIEnum)
    if GUICollections and GUICollections[UIEnum] then
        GUICollections[UIEnum].Instance():Destroy()

        --移除uiList中的UI实例
        local rmIndex = -1
        for i = #uiList, 1, -1 do
            if uiList[i] == GUICollections[UIEnum].Instance() then
                rmIndex = i
                break
            end
        end
        if -1 ~= rmIndex then
            table.remove(uiList, rmIndex)
        end
    end
end

function UIManager.CloseUISelf(ui)
    if ui then
        ui.Instance():Destroy()
    end
    --移除uiList中的UI实例
    local rmIndex = -1
    for i = #uiList, 1, -1 do
        if uiList[i] == ui.Instance() then
            rmIndex = i
            break
        end
    end
    if -1 ~= rmIndex then
        table.remove(uiList, rmIndex)
    end
end

---根据UI枚举获得UI界面实例
---@param UIEnum number
function UIManager.GetViewByType(UIEnum)
    if GUICollections and GUICollections[UIEnum] then
        return GUICollections[UIEnum].Instance()
    end
    return nil
end

---判断一个UI是否存在于界面上
---@param UIEnum number
function UIManager.IsExist(UIEnum)
    if GUICollections and GUICollections[UIEnum] then
        return GUICollections[UIEnum].Instance():IsExist()
    end
    return false
    -- 不用这种遍历的判断方式了
    --if uiList then
    --    for _, v in ipairs(uiList) do
    --        if GUICollections[UIEnum].Instance() == v then
    --            return true
    --        end
    --    end
    --end
    --return false
end

---恢复显示之前记录下来的隐藏UI
function UIManager.PopAndShowAllUI()
    if recordList and next(recordList) ~= nil then
        for _, v in ipairs(recordList) do
            v:SetVisible(true)
        end
    end
    recordList = {}
    EventMgr.DispatchEvent(Modules.moduleId.Common, Modules.notifyId.Common.ALLUI_SHOWSTATE_CHANGED, { state = true })
end

---记录并隐藏除了指定类型的当前显示的所有UI
---@param UIEnum number
---@param extUI table
function UIManager.StashAndHideAllUI(UIEnum, extUI)
    recordList = {}
    for _, v in ipairs(uiList) do
        if v and v:IsVisible() and v.PanelName ~= extUI.PanelName then
            table.insert(recordList, v)
            v:SetVisible(false)
        end
    end
    EventMgr.DispatchEvent(Modules.moduleId.Common, Modules.notifyId.Common.ALLUI_SHOWSTATE_CHANGED, { state = false })
end

---统一关闭属于某一UI层
---@param level number
function UIManager.CloseUIByLevel(level)
    if uiList then
        -- 倒序关闭
        for i = #uiList, 1, -1 do
            uiList[i]:Destroy()
        end
    end
    uiList = {}
end

---获取最近一次打开的面板
function UIManager.GetLastOpenPanel()
    if uiList then
        return uiList[#uiList]
    end
    return nil
end

---显示UI背景模糊
---@param ui table
function UIManager.ShowUIBlur(ui)
    CommonUtil.ShowUIBlur(ui.Panel, ui.PanelName)
end

---显示UI背景遮罩
---@param ui table
function UIManager.ShowUIMask(ui)
    CommonUtil.ShowUIMask(ui.Panel, ui.PanelName)
end

---获取UI排序管理器
function UIManager.GetUISorterMgr()
    return uiSorter
end

return UIManager