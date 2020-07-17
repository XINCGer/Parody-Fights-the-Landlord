---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                   ObjectPool的使用示例
---

--- ObjectPool Test
local LOjectPool = require("Common.Collections.LObjectPool")

local index = 0
local createAction = function()
    local gameobj = UnityEngine.GameObject.New()
    gameobj.name = "CacheObj" .. index
    index = index + 1
    return gameobj
end

local releaseAction = function(obj)
    print("------------>ObjType:",type(obj))
    print("---->释放Obj:",obj.name)
end

local mObjectPool = LOjectPool:New(createAction,releaseAction)

local obj_1 = mObjectPool:get()
print("-------->第一个物体的名字",obj_1.name)

local obj_2 = mObjectPool:get()
print("-------->第二个物体的名字",obj_2.name)

mObjectPool:release(obj_1)

local obj_3 = mObjectPool:get()
print("-------------->type",type(obj_3))
print("-------->第三个物体的名字",obj_3.name)