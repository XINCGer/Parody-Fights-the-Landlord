//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;

/// <summary>
/// UI排序组件
/// </summary>
public class SorterTag : MonoBehaviour
{
    public int Space3D = 0;

    public int GetSorter()
    {
        return transform.GetSiblingIndex();
    }

    public void SetSorter(int index)
    {
        if (index == -1)
        {
            transform.SetAsFirstSibling();
        }
        else
        {
            transform.SetSiblingIndex(index);
        }
    }

    public void SetSpace3D(int z)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,z);
    }
}
