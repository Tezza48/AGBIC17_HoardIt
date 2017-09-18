using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HoardIt.Core;

namespace HoardIt.Assets
{
    [Serializable]
    public class Item
    {
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private EItemBaseType m_Type;
        [SerializeField]
        public Rect m_Shape;
        [SerializeField]
        private Sprite m_Sprite;

        public string Name { get { return m_Name; } set { m_Name = value; } }

        public EItemBaseType Type { get { return m_Type; } }

        public Sprite Sprite { get { return m_Sprite; } set { m_Sprite = value; } }

        public Rect Position { get { return m_Shape; } set { m_Shape = value; } }
        
        public Item(string name, EItemBaseType type, Point size, Sprite sprite)
        {
            m_Name = name;
            m_Type = type;
            m_Shape = new Rect(Vector2.zero, (Vector2)size);
            m_Sprite = sprite;
        }
    }

}
