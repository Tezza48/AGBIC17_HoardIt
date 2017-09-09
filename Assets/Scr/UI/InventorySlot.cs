using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using HoardIt.Assets;
using UnityEngine.EventSystems;

namespace HoardIt.UI
{
    public class InventorySlot : Selectable
    {
        public int m_X, m_Y;
        public InventoryUI Ref_InventoryUI;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Ref_InventoryUI.OnSelect(m_X, m_Y);

        }

        public void Init(InventoryUI inv, int x, int y)
        {
            Ref_InventoryUI = inv;
            m_X = x;
            m_Y = y;
        }
    }
}
