using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : Item
{
    [System.Serializable]
    public class ThrowItem : Action
    {
        public float damage = 50f;
        public float force = 6f;
        public float lifeTime = 25f;
        public float lifeAfterCollide = 1.5f;

        public override void Initialize()
        {
            
        }
        public override void OnStart()
        {

        }
        public override void OnUpdate()
        {

        }
        public override void OnEnd()
        {
            //GameObject ball = Instantiate(fireball, ((Throwable)it).transform.position, ((Throwable)it).transform.rotation);
            Inventory.instance.ReleaseCurrentItem();
            Rigidbody rb = ((Throwable)it).GetComponent<Rigidbody>();
            rb.useGravity = false;
            Vector3 target = GameManager.instance.cam.transform.forward;
            rb.AddForce(target * force, ForceMode.Impulse);
            Fireball fb = ((Throwable)it).gameObject.AddComponent<Fireball>();
            fb.damage = damage;
            fb.lifeAfterCollide = lifeAfterCollide;
            fb.lifeTime = lifeTime;
            ((Throwable)it).isInteractable = false;
            //Destroy(((Throwable)it).gameObject);
            bool ItemFilter(Item i)
            {
                if (i is Throwable thr)
                {
                    return true;
                }
                return false;
            }
            Item nextThrowable = Inventory.instance.FindItem(ItemFilter);
            if (nextThrowable)
            {
                Inventory.instance.TakeItem(nextThrowable);
            }
        }
    }
    public ThrowItem throwItem;

    public override void SetActions()
    {
        actions = new Action[] { throwItem };
    }
    public override void Use()
    {
        if (mouse0KeyDown && !throwItem.isActual) throwItem.Start();
    }
}
