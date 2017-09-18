using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HoardIt.Assets
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemEntity : Interactable
    {
        const int sortingLayer = -5;
        SpriteRenderer m_SpriteRenderer;
        BoxCollider2D m_Collider;
        Item m_BaseItem;

        public void Init(Item item)
        {
            m_BaseItem = item;
        }

        void Start()
        {
            m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            m_SpriteRenderer.sprite = m_BaseItem.Sprite;
            m_SpriteRenderer.sortingOrder = sortingLayer;
        }

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
