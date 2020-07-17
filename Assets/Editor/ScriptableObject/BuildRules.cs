using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ColaFramework.ToolKit
{
    [CreateAssetMenu(fileName = "BuildRules.asset", menuName = "ColaFramework/BuildRules", order = 3)]
    public class BuildRules : SerializedScriptableObject
    {
        [LabelText("按照文件夹标记")]
        [SerializeField]
        internal List<string> MarkWithDirList;

        [LabelText("按照文件标记")]
        [SerializeField]
        internal List<string> MarkWithFileList;

        [LabelText("标记为一个Bundle")]
        [SerializeField]
        internal List<string> MarkWithOneBundleList;

        [LabelText("手动处理,程序不会自动处理的列表")]
        [SerializeField]
        internal List<string> ManualList;

        public bool IsInBuildRules(string path)
        {
            if (null != ManualList)
            {
                foreach (var item in ManualList)
                {
                    if (path.StartsWith(item))
                    {
                        return true;
                    }
                }
            }

            if (null != MarkWithDirList)
            {
                foreach (var item in MarkWithDirList)
                {
                    if (path.StartsWith(item))
                    {
                        return true;
                    }
                }
            }

            if (null != MarkWithFileList)
            {
                foreach (var item in MarkWithFileList)
                {
                    if (path.StartsWith(item))
                    {
                        return true;
                    }
                }
            }

            if (null != MarkWithOneBundleList)
            {
                foreach (var item in MarkWithOneBundleList)
                {
                    if (path.StartsWith(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
