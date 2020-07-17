---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                   登录界面
---

local UIBase = require("Core.ui.UIBase")
local UILoginPanel = Class("UILoginPanel", UIBase)

local _instance = nil

function UILoginPanel.Instance()
    if nil == _instance then
        _instance = UILoginPanel:new()
    end
    return _instance
end

function UILoginPanel:InitParam()
    self.uiDepthLayer = ECEnumType.UIDepth.NORMAL
    self:ShowUIMask(false)
end

-- override UI面板创建结束后调用，可以在这里获取gameObject和component等操作
function UILoginPanel:OnCreate()
    Util.UI.SetImageSpriteFromAtlas(self.m_okBtn.image, "bt_buy")

    self.m_Dropdown:SetCaptionText("动态更改Caption")
    self.m_Dropdown:AddDropdownItem("选项1")
    self.m_Dropdown:AddDropdownItem("选项2")
    self.m_Dropdown:AddDropdownItem("选项3")
    self.m_Dropdown.onValueChanged = function(index)
        print("------------->选中了第" .. index .. "个选项")
    end
    self.m_Dropdown:RefreshShownValue()
end

-- 界面可见性变化的时候触发
function UILoginPanel:OnShow(isShow)
    Ctrl.Login.RequestConnectServer()
    self.m_vertical_tableview.CellCount = 20
    self.m_vertical_tableview:Reload(true)

    CommonUtil.PlayMultipleSound("Audio/2d/welcome.mp3")
    CommonUtil.PlayMultipleSound("Audio/2d/chat_01.mp3")
    CommonUtil.PlayBackgroundMusic("Audio/2d/MainTheme.mp3")


    local cjson = require "cjson"

    CommonUtil.DownLoadServerList(function (ErrorCode,msg)
        print(ErrorCode,msg)
        --解析json字符串
        local data = cjson.decode(msg)
        print(data)
    end)

end

function UILoginPanel:onClick(name)
    if name == "cancelBtn" then
        self:DestroySelf()
    elseif name == "okBtn" then
        self:DestroySelf()
        UIManager.Instance():Close(ECEnumType.UIEnum.Loading)
        UIManager.Instance():Open(ECEnumType.UIEnum.WorldDialog)
        SceneCharacter.CreateSceneCharacterInf("Arts/Avatar/Blade_Girl.prefab", AnimCtrlEnum.CharAnimator, true)
        Ctrl.Login.RequestConnectServer()
    end
end

function UILoginPanel:onTableviewCellInit(tableview, cell)
    local cellView = self:GetCellView(tableview, cell)
    cellView.m_Text.text = "哈哈" .. cell.index
end

function UILoginPanel:onTableviewClick(tableview, target)
    print("---------->点击了Cell" .. target.name)
end

-- 界面销毁的过程中触发
function UILoginPanel:OnDestroy()
    UIBase.OnDestroy(self)
end

return UILoginPanel