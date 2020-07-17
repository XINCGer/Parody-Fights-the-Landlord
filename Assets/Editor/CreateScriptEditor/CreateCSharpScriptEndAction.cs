//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEditor.ProjectWindowCallback;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// 创建C#代码的EndAction
    /// </summary>
    public class CreateCSharpScriptEndAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            CreateTemplateFile(pathName, resourceFile, fileNameWithoutExtension);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

        internal static void CreateTemplateFile(string pathName, string resourceFile, string replaceName)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            text = Regex.Replace(text, "#NAME#", replaceName);
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
        }
    }
}