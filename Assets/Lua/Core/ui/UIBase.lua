---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 UIBase基类
---

local UIBase = Class("UIBase")

-- override 初始化各种数据
function UIBase:initialize()
    self.Panel = nil
    self.ResPath = nil
    self.Layer = 0
    self.UILevel = 0
    self.subUIList = {}
    self.uiDepthLayer = ECEnumType.UIDepth.NORMAL
    self.uiCanvas = nil
    self.sortEnable = true
    self.sorterTag = nil
    self.uguiMsgHandler = nil
    self.PanelName = ""
    self.isExist = false
    self.isShowUIBlur = false
    self.isShowUIMask = false
    self.bindView = nil
    self:InitParam()
end

-- virtual 子类可以初始化一些变量
function UIBase:InitParam()

end

-- 对外调用，用于创建UI
function UIBase:Create()
    if nil ~= self.Panel then
        CommonUtil.DiscardGameObject(self.ResPath, self.Panel)
    end

    local ret, bindView = pcall(require, "UIBindViews." .. tostring(self) .. "_BindView")
    if ret then
        self.bindView = bindView
        self.ResPath = bindView.viewPath
    else
        warn("UIView don't have BindView! " .. tostring(self))
    end
    if (nil == self.ResPath) then
        error("UIView respath is nil " .. tostring(self))
        return
    end

    self.Panel = CommonUtil.InstantiatePrefab(self.ResPath, CommonUtil.GetUIRootTransform())
    self.PanelName = self.Panel.name
    self.Layer = self.Panel.layer
    -- 如果参与UI排序
    if self.sortEnable then
        self.sorterTag = self.Panel:AddSingleComponent(typeof(SorterTag))
        self.uiCanvas = self.Panel:AddSingleComponent(typeof(UnityEngine.Canvas))
        self.Panel:AddSingleComponent(typeof(UnityEngine.UI.GraphicRaycaster))
        self.uiCanvas.overrideSorting = true
        self.Panel:AddSingleComponent(typeof(ParticleOrderAutoSorter))
        UIManager.GetUISorterMgr():AddPanel(self)
    end
    self.isExist = true
    -- ShowUIBlur 与 ShowUIMask 互斥
    if self.isShowUIBlur then
        UIManager.ShowUIBlur(self)
    elseif self.isShowUIMask then
        UIManager.ShowUIMask(self)
    end

    if ret then
        bindView.BindView(self, self.Panel)
    end
    self:AttachListener(self.Panel)
    self:RegisterEvent()
    self:OnCreate()
    self:OnShow(self:IsVisible())
end

--对外调用，用于创建UI，不走ResPath加载，直接由现有gameObject创建
function UIBase:CreateWithGo(gameObejct)
    self.Panel = gameObejct
    self.PanelName = self.Panel.name
    self.Layer = self.Panel.layer
    if self.sortEnable then
        self.sorterTag = self.Panel:AddSingleComponent(typeof(SorterTag))
        self.uiCanvas = self.Panel:AddSingleComponent(typeof(UnityEngine.Canvas))
        self.Panel:AddSingleComponent(typeof(UnityEngine.GraphicRaycaster))
        self.uiCanvas.overrideSorting = true
        self.Panel:AddSingleComponent(typeof(ParticleOrderAutoSorter))
        UIManager.GetUISorterMgr():AddPanel(self)
    end
    self.isExist = true
    -- ShowUIBlur 与 ShowUIMask 互斥
    if self.isShowUIBlur then
        UIManager.ShowUIBlur(self)
    elseif self.isShowUIMask then
        UIManager.ShowUIMask(self)
    end
    self:AttachListener(self.Panel)
    self:RegisterEvent()
    self:OnCreate()
    self:OnShow(self:IsVisible())
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UIBase:OnCreate()

end

-- 界面可见性变化的时候触发
function UIBase:OnShow(isShow)

end

-- 设置界面可见性
function UIBase:SetVisible(isVisible)
    self.Panel:SetActive(isVisible)
    self:OnShow(isVisible)
end

-- 返回界面的可见性
function UIBase:IsVisible()
    return nil ~= self.Panel and self.Panel.activeSelf
end

-- 返回界面实例是否存在
function UIBase:IsExist()
    return self.isExist
end

-- 销毁一个UI界面，不要直接调用，通过UIManger关闭、派发消息关闭或者直接调用DestroySelf函数关闭
function UIBase:Destroy()
    if self.sortEnable then
        UIManager.GetUISorterMgr():RemovePanel(self)
    end
    self:DestroySubPanels()
    self:UnRegisterEvent()
    self:UnAttachListener(self.Panel)
    if nil ~= self.bindView then
        self.bindView.UnBindView(self)
    end
    if self.isShowUIBlur then
        CommonUtil.DestroyUIBlur()
    end
    if nil ~= self.Panel then
        if 0 ~= self.ResPath then
            CommonUtil.DiscardGameObject(self.ResPath, self.Panel)
            self.Panel = nil
        else
            self:SetVisible(false)
        end
    end
    self.isExist = false
    self:OnDestroy()
end

-- 界面销毁的过程中触发
function UIBase:OnDestroy()
    self.Panel = nil
    self.bindView = nil
    self.Layer = 0
    self.uiCanvas = nil
    self.sorterTag = nil
    self.uguiMsgHandler = nil
    self.PanelName = ""
    self.isExist = false
    self.isShowUIBlur = false
    self.isShowUIMask = false
end

-- 销毁自身，可以在自身的方法中调用，同时会去UIManager中清理
function UIBase:DestroySelf()
    UIManager.CloseUISelf(self)
end

-- 关联子UI，统一参与管理
function UIBase:AttachSubPanel(subPanelPath, subUI, uiLevel)
    if nil == subPanelPath or subPanelPath == "" then
        return
    end
    local subUIObj = self.Panel:FindChildByPath(subPanelPath)
    if nil ~= subUIObj then
        subUI:CreateWithGo(subUIObj, uiLevel)
        table.insert(self.subUIList, subUI)
    end
end

-- 将一个UI界面注册为本UI的子界面，统一参与管理
function UIBase:RegisterSubPanel(subUI)
    if nil == subUI then
        return
    end
    subUI.uiDepthLayer = self.uiDepthLayer
    table.insert(self.subUIList, subUI)
end

-- 解除子UI关联
function UIBase:DetchSubPanel(subUI)
    if nil ~= self.subUIList then
        table.remove(subUI)
    end
end

--  销毁关联的子面板，不要重写
function UIBase:DestroySubPanels()
    if nil ~= self.subUIList then
        for _, v in ipairs(self.subUIList) do
            v:Destroy()
            v.Panel = nil
        end
    end
    for _, v in pairs(self.subUIList) do
        v = nil
    end
    self.subUIList = {}
end

-- 将当前UI层级提高，展示在当前Level的最上层
function UIBase:BringTop()
    UIManager.GetUISorterMgr():MovePanelToTop(self)
end

-- 将当前UI提升到指定UIDepthLayer的最上层
function UIBase:BringToTopOfLayer(depthLayer)
    UIManager.GetUISorterMgr():MovePanelToTopOfLayer(self, depthLayer)
end

-- 显示UI背景模糊
function UIBase:ShowUIBlur(isShow)
    self.isShowUIBlur = isShow
end

-- 显示UI背景遮罩
function UIBase:ShowUIMask(isShow)
    self.isShowUIMask = isShow
end

-- 设置点击外部关闭(执行该方法以后，当点击其他UI的时候，会自动关闭本UI)
function UIBase:SetOutTouchDisappear()
    UIManager.SetOutTouchDisappear(self)
end

-- 获取UITableviewCell
function UIBase:GetCellView(tableview, cell)
    if nil ~= self.bindView then
        return self.bindView.GetCellView(self, tableview, cell)
    end
    return nil
end

-- 注册UIEventListener
function UIBase:AttachListener(gameObject)
    if nil == gameObject then
        return
    end
    local uguiMsgHandler = gameObject:GetComponent("UGUIMsgHandler")
    if uguiMsgHandler == nil then
        uguiMsgHandler = gameObject:AddComponent(typeof(UGUIMsgHandler))
    end
    self.uguiMsgHandler = uguiMsgHandler

    -- BindFunction
    self.uguiMsgHandler.onClick = function(name)
        -- 添加对点击Blur的判断
        if self.isShowUIBlur and name == "blur_" .. self.PanelName then
            self:Destroy()
        elseif self.isShowUIMask and name == "mask_" .. self.PanelName then
            self:Destroy()
        end
        self:onClick(name)
    end
    self.uguiMsgHandler.onBoolValueChange = function(name, isSelect)
        self:onBoolValueChange(name, isSelect)
    end
    self.uguiMsgHandler.onEvent = function(eventName)
        self:onEvent(eventName)
    end
    self.uguiMsgHandler.onFloatValueChange = function(name, value)
        self:onFloatValueChange(name, value)
    end
    self.uguiMsgHandler.onIntValueChange = function(name, value)
        self:onIntValueChange(name, value)
    end
    self.uguiMsgHandler.onStrValueChange = function(name, text)
        self:onStrValueChange(name, text)
    end
    self.uguiMsgHandler.onDrag = function(name, deltaPos, curToucPosition)
        self:onDrag(name, deltaPos, curToucPosition)
    end
    self.uguiMsgHandler.onBeginDrag = function(name, deltaPos, curToucPosition)
        self:onBeginDrag(name, deltaPos, curToucPosition)
    end
    self.uguiMsgHandler.onEndDrag = function(name, deltaPos, curToucPosition)
        self:onEndDrag(name, deltaPos, curToucPosition)
    end
    self.uguiMsgHandler.onTableviewCellInit = function(tableview, cell)
        self:onTableviewCellInit(tableview, cell)
    end
    self.uguiMsgHandler.onTableviewClick = function(tableview, target)
        self:onTableviewClick(tableview, target)
    end

    self.uguiMsgHandler:AttachListener(gameObject)
end

function UIBase:UnAttachListener(gameObject)
    if nil == gameObject then
        return
    end
    if self.uguiMsgHandler then
        self.uguiMsgHandler:UnAttachListener(gameObject)
    end
    --UnBindFunction
    self.uguiMsgHandler.onClick = nil
    self.uguiMsgHandler.onBoolValueChange = nil
    self.uguiMsgHandler.onEvent = nil
    self.uguiMsgHandler.onFloatValueChange = nil
    self.uguiMsgHandler.onStrValueChange = nil
    self.uguiMsgHandler.onDrag = nil
    self.uguiMsgHandler.onBeginDrag = nil
    self.uguiMsgHandler.onEndDrag = nil
    self.uguiMsgHandler.onTableviewCellInit = nil
    self.uguiMsgHandler.onTableviewClick = nil
    self.uguiMsgHandler.onTableviewPress = nil

    self.uguiMsgHandler = nil

end

-- 注册UI事件监听
function UIBase:RegisterEvent()

end

-- 取消注册UI事件监听
function UIBase:UnRegisterEvent()

end

------------------- UI事件回调 --------------------------
function UIBase:onClick(name)

end

function UIBase:onBoolValueChange(name, isSelect)

end

function UIBase:onEvent(eventName)
    if eventName == "onClick" then
        UIManager.NotifyDisappear(self.PanelName)
    end
end

function UIBase:onFloatValueChange(name, value)

end

function UIBase:onIntValueChange(name, index)

end

function UIBase:onStrValueChange(name, text)

end

function UIBase:onDrag(name, deltaPos, curToucPosition)

end

function UIBase:onBeginDrag(name, deltaPos, curToucPosition)

end

function UIBase:onEndDrag(name, deltaPos, curToucPosition)

end

function UIBase:onTableviewCellInit(tableview, cell)

end

function UIBase:onTableviewClick(tableview, target)

end
---------------------- UI事件回调 --------------------------

return UIBase