using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public int layers; 
    public int width;

    public void CheckAndDestroyItems()
    {
        // Collect all slots with Layer 0
        List<Slot> layer1Slots = new List<Slot>();
        foreach (Transform child in transform)
        {
            Slot slot = child.GetComponent<Slot>();
            if (slot != null && slot.Layer == 0)
            {
                layer1Slots.Add(slot);
            }
        }

        if (layer1Slots.Count == 3)
        {
            ItemType firstItemType = null;
            bool allMatch = true;

            foreach (Slot slot in layer1Slots)
            {
                if (slot.IsHoldingItem && slot.GetComponentInChildren<ItemType>() != null)
                {
                    ItemType currentItemType = slot.GetComponentInChildren<ItemType>();
                    if (firstItemType == null)
                    {
                        firstItemType = currentItemType;
                    }
                    else if (firstItemType.Type != currentItemType.Type)
                    {
                        allMatch = false;
                        break;
                    }
                }
                else
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                foreach (Slot slot in layer1Slots)
                {
                    ItemType itemType = slot.GetComponentInChildren<ItemType>();
                    if (itemType != null)
                    {
                        Destroy(itemType.gameObject);
                        slot.HoldTheItem(false); 
                    }
                }

                foreach (Transform child in transform)
                {
                    Slot slot = child.GetComponent<Slot>();
                    ItemType itemType = slot.GetComponentInChildren<ItemType>();
                    if (slot != null && slot.Layer > 0)
                    {
                        slot.Layer -= 1; 
                        itemType.ChangeColorTint(slot.Layer);
                        itemType.ChangeOrderInLayer(slot.Layer);
                    }
                }
            }
        }
    }




}
