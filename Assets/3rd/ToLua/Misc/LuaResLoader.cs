/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
//优先读取persistentDataPath/系统/Lua 目录下的文件（默认下载目录）
//未找到文件再去读取StreamingAsset目录下的lua文件
using UnityEngine;
using LuaInterface;
using System.IO;
using System.Text;
using ColaFramework;


/// <summary>
/// 继承自LuaFileUtils 重写里面的ReadFile
/// </summary>
public class LuaResLoader : LuaFileUtils
{
    public LuaResLoader()
    {
        instance = this;
        beZip = AppConst.LuaBundleMode;
    }

    /// <summary>
    /// 当LuaVM加载Lua文件的时候，这里就会被调用，
    /// 用户可以自定义加载行为，只要返回byte[]即可。
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public override byte[] ReadFile(string fileName)
    {
#if !UNITY_EDITOR
        byte[] buffer = ReadDownLoadFile(fileName);

        if (buffer == null)
        {
            buffer = ReadResourceFile(fileName);
        }        
        
        if (buffer == null)
        {
            buffer = base.ReadFile(fileName);
        }        
#else
        byte[] buffer = base.ReadFile(fileName);

        if (buffer == null)
        {
            buffer = ReadResourceFile(fileName);
        }

        if (buffer == null)
        {
            buffer = ReadDownLoadFile(fileName);
        }
#endif

        return buffer;
    }

    public override string FindFileError(string fileName)
    {
        if (Path.IsPathRooted(fileName))
        {
            return fileName;
        }

        if (Path.GetExtension(fileName) == ".lua")
        {
            fileName = fileName.Substring(0, fileName.Length - 4);            
        }

        using (CString.Block())
        {
            CString sb = CString.Alloc(512);

            for (int i = 0; i < searchPaths.Count; i++)
            {
                sb.Append("\n\tno file '").Append(searchPaths[i]).Append('\'');
            }

            sb.Append("\n\tno file './Resources/").Append(fileName).Append(".lua'")
              .Append("\n\tno file '").Append(LuaConst.luaResDir).Append('/')
			  .Append(fileName).Append(".lua'");
            sb = sb.Replace("?", fileName);

            return sb.ToString();
        }
    }

    byte[] ReadResourceFile(string fileName)
    {
        if (!fileName.EndsWith(".lua"))
        {
            fileName += ".lua";
        }

        byte[] buffer = null;
        string path = "Lua/" + fileName;
        TextAsset text = Resources.Load(path, typeof(TextAsset)) as TextAsset;

        if (text != null)
        {
            buffer = text.bytes;
            Resources.UnloadAsset(text);
        }

        return buffer;
    }

    byte[] ReadDownLoadFile(string fileName)
    {
        if (!fileName.EndsWith(".lua"))
        {
            fileName += ".lua";
        }

        string path = fileName;

        if (!Path.IsPathRooted(fileName))
        {            
            path = string.Format("{0}/{1}", LuaConst.luaResDir, fileName);            
        }

        if (File.Exists(path))
        {
#if !UNITY_WEBPLAYER
            return File.ReadAllBytes(path);
#else
            throw new LuaException("can't run in web platform, please switch to other platform");
#endif
        }

        return null;
    }


    /// <summary>
    /// 添加打入Lua代码的AssetBundle
    /// </summary>
    /// <param name="bundle"></param>
    public void AddBundle(string bundleName)
    {
        string url = AppConst.AssetPath + bundleName.ToLower();
        if (File.Exists(url))
        {
            var bytes = File.ReadAllBytes(url);
            // 已注释, CreateFromMemoryImmediate从5.3开始改为LoadFromMemory,需要用的请自行取消注释~
            // AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(bytes);
            AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
            if (bundle != null)
            {
                bundleName = bundleName.Replace("lua/", "").Replace(AppConst.ExtName, "");
                base.AddSearchBundle(bundleName.ToLower(), bundle);
            }
        }
    }
}
