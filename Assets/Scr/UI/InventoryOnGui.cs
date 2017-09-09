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
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
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
                    GUI.DrawTextureWithTexCoords(position, Res_SlotSprite.texture, new Rect(xPos, yPos, w, h));
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
