using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Item
{
    public string caliber;
    public float energy;
    public float penetration = 1;
    
    public override void SetActions() { }
    public override void Use() { }
}
