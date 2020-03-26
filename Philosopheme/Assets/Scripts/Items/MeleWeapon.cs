using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : Item
{
    [System.Serializable]
    public class Attack : Action
    {
        public float damage;

        List<Health> hittedHealthBoxes;

        public override void Initialize()
        {
            hittedHealthBoxes = new List<Health>();
        }
        public override void OnStart() { }
        public override void OnUpdate()
        {
            for (int i = 0; i < ((MeleWeapon)it).raySources.Length - 1; i++)
            {
                Vector3 start = ((MeleWeapon)it).raySources[i].position;
                for (int j = i + 1; j < ((MeleWeapon)it).raySources.Length; j++)
                {
                    Vector3 end = ((MeleWeapon)it).raySources[j].position;
                    Vector3 dir = end - start;
                    RaycastHit hit;
                    Physics.Raycast(start, dir, out hit, dir.magnitude);
                    Health health = hit.collider?.gameObject.GetComponent<Health>();
                    if (health && hittedHealthBoxes.IndexOf(health) == -1)
                    {
                        health.HealthChange(-damage);
                        hittedHealthBoxes.Add(health);
                    }
                }
            }
        }
        public override void OnEnd()
        {
            hittedHealthBoxes.Clear();
        }
    }

    public Transform[] raySources;
    public Attack attack1;
    public Attack attack2;

    public override void SetActions()
    {
        actions = new Action[] { attack1, attack2 };
    }

    public override void Use()
    {
        if (mouse0KeyDown && !attack1.isActual && !attack2.isActual) attack1.Start();
        if (mouse1KeyDown && !attack1.isActual && !attack2.isActual) attack2.Start();
    }
}
