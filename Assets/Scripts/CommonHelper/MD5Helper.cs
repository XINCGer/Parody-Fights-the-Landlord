//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ColaFramework
{
    /// <summary>
    /// MD5码加密工具类
    /// </summary>
    public static class MD5Helper
    {
        /// <summary>
        /// 对指定路径的文件加密，返回加密后的文本
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string MD5EncryptFile(string filePath)
        {
            byte[] retVal;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(fs);
            }
            return retVal.ToHex("x2");
        }


        /// <summary>
        /// 对指定的字符串加密，返回加密后的字符串
        /// </summary>
        /// <param name="originStr"></param>
        /// <returns></returns>
        public static string MD5EncryptString(string originStr)
        {
            byte[] retVal;
            MD5 md5 = new MD5CryptoServiceProvider();
            retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(originStr));
            return retVal.ToHex("x2");
        }
    }
}