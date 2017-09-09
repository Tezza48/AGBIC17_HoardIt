using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoardIt.Assets;

namespace HoardIt.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public GridLayoutGroup Ref_Slots;
        public Inventory Ref_Inventory;
        public int m_Width = 6, m_Height = 5;
        public InventorySlot Prefab_Slot;

        private List<Image> m_ItemDisplay = new List<Image>();
        private Rect m_InventorySlotBounds;
        private float m_InventorySlotOffset;

        private void Awake()
        {
        }

        public void OnSelect(int x, int y)
        {
            
        }

        private void OnEnable()
        {
            Vector2 first = Ref_Slots.transform.GetChild(0).GetComponent<RectTransform>().localPosition + Ref_Slots.transform.localPosition;
            Vector2 last = Ref_Slots.transform.GetChild(Ref_Slots.transform.childCount - 1).GetComponent<RectTransform>().localPosition + Ref_Slots.transform.localPosition;
            m_InventorySlotBounds = new Rect(first, last);
            m_InventorySlotOffset = m_InventorySlotBounds.width / m_Width;
            //DrawInventory();
        }

        // Use this for initialization
        void Start ()
        {
            for (int y = 0; y < m_Height; y++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    var invSlot = Instantiate(Prefab_Slot);
                    invSlot.Init(this, x, y);
                    invSlot.transform.SetParent(Ref_Slots.transform);
                }
            }

            /*if (m_InventorySlotBounds == new Rect())*/
        }
	
	    // Update is called once per frame
	    void Update ()
        {

        }

        private void OnBecameInvisible()
        {

        }

        private void OnBecameVisible()
        {

        }

        private void OnGUI()
        {
            DrawInventory_();
        }

        void DrawInventory()
        {
            // Clear itemDisplay list
            foreach (var item in m_ItemDisplay)
            {
                Destroy(item.gameObject);
            }
            m_ItemDisplay = new List<Image>();
            // Draw New itemDisplay list
            foreach (var item in Ref_Inventory.m_Items)
            {
                Image newImage = new GameObject().AddComponent<Image>();
                m_ItemDisplay.Add(newImage);
                newImage.transform.SetParent(transform);
                newImage.rectTransform.pivot = Vector2.zero;
                newImage.rectTransform.anchorMin = Vector2.zero;
                newImage.rectTransform.anchorMax = Vector2.zero;
                newImage.rectTransform.localPosition = item.Bounds.position * 90 + m_InventorySlotBounds.position;
                newImage.rectTransform.sizeDelta = item.Bounds.max * 90;
                newImage.name = item.Name + " Display";
                newImage.sprite = item.Sprite;
                newImage.raycastTarget = false;
            }
        }
        void DrawInventory_()
        {
            // Draw New itemDisplay list
            foreach (var item in Ref_Inventory.m_Items)
            {
                item.m_Shape.xMax *= Screen.width / 6;
                item.m_Shape.yMax *= Screen.height/5;
                GUI.DrawTextureWithTexCoords(item.Bounds, item.Sprite.texture, item.Sprite.textureRect);
            }
        }
    }
}
