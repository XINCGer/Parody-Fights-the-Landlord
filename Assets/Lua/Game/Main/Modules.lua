---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                 Modules的定义
---

local Modules = {}
local Util = {}
local Ctrl = {}
local Mod = {}

-- 注册全局变量
_G.Modules = Modules

Modules.moduleId = require("Game.Main.ModuleId")
Modules.notifyId = require("Game.Main.NotifyId")

local ModuleNameEnum = {
    Ctrl = 1,
    Mod = 2,
    Util = 3,
}

local function AssemModuleName(name, moduleEnum)
    if ModuleNameEnum.Ctrl == moduleEnum then
        return string.format("Modules.%s.%s_Controller", name, name)
    elseif ModuleNameEnum.Mod == moduleEnum then
        return string.format("Modules.%s.%s_Module", name, name)
    elseif ModuleNameEnum.Util == moduleEnum then
        return string.format("Utilitys.%s_Utils", name)
    end
end

-- 把要注册的工具都放在此列表里面
local UtilList = {
    "LuaCommon",
    "UI",
    "Table",
}

-- 优先启动的Module列表
local PriorityBootList = {
    "Login",
}

-- 正常启动的Module列表
local NornamlBootList = {

}


-- 注册工具类
local function RegisterUtility(name)
    local result, utl = pcall(require, AssemModuleName(name, ModuleNameEnum.Util))
    if result and utl then
        Util[name] = utl
        --执行Utility的initialize初始化方法
        if utl.initialize and "function" == type(utl.initialize) then
            utl.initialize()
        end
    else
        print("RegisterUtility Step Failed! : ",name)
    end
end

-- 注册Module\注册Controller
local function InitModule(name)
    local result, controller = pcall(require, AssemModuleName(name, ModuleNameEnum.Ctrl))
    if result and controller then
        Ctrl[name] = controller
    end
    local result, module = pcall(require, AssemModuleName(name, ModuleNameEnum.Mod))
    if result and module then
        Mod[name] = module
    end
end

local function ShutDownModule(name)
    local controller = Ctrl[name]
    if controller and controller.OnDestroy and "function" == controller.OnDestroy then
        controller.OnDestroy()
    end

    local module = Mod[name]
    if module and module.OnDestroy and "function" == module.OnDestroy then
        controller.OnDestroy()
    end
end

local function RegisterGlobalVar()
    define("Util", Util)
    define("Ctrl", Ctrl)
    define("Mod", Mod)
end

--- 初始化：注册全局变量和工具类，应该优先于Boot启动
function Modules.Initialize()
    RegisterGlobalVar()

    for _, v in ipairs(UtilList) do
        RegisterUtility(v)
    end
end

function Modules.PriorityBoot()
    for _, v in ipairs(PriorityBootList) do
        InitModule(v)
    end

    for _, v in ipairs(PriorityBootList) do
        --执行Module的OnInit方法
        local controller = Ctrl[v]
        if controller and controller.OnInit and "function" == type(controller.OnInit) then
            controller.OnInit()
        end
        --执行Module的OnInit方法
        local module = Mod[v]
        if module and module.OnInit and "function" == type(module.OnInit) then
            module.OnInit()
        end
    end
end

function Modules.Boot()
    for _, v in ipairs(NornamlBootList) do
        InitModule(v)
    end

    for _, v in ipairs(NornamlBootList) do
        --执行Module的OnInit方法
        local controller = Ctrl[v]
        if controller and controller.OnInit and "function" == type(controller.OnInit) then
            controller.OnInit()
        end
        --执行Module的OnInit方法
        local module = Mod[v]
        if module and module.OnInit and "function" == type(module.OnInit) then
            module.OnInit()
        end
    end
end

function Modules.ShutDown()
    for _, v in ipairs(PriorityBootList) do
        ShutDownModule(v)
    end
    for _, v in ipairs(NornamlBootList) do
        ShutDownModule(v)
    end

    for k, v in pairs(Ctrl) do
        package.loaded[AssemModuleName(k,ModuleNameEnum.Ctrl)] = nil
        Ctrl[k] = nil
    end

    for k, v in pairs(Mod) do
        package.loaded[AssemModuleName(k,ModuleNameEnum.Mod)] = nil
        Mod[k] = nil
    end
end

return Modules