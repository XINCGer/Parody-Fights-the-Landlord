---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---            UISorter UI界面层级排序
---

local UISorter = Class("UISorter")

function UISorter:initialize()
    self.minSortIndex = 0
    self.maxSortIndex = 0
    self.is3DHigher = true
    self.uiDic = {} --key:UI名字,value:UIInfo
    self.uiSortList = {}
    self.canvasSortList = {}
end

function UISorter.Create(minSortIndex, maxSortIndex)
    local UISorter = UISorter:new()
    UISorter.minSortIndex = minSortIndex
    UISorter.maxSortIndex = maxSortIndex
    return UISorter
end

-- 对某个Gameobject下的Canvas进行动态排序功能,识别子canvas，根据sortingOrder对canvas的排序
function UISorter:SortIndexSetter(uiPanel, sortIndex)
    if nil == uiPanel or nil == uiPanel.Panel then
        return 0
    end

    self.canvasSortList = {}
    local allCanvas = uiPanel.Panel:GetComponentsInChildren("Canvas", true):ToTable()
    for i = 1, #allCanvas do
        if allCanvas[i] then
            table.insert(self.canvasSortList, allCanvas[i])
        end
    end
    table.sort(self.canvasSortList, function(a, b)
        return a.sortingOrder < b.sortingOrder
    end)

    for i = 1, #self.canvasSortList do
        self.canvasSortList[i].sortingOrder = sortIndex
        --- Canvas 层级 +2 间隔一个空层级，某些组件比如UGUI Dropdown组件关闭按钮为dropdown层级减一，无间隔会与其他层级冲突导致关闭功能异常
        sortIndex = sortIndex + 2
    end
    return sortIndex
end

-- 设置UI的SortTag,根据显示修改上下关系做到排序
function UISorter:SortTagIndexSetter(uiPanel, sortIndex)
    if nil == uiPanel or nil == uiPanel.Panel then
        return 0
    end
    if not uiPanel.sorterTag then
        return sortIndex
    end
    uiPanel.sorterTag:SetSorter(sortIndex + 1)
    sortIndex = uiPanel.sorterTag:GetSorter()
    return sortIndex
end

-- 设置带有3D模型UI的SortTag，带3d模型的ui需要排序设置
function UISorter:SortTag3DSetter(uiPanel, pos3D, isHigher)
    if uiPanel == nil or nil == uiPanel.Panel then
        return 0
    end
    if not uiPanel:IsVisible() then
        return pos3D
    end

    if not uiPanel.sorterTag then
        return pos3D
    end
    local space3d = uiPanel.sorterTag.Space3D
    if isHigher then
        if space3d > 0 or pos3D ~= 0 then
            pos3D = pos3D - space3d
            uiPanel.sorterTag:SetSpace3D(pos3D)
        else
            uiPanel.sorterTag:SetSpace3D(pos3D)
            pos3D = pos3D + space3d
        end
    end
    return pos3D
end

-- 添加打开面板时调用，会重排UI
function UISorter:AddPanel(uiPanel)
    if nil == uiPanel then
        error("invalid param #2 to AddPanel, can not be nil")
        return
    end
    if self.uiDic[uiPanel.PanelName] then
        error("AddPanel failed, the panel is already added: " .. uiPanel.PanelName)
        return
    end
    local panelInfo = { panelName = uiPanel.PanelName, uiPanel = uiPanel, depthLayer = uiPanel.uiDepthLayer, index = 0, moveTop = 1 }
    self.uiDic[uiPanel.PanelName] = panelInfo
    table.insert(self.uiSortList, panelInfo)

    -- 重排UI界面
    self:ResortPanels()
end

--  移除关闭面板时调用，会重排UI
function UISorter:RemovePanel(uiPanel)
    local panelInfo = self.uiDic[uiPanel.PanelName]
    if not panelInfo then
        warn("Failed to RemovePanel: the panel not found: " .. uiPanel.PanelName)
        return
    end
    self.uiDic[uiPanel.PanelName] = nil

    -- 重排UI界面
    self:ResortPanels()
end

-- 将指定的UI提升到当前UILEVEL的最上层
function UISorter:MovePanelToTop(uiPanel)
    local panelInfo = self.uiDic[uiPanel.PanelName]
    if not panelInfo then
        warn("Failed to MovePanelToTop: the panel not found: " .. uiPanel.PanelName)
        return
    end
    panelInfo.moveTop = 1
    self:ResortPanels()
end

-- 将指定的UI提升到指定UILEVEL的最上层
function UISorter:MovePanelToTopOfLayer(uiPanel, depthLayer)
    local panelInfo = self.uiDic[uiPanel.PanelName]
    if not panelInfo then
        warn("Failed to MovePanelToTop: the panel not found: " .. uiPanel.PanelName)
        return
    end
    panelInfo.moveTop = 1
    panelInfo.depthLayer = depthLayer
    self:ResortPanels()
end

-- 重排UI界面
-- 根据UI的打开先后顺序先赋值index，然后根据uiDepthLayer\moveTop\index三者权重进行UI重排
function UISorter:ResortPanels()
    for i = 1, #self.uiSortList do
        self.uiSortList[i].index = i
    end

    -- 对UI进行排序
    table.sort(self.uiSortList, function(a, b)
        local leftRemoved = self.uiDic[a.panelName] == nil
        local rightRemoved = self.uiDic[b.panelName] == nil
        -- 判空处理
        if leftRemoved ~= rightRemoved then
            return not leftRemoved
        end

        if a.depthLayer ~= b.depthLayer then
            return a.depthLayer < b.depthLayer
        end

        if a.moveTop ~= b.moveTop then
            return a.moveTop < b.moveTop
        end
        return a.index < b.index
    end)

    -- 从uiSortList中移除已经关闭的UI
    -- 倒序删除
    for i = #self.uiSortList, 1, -1 do
        local panelName = self.uiSortList[i].panelName
        if not self.uiDic[panelName] then
            table.remove(self.uiSortList)
        else
            break
        end
    end

    local _index = self.minSortIndex
    local _sortIndex = -1
    local space3d = 0

    for k, v in ipairs(self.uiSortList) do
        v.moveTop = 0  -- 重置moveTop标志位

        if _index > self.maxSortIndex then
            _index = self.maxSortIndex
        end
        _index = self:SortIndexSetter(v.uiPanel, _index)
        _sortIndex = self:SortTagIndexSetter(v.uiPanel, _sortIndex)
        if self.is3DHigher then
            space3d = self:SortTag3DSetter(v.uiPanel, space3d, true)
        end
    end

    if not self.is3DHigher then
        for i = #self.uiSortList, 1, -1 do
            space3d = self:SortTag3DSetter(self.uiSortList[i].uiPanel, space3d, false)
        end
    end

end

return UISorter