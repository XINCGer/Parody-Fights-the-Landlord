---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---              Login_Controller Controller业务逻辑
---

--- 公有字段和方法
local public = {}
--- 私有字段和方法
local private = {}

--- Controller模块的初始化，可以在这里做初始化和添加监听等操作
function public.OnInit()
    NetManager.Register(Protocol.C2S_Login, private.OnLoginServer)
end

--- Controller模块的销毁，可以在这里做清理工作和取消监听等操作
function public.OnDestroy()
    NetManager.UnRegister(Protocol.C2S_Login)
end

function public.RequestConnectServer()
    NetManager.Connect(LuaAppConst.ServerIp, LuaAppConst.Port, function()
        print("--------------->连接成功!")
        public.RequestLoginServer()
    end)
end

--- 测试请求登录服务器
function public.RequestLoginServer()
    NetManager.RequestSproto(Protocol.C2S_Login, { accountId = 1001, charId = 10086, userName = "Jackson" })
end

--- 测试函数如无需要可以删除
function private.OnLoginServer(code, msg)
    print("------------>成功登录游戏服务器,code is:", code, "msg is:", msg)
    UIManager.Close(ECEnumType.UIEnum.Loading)
    UIManager.Open(ECEnumType.UIEnum.WorldDialog)
    SceneCharacter.CreateSceneCharacterInf("Arts/Avatar/Blade_Girl.prefab", AnimCtrlEnum.CharAnimator, true)
end

return public