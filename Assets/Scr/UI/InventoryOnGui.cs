using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoardIt.Assets;
namespace HoardIt.UI
{
    public class InventoryOnGui : MonoBehaviour {

        public Sprite Res_SlotSprite;
        public Texture Res_Texture;
        public Inventory Ref_Inventory;
        public int[] m_InvSlotSize;

        public float m_GuiScale;

        private void OnGUI()
        {
            Debug.Log("Screen\n" + "Width: " + Screen.width + " Height: " + Screen.height);
            DrawInventorySlots();
            DrawItems();
        }

        private void DrawItems()
        {
            int a = 0;
            // Draw Items
            foreach (var item in Ref_Inventory.m_Items)
            {
                Rect position = item.Position;
                position.x *= m_GuiScale;
                position.y = (Screen.height - item.Position.y * m_GuiScale) - (item.Position.height * m_GuiScale);
                position.width *= m_GuiScale;
                position.yMax *= m_GuiScale;

                Rect coords = item.Sprite.rect;
                float xPos = coords.x / item.Sprite.texture.width;
                float yPos = coords.y / item.Sprite.texture.height;
                float w = coords.width / item.Sprite.texture.width;
                float h = coords.height / item.Sprite.texture.height;
                coords.Set(xPos, yPos, w, h);

                GUI.DrawTextureWithTexCoords(position, item.Sprite.texture, coords);
                Debug.Log("Drawing Item " + item.Name + "\n" + position.ToString() + " " + coords.ToString());
                a++;
            }
        }

        private void DrawInventorySlots()
        {
            // Draw Slots
            for (int y = 0; y <= m_InvSlotSize[1]; y++)
            {
                for (int x = 0; x <= m_InvSlotSize[0]; x++)
                {
                    Rect position = new Rect(
                        m_GuiScale * x, Screen.height - y * m_GuiScale - m_GuiScale,
                        m_GuiScale, m_GuiScale);
                    Rect coords = Res_SlotSprite.rect;
                    float xPos = coords.x / Res_SlotSprite.texture.width;
                    float yPos = coords.y / Res_SlotSprite.texture.height;
                    float w = coords.width / Res_SlotSprite.texture.width;
                    float h = coords.height / Res_SlotSprite.texture.height;
                    coords.Set(xPos, yPos, w, h);
                    GUI.DrawTextureWithTexCoords(position, Res_SlotSprite.texture, coords);
                }
            }
        }

        // Use this for initialization
        void Start ()
        {
		
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
		
	    }
    }
}
