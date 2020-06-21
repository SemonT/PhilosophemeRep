using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : Item
{
    [System.Serializable]
    public class Attack : Action
    {
        public float damage;
        public float minDamageSpeed = 3.5f;

        Vector3[] raySourcesPrevPoss;
        List<GameObject> hittedObjects;

        public override void Initialize()
        {
            hittedObjects = new List<GameObject>();
        }
        public override void OnStart()
        {
            ref Transform[] raySources = ref ((MeleWeapon)it).raySources;
            int length = raySources.Length;
            raySourcesPrevPoss = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                raySourcesPrevPoss[i] = raySources[i].position;
            }
        }
        public override void OnUpdate()
        {
            ref Transform[] raySources = ref ((MeleWeapon)it).raySources;

            for (int i = 0; i < raySources.Length; i++)
            {
                Vector3 start = raySources[i].position;
                float speed = (start - raySourcesPrevPoss[i]).magnitude / Time.deltaTime;
                if (speed > minDamageSpeed)
                {
                    for (int j = i + 1; j < raySources.Length + 1; j++)
                    {
                        Vector3 end;
                        if (j < raySources.Length)
                        {
                            end = raySources[j].position;
                        }
                        else
                        {
                            end = start;
                            start = raySourcesPrevPoss[i];
                        }
                        Vector3 dir = end - start;

                        Debug.DrawLine(start, end, Color.red, Time.deltaTime);

                        RaycastHit hit;
                        Physics.Raycast(start, dir, out hit, dir.magnitude);
                        if (hit.collider)
                        {
                            GameObject obj = hit.collider.gameObject;
                            if (hittedObjects.IndexOf(obj) == -1)
                            {
                                hittedObjects.Add(obj);
                                MaterialModel materialModel = obj.GetComponent<MaterialModel>();
                                if (!materialModel) materialModel = MaterialModel.defaultMaterialModel;
                                GameObject[] clubEffects = materialModel.pack.clubEffects;
                                if (clubEffects.Length > 0)
                                {
                                    GameObject o = Instantiate(
                                        clubEffects[Random.Range(0, clubEffects.Length)],
                                        hit.point,
                                        Quaternion.LookRotation(hit.normal)
                                    );
                                    o.transform.SetParent(hit.collider.gameObject.transform, true);
                                    o.GetComponent<ParticleSystem>().Play();
                                }
                                //if (materialModel.pack.clubHits.Length > 0)
                                //{
                                //    GameObject o = Instantiate(
                                //        materialModel.pack.clubHits[Random.Range(0, materialModel.pack.clubHits.Length)],
                                //        hit.point + hit.normal * 0.005f,
                                //        Quaternion.LookRotation(-hit.normal)
                                //    );
                                //    o.transform.SetParent(obj.transform, true);
                                //    o.transform.GetComponentInChildren<MeshRenderer>()?.gameObject.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)), Space.Self);
                                //}
                                Health health = obj.GetComponent<Health>();
                                if (!health)
                                {
                                    Health[] healths = obj.GetComponentsInParent<Health>();
                                    if (healths.Length > 0)
                                        health = healths[0];
                                }
                                if (health)
                                    health.HealthChange(-damage);
                            }
                        }
                    }
                }

                raySourcesPrevPoss[i] = raySources[i].position;
            }
        }
        public override void OnEnd()
        {
            hittedObjects.Clear();
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
