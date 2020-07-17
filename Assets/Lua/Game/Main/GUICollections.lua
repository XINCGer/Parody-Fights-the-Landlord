---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                UI界面的统一注册
---

local GUICollections = {
    [ECEnumType.UIEnum.Login] = require("Modules.Login.Views.UILoginPanel"),
    [ECEnumType.UIEnum.Loading] = require("Modules.Common.Views.UILoading"),
    [ECEnumType.UIEnum.DebugPanel] = require("Modules.Common.Views.UIDebugPanel"),
    [ECEnumType.UIEnum.WorldDialog] = require("Modules.World.Views.UIWorldDialog"),
    [ECEnumType.UIEnum.UIModel] = require("Modules.Test.Views.UIModelPanel"),
}

return GUICollections