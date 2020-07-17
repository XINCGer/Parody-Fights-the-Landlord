//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
	public sealed class UIComponentCollection : MonoBehaviour
	{
		[SerializeField]
		private List<Component> components = new List<Component>();

		public T Get<T>(int index) where T : Component
		{
			return (T)components[index];
		}
        public Component Get(int index)
        {
            return components[index];
        }

        [LuaInterface.NoToLua]
        public void Clear()
        {
            components.Clear();
        }

        [LuaInterface.NoToLua]
        public void Add(Component component)
        {
            components.Add(component);
        }
    }
}
