---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                UI界面的统一注册
---

local GUICollections = {
    [ECEnumType.UIEnum.Login] = require("Modules.Login.Views.UILoginPanel"),
    [ECEnumType.UIEnum.Loading] = require("Modules.Common.Views.UILoading"),
}

return GUICollections