//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

namespace ColaFramework
{
    /// <summary>
    /// 自动注入功能的主逻辑
    /// </summary>
    public class AutoInject
    {
        /// <summary>
        /// 供外界调用的注入接口
        /// </summary>
        /// <param name="go"></param>脚本对应的Gameobject(传入该Gameobject的Root节点即可)
        /// <param name="obj"></param>该脚本,一般传入this即可
        public static void Inject(GameObject go, object obj)
        {
            if (go == null || obj == null)
            {
                return;
            }
            Dictionary<string, List<AutoInjectAttribute>> dic = ScanObject(obj);
            if (dic != null)
            {
                BeginInject(obj, go, dic);
                dic.Clear();
            }
        }

        /// <summary>
        /// 通过反射机制扫描出脚本中打了AutoInjectAttibute标签的字段、属性等内容
        /// </summary>
        /// <param name="obj"></param>对应的脚本类
        /// <returns></returns>
        private static Dictionary<string, List<AutoInjectAttribute>> ScanObject(object obj)
        {
            Dictionary<string, List<AutoInjectAttribute>> dic = null;

            FieldInfo[] sz_fi = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (sz_fi != null)
            {
                int count = sz_fi.Length;
                for (int i = 0; i < count; i++)
                {
                    FieldInfo fi = sz_fi[i];
                    AutoInjectAttribute[] sz_attr = fi.GetCustomAttributes(typeof(AutoInjectAttribute), true) as AutoInjectAttribute[];

                    if (sz_attr != null && sz_attr.Length > 0)
                    {
                        if (sz_attr.Length > 1)
                        {
                            Debug.LogWarning("自动注入只能标记一次，重复的会取最后一次");
                        }

                        AutoInjectAttribute attr = sz_attr[sz_attr.Length - 1];
                        attr.fi = fi;
                        attr.type = fi.FieldType;
                        string name = string.IsNullOrEmpty(attr.name) ? "" : attr.name;
                        dic = dic == null ? new Dictionary<string, List<AutoInjectAttribute>>() : dic;
                        if (!dic.ContainsKey(name))
                        {
                            dic.Add(name, new List<AutoInjectAttribute>());
                        }
                        dic[name].Add(attr);
                    }
                }
            }
            return dic;

        }


        /// <summary>
        /// 深度遍历去搜索Gameobject下面的子物体，完成注入
        /// </summary>
        /// <param name="go"></param>要扫描的Root节点
        /// <param name="_callback"></param>回调函数
        public static void ScanAllChild(GameObject go, Action<GameObject> _callback)
        {
            if (go == null)
            {
                return;
            }
            _callback(go);
            int count = go.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject gochild = go.transform.GetChild(i).gameObject;
                ScanAllChild(gochild, _callback);
            }
        }

        /// <summary>
        /// 自动注入的主要逻辑
        /// </summary>
        /// <param name="obj"></param>脚本类对象
        /// <param name="root"></param>脚本对应的GameObject的Root
        /// <param name="dic"></param>扫描出的字段，属性等内容
        private static void BeginInject(object obj, GameObject root, Dictionary<string, List<AutoInjectAttribute>> dic)
        {
            if (obj == null || root == null || dic == null)
            {
                return;
            }
            ScanAllChild(root, x =>
            {
                string key = x.name;
                if (x == root)
                {
                    key = "";
                }
                List<AutoInjectAttribute> list = null;
                dic.TryGetValue(key, out list);
                if (list != null)
                {
                    int count = list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var v = list[i];
                        if (v.type == typeof(GameObject))
                        {
                            v.fi.SetValue(obj, x);
                            continue;
                        }
                        if (v.type == typeof(Transform))
                        {
                            v.fi.SetValue(obj, x.transform);
                            continue;
                        }
                        //if (v.type.IsAssignableFrom(typeof(Component)))
                        if (typeof(Component).IsAssignableFrom(v.type))
                        {
                            v.fi.SetValue(obj, x.GetComponent(v.type));
                            continue;
                        }
                    }
                }
            });
        }


    }
}