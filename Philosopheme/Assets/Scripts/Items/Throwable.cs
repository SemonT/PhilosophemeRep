using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : Item
{
    [System.Serializable]
    public class ThrowItem : Action
    {
        public GameObject fireball;
        public float damage = 50f;
        public float force = 100f;

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
            GameObject ball = Instantiate(fireball, ((Throwable)it).transform.position, ((Throwable)it).transform.rotation);
            Rigidbody bullet = ball.GetComponent<Rigidbody>();
            Vector3 target = GameManager.instance.cam.transform.forward;
            bullet.AddForce(target * force);
            Inventory.instance.ReleaseCurrentItem();
            Destroy(((Throwable)it).gameObject);
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
