using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Item
{
    [System.Serializable]
    public class Shoot : Action
    {
        public float damage;

        bool shootMade;
        public override void OnStart()
        {
            shootMade = false;
        }
        public override void OnUpdate()
        {
            if (!shootMade)
            {
                shootMade = true;


            }
        }
        public override void OnEnd() { }
    }
    [System.Serializable]
    public class Reload : Action
    {
        public override void OnStart() { }
        public override void OnUpdate() { }
        public override void OnEnd() { }
    }
    [System.Serializable]
    public class Aim : Action
    {
        public override void OnStart() { }
        public override void OnUpdate() { }
        public override void OnEnd() { }
    }

    public Transform bulletStartPoint;
    public Transform bulletDirectionPoint;
    public Shoot shoot;
    public Reload reload;
    public Reload aim;

    Action currentAction;
    bool isActive = false;
    float useTimer = 0;

    public override void SetActions()
    {
        actions = new Action[] { shoot, reload, aim };
    }
    public override void Use(bool mouse0Key, bool mouse1Key, bool rKey)
    {
        if (mouse0Key && !shoot.isActual) shoot.Start();
    }
}
