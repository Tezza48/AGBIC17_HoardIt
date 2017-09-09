using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HoardIt.Assets
{
    [Serializable]
    public class Item
    {
        public enum EItem
        {
            Dagger, Sword, Bow, Star
        }
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private EItem m_Type;
        [SerializeField]
        public Rect m_Shape;
        [SerializeField]
        private Sprite m_Sprite;

        public Sprite Sprite { get { return m_Sprite; } set { m_Sprite = value; } }

        public Rect Bounds { get { return m_Shape; } set { m_Shape = value; } }

        public string Name { get { return m_Name; } set { m_Name = value; } }

        public Item()
        {

        }
    }

    public class ItemEntity : Interactable
    {

        protected override void OnHover(PlayerControl player)
        {
            throw new NotImplementedException();
        }

        protected override void OnInteract()
        {
            throw new NotImplementedException();
        }
    }
}
