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
        
        public float m_InvSlotSizeScreenSpace;
        public Rect m_InvBoundsScreenSpace;

        public Sprite xHair;

        private void Awake()
        {

        }

        public void OnSelect(int x, int y)
        {
            
        }

        private void OnEnable()
        {
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
                    invSlot.name = "InvSlot " + x + ", " + y;
                    invSlot.Init(this, x, y);
                    invSlot.transform.SetParent(Ref_Slots.transform);
                }
            }

            
            /*if (m_InventorySlotBounds == new Rect())*/
        }
	
	    // Update is called once per frame
	    void Update ()
        {
            var firstR = Ref_Slots.transform.GetChild(0).GetComponent<RectTransform>();
            var lastR = Ref_Slots.transform.GetChild(Ref_Slots.transform.childCount - 1).GetComponent<RectTransform>();
            Vector2 first = firstR.position;
            first.y -= firstR.sizeDelta.y;
            Vector2 last = lastR.position;
            last.y -= lastR.sizeDelta.y;

            // Swap the y values because of screen space
            Vector2 temp = first;
            first.y = last.y;
            last.y = temp.y;

            m_InvBoundsScreenSpace = new Rect(first, last - first);
            m_InvSlotSizeScreenSpace = m_InvBoundsScreenSpace.width / (m_Width - 1);
            //m_InvSlotSizeScreenSpace = 100;
        }

        private void OnGUI()
        {
            DrawInventory();
            //DebugInvBounds();
        }

        private void DebugInvBounds()
        {
            Rect coords = xHair.rect;
            coords.x = coords.x / xHair.texture.width;
            coords.y = coords.y / xHair.texture.height;
            coords.width = coords.width / xHair.texture.width;
            coords.height = coords.height / xHair.texture.height;

            GUI.DrawTextureWithTexCoords(new Rect(m_InvBoundsScreenSpace.min - new Vector2(5f, 5f), new Vector2(10, 10)), xHair.texture, coords);
            GUI.DrawTextureWithTexCoords(new Rect(m_InvBoundsScreenSpace.max - new Vector2(5f, 5f), new Vector2(10, 10)), xHair.texture, coords);
        }
        
        void DrawInventory()
        {
            // convert inv bounds into screen space
            
            // Draw New itemDisplay list
            foreach (var item in Ref_Inventory.m_Items)
            {
                Rect position = item.Position;
                position.x = m_InvBoundsScreenSpace.x + position.x * m_InvSlotSizeScreenSpace;
                position.y = m_InvBoundsScreenSpace.y + position.y * m_InvSlotSizeScreenSpace;
                position.width *= m_InvSlotSizeScreenSpace;
                position.height *= m_InvSlotSizeScreenSpace;

                Rect coords = item.Sprite.rect;
                coords.x = coords.x / item.Sprite.texture.width;
                coords.y = coords.y / item.Sprite.texture.height;
                coords.width = coords.width / item.Sprite.texture.width;
                coords.height = coords.height / item.Sprite.texture.height;

                GUI.DrawTextureWithTexCoords(position, item.Sprite.texture, coords);
            }
        }
    }
}
