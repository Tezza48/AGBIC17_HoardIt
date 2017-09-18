using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HoardIt.Core;

namespace HoardIt.Assets
{
    public enum EItemBaseType
    {
        Dagger, Sword, Bow, Star, 
    }

    [Serializable]
    public struct TypeSizeSprite
    {
        public EItemBaseType m_Type;
        public Point m_Size;
        public Sprite m_Sprite;
    }

    public class ItemBase : MonoBehaviour
    {
        public static ItemBase singleton;

        public TypeSizeSprite[] baseTypes;

        void Start()
        {
            if(singleton == null)
            {
                singleton = this;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
