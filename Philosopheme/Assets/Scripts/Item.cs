using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    public string itemClassName = "undefined";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Interact()
    {
        Inventory inv = Inventory.instance;
        if (inv.isOpened)
        {
            inv.TakeItem(this);
        }
        else
        {
            inv.PickupItem(this);
        }
    }

    public void Use()
    {
        print("USING!");
    }
}
