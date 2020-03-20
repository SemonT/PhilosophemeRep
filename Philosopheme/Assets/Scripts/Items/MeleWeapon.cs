using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : Item
{
    [System.Serializable]
    public class Attack : Action
    {
        public float damage;
    }

    public Transform[] raySources;
    public Attack[] attacks;

    Attack currentAttack;
    bool isActive = false;
    List<Health> hittedHealthBoxes = new List<Health>();
    float useTimer = 0;

    void Start()
    {
        isUsable = true;
        actions = attacks;
    }

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
                        health.HealthChange(-currentAttack.damage);
                        hittedHealthBoxes.Add(health);
                    }
                }
            }
            
            if (useTimer > 0)
                useTimer -= Time.deltaTime;
            else
            {
                isActive = false;
                isUsable = true;
            }
        }
    }

    public override void Use(AnimationClip anim)
    {
        if (!isActive)
        {
            currentAttack = null;
            foreach (Attack a in actions)
            {
                if (a.animationName == anim.name)
                {
                    currentAttack = a;
                    break;
                }
            }
            if (currentAttack != null)
            {
                hittedHealthBoxes.Clear();
                useTimer = anim.length;
                isActive = true;
                isUsable = false;
            }
        }
    }
}
