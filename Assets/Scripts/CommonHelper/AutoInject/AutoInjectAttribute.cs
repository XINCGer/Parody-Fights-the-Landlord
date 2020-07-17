//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Reflection;

namespace ColaFramework
{
    /// <summary>
    /// 自动注入标签（打上该标签的都可以参与自动注入）
    /// </summary>
    public class AutoInjectAttribute : Attribute
    {
        public string name { set; get; }
        public Type type { set; get; }
        public FieldInfo fi { set; get; }
        public AutoInjectAttribute(string name = null)
        {
            this.name = name;
        }
    }
}
