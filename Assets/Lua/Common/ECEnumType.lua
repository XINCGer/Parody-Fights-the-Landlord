---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                通用的枚举定义
---

local ECEnumType = {}
_G.ECEnumType = ECEnumType

---UI界面枚举
ECEnumType.UIEnum = {
    Login = 1, -- 登录界面
    Loading = 2, --Loading界面
    DebugPanel = 3, -- Debug信息界面
    WorldDialog = 4, --设置界面
    UIModel = 5, --UGUIModel测试界面
}

-- UI Level逻辑层级
-- UI之间的从属关系
ECEnumType.UILevel = {
    None = 0, -- 不参与UI打开关闭的管理
    Level1 = 1, -- Level1 界面两两互斥，打开一个则会关闭另一个，并存储下来
    Level2 = 2, -- Level2 的界面会在Level1界面关闭的时候统一被关闭掉
    Level3 = 3, -- Level3 的界面之间打开一个新的界面成功后会销毁另外一个
}

-- UI 的层级，上下级显示，渲染的层级
ECEnumType.UIDepth = {
    BOTTOMMOST = 1, -- 最下面的一层
    BOTTOM = 2, -- 较低的层级
    NORMAL = 3, -- 正常的层级
    TOP = 4, -- 顶层
    TOPMOST = 5, -- 最顶层，显示在最上面
    DEBUG = 6, -- 用于调试的层级
}

ECEnumType.DownErrorCode = {
    SUCCESS = 0,
    ERROR = 1,
    TIME_OUT = 2,
}

return ECEnumType