using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using HoardIt.Assets;
using UnityEngine.EventSystems;

namespace HoardIt.UI
{
    class InventorySlot : Selectable
    {
        public int m_X, m_Y;
        public Inventory Ref_Inventory;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Ref_Inventory.OnSelect(m_X, m_Y);
        }
    }
}
