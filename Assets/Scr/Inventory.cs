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

        

	    // Use this for initialization
	    void Start ()
        {

	    }
	
	    // Update is called once per frame
	    void Update () {
		
	    }

        public void OnSelect(int x, int y)
        {

        }
    }
}
