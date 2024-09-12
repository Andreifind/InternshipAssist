using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public int layers;
    public int width;
    private bool allMatch = true;

    public void CheckAndDestroyItems()
    {
        List<Slot> layer0Slots = new List<Slot>();
        List<Slot> higherLayerSlots = new List<Slot>();

        foreach (Transform child in transform)
        {
            Slot slot = child.GetComponent<Slot>();
            if (slot != null)
            {
                if (slot.Layer == 0)
                {
                    layer0Slots.Add(slot); 
                }
                else if (slot.Layer > 0)
                {
                    higherLayerSlots.Add(slot); 
                }
            }
        }

        if (layer0Slots.Count == 3)
        {
            ItemType firstItemType = null;
            allMatch = true;
            bool allEmpty = true; 

            foreach (Slot slot in layer0Slots)
            {
                if (slot.IsHoldingItem && slot.GetComponentInChildren<ItemType>() != null)
                {
                    allEmpty = false;

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
                }
            }

            bool hasHigherLayerSlots = higherLayerSlots.Count > 0;

            if (hasHigherLayerSlots && allEmpty)
            {
                foreach (Slot slot in layer0Slots)
                {
                    Destroy(slot.gameObject);
                }

                foreach (Transform child in transform)
                {
                    Slot slot = child.GetComponent<Slot>();
                    if (slot != null && slot.Layer > 0)
                    {
                        slot.Layer -= 1;
                        slot.RefreshCollider();
                    }
                }
            }

            else if (allMatch && hasHigherLayerSlots)
            {
                foreach (Slot slot in layer0Slots)
                {
                    Destroy(slot.gameObject);
                }

                foreach (Transform child in transform)
                {
                    Slot slot = child.GetComponent<Slot>();
                    if (slot != null && slot.Layer > 0)
                    {
                        slot.Layer -= 1;
                        slot.RefreshCollider();

                        ItemType item = slot.GetComponentInChildren<ItemType>();
                        if (item != null)
                        {
                            item.ChangeColorTint(slot.Layer);
                            item.ChangeOrderInLayer(slot.Layer);
                        }
                    }
                }
                this.GetComponentInParent<LevelManager>().MatchedItems();
                this.GetComponentInParent<LevelManager>().Combo.AddCombo();
                this.GetComponentInParent<LevelManager>().SpawnPopup(transform.position);
            }


            else if (allMatch && !hasHigherLayerSlots)
            {
                foreach (Slot slot in layer0Slots)
                {
                    ItemType itemType = slot.GetComponentInChildren<ItemType>();
                    if (itemType != null)
                    {
                        Destroy(itemType.gameObject);
                        slot.HoldTheItem(false); 
                    }
                }
                this.GetComponentInParent<LevelManager>().MatchedItems();
                this.GetComponentInParent<LevelManager>().Combo.AddCombo();
                this.GetComponentInParent<LevelManager>().SpawnPopup(transform.position);
            }
        }
    }
}
