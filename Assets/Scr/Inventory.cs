using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoardIt.Assets
{
    public class Inventory : MonoBehaviour
    {
        public RectTransform Ref_InventoryGui;
        public Image Prefab_Image;
        public List<Item> m_Items = new List<Item>();
        public List<Image> m_ItemDisplay = new List<Image>();

	    // Use this for initialization
	    void Start ()
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                //float multiplier = Ref_InventoryGui.sizeDelta.x/5;
                //var rect = m_Items[i].Bounds;
                //rect.position *= multiplier;
                //rect.size *= multiplier;
                //m_ItemDisplay.Add(Instantiate(Prefab_Image));
                //m_ItemDisplay[i].sprite = m_Items[i].Sprite;
                //m_ItemDisplay[i].transform.parent = Ref_InventoryGui.transform;
                //m_ItemDisplay[i].rectTransform.rect.Set(rect.x, rect.y, rect.width, rect.height);
            }
	    }
	
	    // Update is called once per frame
	    void Update () {
		
	    }

        public void OnSelect(int x, int y)
        {

        }
    }
}
