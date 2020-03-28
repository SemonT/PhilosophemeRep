using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : Item
{
    [System.Serializable]
    public class Attack : Action
    {
        public float damage;

        List<GameObject> hittedObjects;

        public override void Initialize()
        {
            hittedObjects = new List<GameObject>();
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
                    if (hit.collider)
                    {
                        GameObject obj = hit.collider.gameObject;
                        if (hittedObjects.IndexOf(obj) == -1)
                        {
                            hittedObjects.Add(obj);
                            MaterialModel materialModel = obj.GetComponent<MaterialModel>();
                            if (!materialModel) materialModel = MaterialModel.defaultMaterialModel;
                            if (materialModel.pack.clubHits.Length > 0)
                            {
                                GameObject o = Instantiate(
                                    materialModel.pack.clubHits[Random.Range(0, materialModel.pack.clubHits.Length)],
                                    hit.point + hit.normal * 0.005f,
                                    Quaternion.LookRotation(-hit.normal)
                                );
                                o.transform.SetParent(obj.transform, true);
                                o.transform.GetComponentInChildren<MeshRenderer>()?.gameObject.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)), Space.Self);
                            }
                            obj.GetComponent<Health>()?.HealthChange(-damage);
                        }
                    }
                }
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
