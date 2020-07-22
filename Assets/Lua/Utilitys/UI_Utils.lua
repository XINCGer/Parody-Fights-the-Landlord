---
---                 ColaFramework
--- Copyright © 2018-2049 ColaFramework 马三小伙儿
---                    UI工具类
---

local UI_Utils = Class("UI_Utils")
local AtlasResPathCfg = require("Common.AtlasResPathCfg")

local COMMON_COLORS = {
    Red = Color(225, 0, 0),
    Green = Color(0, 225, 0),
    Blue = Color(0, 0, 225),
    White = Color(240, 244, 244),
    Black = Color(17, 37, 69),
    Gray = Color(91, 93, 112),
    Yellow = Color(227, 147, 7),
}
local chineseNumIndex = 1

-- override 初始化各种数据
function UI_Utils.initialize()

end

-- 获取I18N文字
function UI_Utils.GetText(id)
    local cfg = ConfigMgr.GetItem("Language", id)
    if cfg then
        return cfg.text or ""
    end
    return ""
end

function UI_Utils.GetColor(name)
    return COMMON_COLORS[name] or COMMON_COLORS.White
end

function UI_Utils.GetChineseNumber(number)
    if number >= 0 and number <= 9 then
        return UI_Utils.GetText(chineseNumIndex + number)
    else
        warn(string.format("输入的数字%d不在0~9范围内！", number))
        return ""
    end
end

function UI_Utils.GetSpriteResPath(spriteName)
    if nil == spriteName or "" == spriteName then
        warn("需要指定一个SpriteName")
        return nil
    end
    return AtlasResPathCfg[spriteName] or nil
end

--- 设置一个Image组件的Sprite为某个图集中的图片
--- image:要设置的Image组件
--- spriteName:图集中对应的Sprite的名称
--- keepNativeSize:是否保持原始尺寸
function UI_Utils.SetImageSpriteFromAtlas(image, spriteName, keepNativeSize)
    if nil == image then
        warn("需要指定一个image")
        return
    end
    if nil == spriteName or "" == spriteName then
        warn("需要指定一个SpriteName")
        return
    end
    if image.overrideSprite and image.overrideSprite.name == spriteName then
        return
    end
    local atlasPath = AtlasResPathCfg[spriteName] or nil
    if nil == atlasPath then
        error(spriteName .. " 对应的AtlasResPath 找不到！")
        return
    end
    local spriteAsset = CommonUtil.GetAsset(atlasPath, typeof(UnityEngine.Sprite))
    if nil ~= spriteAsset then
        image.overrideSprite = spriteAsset
        if keepNativeSize then
            image:SetNativeSize()
        end
    end
end

--- 设置一个RawImage的Texture
function UI_Utils.SetRawImage(rawImage, resPath, keepNativeSize)
    if nil == rawImage then
        warn("需要指定RawImage")
        return
    end
    local texture2D = CommonUtil.GetAsset(resPath, typeof(UnityEngine.Texture2D))
    if nil ~= texture2D then
        rawImage.texture = texture2D
        if keepNativeSize then
            rawImage:SetNativeSize()
        end
    end
end

--- Image置灰
function UI_Utils.SetImageGray(image, isGray)
    if nil == image then
        warn("需要指定一个Image")
        return
    end
    image.color = isGray and COMMON_COLORS.Gray or COMMON_COLORS.White
end

function UI_Utils.SetRawImageGray(rawImage, isGray)
    if nil == rawImage then
        warn("需要指定一个RawImage")
        return
    end
    if isGray then
        local garyMat = CommonUtil.GetAsset("Arts/UI/Material/material_defaultgray.mat", typeof(UnityEngine.Material))
        rawImage.material = garyMat
        rawImage.color = COMMON_COLORS.Black
    else
        rawImage.material = nil
        rawImage.color = COMMON_COLORS.White
    end
end

return UI_Utils


