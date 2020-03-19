using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : Item
{
    public Transform[] raySources;
    public float damage1 = 10;
    public float damage2 = 20;
    float damage;

    bool isActive = false;
    List<Health> hittedHealthBoxes = new List<Health>();
    float useTimer = 0;

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            for (int i = 0; i < raySources.Length - 1; i++)
            {
                Vector3 start = raySources[i].position;
                for (int j = i + 1; j < raySources.Length; j++)
                {
                    Vector3 end = raySources[j].position;
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
            
            if (useTimer > 0)
                useTimer -= Time.deltaTime;
            else
                isActive = false;
        }
    }

    public override void Use(AnimationClip anim)
    {
        damage = damage1;
        if (!isActive)
        {
            isActive = true;
            hittedHealthBoxes.Clear();
            if (anim)
                useTimer = anim.length;
        }
    }
}
