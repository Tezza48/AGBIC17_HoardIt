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

            // Draw Slots
            for (int y = 0; y < m_InvSlotSize[1]; y++)
            {
                for (int x = 0; x < m_InvSlotSize[0]; x++)
                {
                    Rect position = new Rect(
                        m_GuiScale * x, Screen.height - y * m_GuiScale,
                        m_GuiScale, m_GuiScale);
                    Rect coords = Res_SlotSprite.rect;
                    float xPos = coords.x / Res_SlotSprite.texture.width;
                    float yPos = coords.y / Res_SlotSprite.texture.height;
                    float w = coords.width / Res_SlotSprite.texture.width;
                    float h = coords.height / Res_SlotSprite.texture.height;
                    coords.Set(xPos, yPos, w, h);
                    GUI.DrawTextureWithTexCoords(position, Res_SlotSprite.texture, coords);
                    Debug.Log("Drawing Slot\n" + position.ToString() + " " + coords.ToString());
                }
            }

            // Draw Items
            foreach (var item in Ref_Inventory.m_Items)
            {
                Rect position = new Rect(
                    m_GuiScale * item.Position.x, Screen.height - item.Position.y * m_GuiScale,
                    m_GuiScale * item.Position.xMax, m_GuiScale * item.Position.yMax);
                Rect coords = item.Sprite.rect;
                float xPos = coords.x / item.Sprite.texture.width;
                float yPos = coords.y / item.Sprite.texture.height;
                float w = coords.width / item.Sprite.texture.width;
                float h = coords.height / item.Sprite.texture.height;
                coords.Set(xPos, yPos, w, h);
                GUI.DrawTextureWithTexCoords(position, item.Sprite.texture, coords);
                Debug.Log("Drawing Item\n" + position.ToString() + " " + coords.ToString());
            }
        }

        private void DrawItems()
        {
            foreach (var item in Ref_Inventory.m_Items)
            {
                item.Sprite = Res_SlotSprite;
                Rect position = new Rect(
                    m_GuiScale * item.Position.x, Screen.height - item.Position.y * m_GuiScale,
                    m_GuiScale * 1, m_GuiScale * 2);
                Rect coords = item.Sprite.rect;
                float xPos = coords.x / item.Sprite.texture.width;
                float yPos = coords.y / item.Sprite.texture.height;
                float w = coords.width / item.Sprite.texture.width;
                float h = coords.height / item.Sprite.texture.height;
                coords.Set(xPos, yPos, w, h);
                GUI.DrawTextureWithTexCoords(position, item.Sprite.texture, coords);
                Debug.Log("Drawing Item\n" + position.ToString() + " " + coords.ToString());
            }
        }

        private void DrawInventoryBackground()
        {
            for (int y = 0; y <= m_InvSlotSize[1]; y++)
            {
                for (int x = 0; x <= m_InvSlotSize[0]; x++)
                {
                    Rect position = new Rect(
                        m_GuiScale * x, Screen.height - y * m_GuiScale,
                        m_GuiScale, m_GuiScale);
                    Rect coords = Res_SlotSprite.rect;
                    float xPos = coords.x / Res_SlotSprite.texture.width;
                    float yPos = coords.y / Res_SlotSprite.texture.height;
                    float w = coords.width / Res_SlotSprite.texture.width;
                    float h = coords.height / Res_SlotSprite.texture.height;
                    coords.Set(xPos, yPos, w, h);
                    GUI.DrawTextureWithTexCoords(position, Res_SlotSprite.texture, coords);
                    Debug.Log("Drawing Slot\n" + position.ToString() + " " + coords.ToString());
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
