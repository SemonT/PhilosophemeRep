using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Interactable
{
    public string className = "undefined";
    public Transform handle;
    public Transform forwardPointer;

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

    public abstract void Use(AnimationClip anim);
}
