﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Item
{
    [System.Serializable]
    public class Shoot : Action
    {
        public Transform bulletBasis;

        static float airDensity = 0.02f;
        static float powBase = 1.004f;
        static float maxRefractionMultiplier = 2;
        static float ricochetMinimalRatio = 1.1f;
        static float n = 2f;
        ParticleSystem fireEffect;
        Ammo currentBullet;
        float h;
        float x3;

        float D(float x, float k0)
        {
            return Mathf.Pow(x, n + 1) / (n + 1) - k0 * x;
        }
        public override void Initialize()
        {
            fireEffect = bulletBasis.GetComponent<ParticleSystem>();
            currentBullet = null;
        }

        enum HitType
        {
            bulletHit = 1,
            bulletHole,
            bulletRicochet,
            clubHit
        }
        struct HitEffectInfo
        {
            public Vector3 prevPos;
            public RaycastHit hit;
            public MaterialModel mm;
            public HitType ht;
            public Quaternion rotation;
            public HitEffectInfo(Vector3 prevPos, RaycastHit hit, Quaternion rotation, MaterialModel mm, HitType ht)
            {
                this.prevPos = prevPos;
                this.hit = hit;
                this.rotation = rotation;
                this.mm = mm;
                this.ht = ht;
            }
        }
        List<HitEffectInfo> postEffects = new List<HitEffectInfo>();
        void PostEffect()
        {
            while (postEffects.Count > 0)
            {
                HitEffectInfo hei = postEffects[0];

                Vector3 colliderVelocity = hei.hit.collider.transform.position - hei.prevPos;
                GameObject[] effects;
                switch (hei.ht)
                {
                    case HitType.bulletHit:
                        effects = hei.mm.pack.bulletHits;
                        break;
                    case HitType.bulletHole:
                        effects = hei.mm.pack.bulletHoles;
                        GameObject[] hitEffects = hei.mm.pack.bulletEffects;
                        if (hitEffects.Length > 0)
                        {
                            GameObject o = Instantiate(
                                hitEffects[Random.Range(0, hitEffects.Length)],
                                hei.hit.point + hei.hit.normal * 0.005f + colliderVelocity * 2,
                                Quaternion.LookRotation(-hei.hit.normal)
                            );
                            o.transform.SetParent(hei.hit.collider.gameObject.transform, true);
                            o.GetComponent<ParticleSystem>().Play();
                        }
                        break;
                    case HitType.bulletRicochet:
                        effects = hei.mm.pack.bulletRicochets;
                        break;
                    case HitType.clubHit:
                        effects = hei.mm.pack.clubHits;
                        break;
                    default:
                        effects = new GameObject[0];
                        break;
                }
                if (effects.Length > 0)
                {
                    GameObject o = Instantiate(
                        effects[Random.Range(0, effects.Length)],
                        hei.hit.point + hei.hit.normal * 0.005f + colliderVelocity * 2,
                        Quaternion.LookRotation(-hei.hit.normal)
                    );
                    o.transform.SetParent(hei.hit.collider.gameObject.transform, true);
                    if (hei.rotation == Quaternion.identity)
                    {
                        o.transform.GetComponentInChildren<MeshRenderer>()?.gameObject.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)), Space.Self);
                    }
                    else
                    {
                        MeshRenderer m = o.transform.GetComponentInChildren<MeshRenderer>();
                        if (m) m.gameObject.transform.rotation = hei.rotation;
                    }

                }


                postEffects.RemoveAt(0);
            }
        }
        void SimulateStep(Vector3 start, Vector3 dir, float k0)
        {
            RaycastHit hit1;
            Physics.Raycast(start, dir, out hit1, 10000f, ~0, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(start, hit1.point, Color.blue, 100000f);
            GameManager.instance.InvokeNextFrame(PostEffect);
            
            if (hit1.collider)
            {
                MaterialModel materialModel = hit1.collider.gameObject.GetComponent<MaterialModel>();
                if (!materialModel) materialModel = MaterialModel.defaultMaterialModel;
                float delta = hit1.distance;
                float k = k0 + delta * airDensity * 60f / currentBullet.penetration;
                Debug.DrawLine(start, hit1.point, Color.blue, 100000f);
                if (k < h)
                {
                    float refractionMultiplier = maxRefractionMultiplier - (maxRefractionMultiplier - 1) * Mathf.Pow(powBase, -materialModel.density / (airDensity * currentBullet.penetration));
                    float angle = Vector3.Angle(-dir, hit1.normal) * Mathf.PI / 180;
                    float newAngle = Mathf.Asin(Mathf.Sin(angle) / refractionMultiplier);
                    float ricochetRatio = (angle / newAngle) / refractionMultiplier;
                    if (ricochetRatio < ricochetMinimalRatio)
                    {
                        postEffects.Add(new HitEffectInfo(hit1.collider.transform.position, hit1, Quaternion.identity, materialModel, HitType.bulletHole));
                        Vector3 normalComponent = -hit1.normal.normalized * Mathf.Cos(newAngle);
                        Vector3 tangentComponent = Vector3.Cross(hit1.normal, Vector3.Cross(hit1.normal, -dir)).normalized * Mathf.Sin(newAngle);
                        //Debug.DrawLine(hit1.point, hit1.point + normalComponent, Color.blue, 100000f);
                        //Debug.DrawLine(hit1.point, hit1.point + tangentComponent, Color.green, 100000f);
                        dir = normalComponent + tangentComponent;
                        RaycastHit[] hits = Physics.RaycastAll(hit1.point + dir * 100f, -dir, 101f);
                        //Debug.DrawLine(hit1.point + dir * 100f, hit1.point, Color.blue, 100000f);
                        if (hits.Length > 0)
                        {
                            RaycastHit hit2 = hits[0];
                            for (int i = 0; i < hits.Length; i++)
                            {
                                if (hits[i].collider.gameObject == hit1.collider.gameObject)
                                {
                                    hit2 = hits[i];
                                    break;
                                }
                            }
                            delta = (hit1.point - hit2.point).magnitude;
                            k0 = k;
                            k = k0 + delta * materialModel.density * 60f / currentBullet.penetration;
                            //print(k + " of " + h);
                            if (k > h) k = h;

                            Health health = hit1.collider.gameObject.GetComponent<Health>();
                            if (!health)
                            {
                                Health[] healths = hit1.collider.gameObject.GetComponentsInParent<Health>();
                                if (healths.Length > 0)
                                    health = healths[0];
                            }
                            if (health)
                            {
                                float x1 = Mathf.Pow(k0, 1 / n);
                                float x2 = Mathf.Pow(k, 1 / n);
                                float S1 = D(x2, k0) - D(x1, k0);
                                float S2 = (x3 - x2) * (k - k0);
                                float S = S1 + S2;
                                float damage = S * 2;
                                health.HealthChange(-damage);
                            }
                            Debug.DrawLine(hit1.point, hit2.point, Color.red, 100000f);
                            if (k < h)
                            {
                                postEffects.Add(new HitEffectInfo(hit2.collider.transform.position, hit2, Quaternion.identity, materialModel, HitType.bulletHole));

                                angle = Vector3.Angle(dir, hit2.normal) * Mathf.PI / 180;
                                newAngle = Mathf.Asin(Mathf.Sin(angle) * refractionMultiplier);
                                normalComponent = hit2.normal.normalized * Mathf.Cos(newAngle);
                                tangentComponent = Vector3.Cross(hit2.normal, Vector3.Cross(dir, hit2.normal)).normalized * Mathf.Sin(newAngle);
                                //Debug.DrawLine(hit2.point, hit2.point + normalComponent, Color.blue, 100000f);
                                //Debug.DrawLine(hit2.point, hit2.point + tangentComponent, Color.green, 100000f);
                                dir = normalComponent + tangentComponent;
                                SimulateStep(hit2.point, dir, k);
                            }
                        }
                    }
                    else
                    {
                        Vector3 normalComponent = hit1.normal.normalized * Mathf.Cos(angle);
                        Vector3 tangentComponent = Vector3.Cross(hit1.normal, Vector3.Cross(hit1.normal, -dir)).normalized * Mathf.Sin(angle);
                        postEffects.Add(new HitEffectInfo(hit1.collider.transform.position, hit1, Quaternion.LookRotation(-normalComponent, Vector3.Cross(tangentComponent, normalComponent)), materialModel, HitType.bulletRicochet));
                        //Debug.DrawLine(hit1.point, hit1.point + normalComponent, Color.blue, 100000f);
                        //Debug.DrawLine(hit1.point, hit1.point + tangentComponent, Color.green, 100000f);
                        dir = normalComponent + tangentComponent;
                        //Debug.DrawLine(hit1.point + dir * 100f, hit1.point, Color.cyan, 100000f);
                        SimulateStep(hit1.point, dir, k);
                    }
                }
                else
                {
                    postEffects.Add(new HitEffectInfo(hit1.collider.transform.position, hit1, Quaternion.identity, materialModel, HitType.bulletHit));
                }
            }
            else
            {
                Debug.DrawLine(start, start + dir.normalized * 100f, Color.blue, 100000f);
            }
        }
        public override void OnStart()
        {
            currentBullet = ((RangedWeapon)it).jerkShutter.chamberAmmo;
            if (currentBullet)
            {
                Destroy(currentBullet.gameObject);
                ((RangedWeapon)it).jerkShutter.Jerk();
                float S = currentBullet.energy / 2;
                x3 = Mathf.Pow((n + 1) * S, 1 / (n + 1));
                h = Mathf.Pow(x3, n);

                fireEffect?.Play();
                SimulateStep(
                    bulletBasis.position,
                    bulletBasis.forward,
                    0);
            }
        }
        public override void OnUpdate() { }
        public override void OnEnd()
        {
            currentBullet = null;
        }
    }
    [System.Serializable]
    public class Reload : Action
    {
        public string caliber;
        public Ammo[] magazineAmmo;

        public bool Filter(Item item)
        {
            if (item is Ammo ammo)
            {
                if (ammo.caliber == caliber)
                    return true;
            }
            return false;
        }
        public override void Initialize() { }
        public override void OnStart()
        {
            for (int i = 0; i < magazineAmmo.Length; i++)
            {
                if (magazineAmmo[i] == null)
                {
                    magazineAmmo[i] = (Ammo)Inventory.instance.PullItem(Filter);
                    if (!magazineAmmo[i]) break;
                }
            }
        }
        public override void OnUpdate() { }
        public override void OnEnd(){ }
    }
    [System.Serializable]
    public class JerkShutter : Action
    {
        public Ammo chamberAmmo;
        public Transform extractorBasis;
        
        public void Jerk()
        {
            Ammo[] magazineAmmo = ((RangedWeapon)it).reload.magazineAmmo;
            if (chamberAmmo)
            {
                chamberAmmo.gameObject.SetActive(true);
                chamberAmmo.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                Collider collider = chamberAmmo.gameObject.GetComponent<Collider>();
                collider.enabled = true;
                collider.isTrigger = false;
                chamberAmmo.gameObject.transform.position = extractorBasis.position;
                chamberAmmo.gameObject.transform.rotation = Quaternion.LookRotation(extractorBasis.forward);
            }
            chamberAmmo = magazineAmmo[0];
            for (int i = 0; i < magazineAmmo.Length - 1; i++)
            {
                magazineAmmo[i] = magazineAmmo[i + 1];
            }
            magazineAmmo[magazineAmmo.Length - 1] = null;
        }
        public override void OnStart()
        {
            it.animator.SetBool("JerkTrigger", false);
        }
        public override void Initialize() { }
        public override void OnUpdate()
        {
            if (it.animator.GetBool("JerkTrigger"))
            {
                it.animator.SetBool("JerkTrigger", false);
                Jerk();
            }
        }
        public override void OnEnd() { }
    }
    [System.Serializable]
    public class ShowAmmo : Action
    {
        public Transform magazineBasis;
        public Transform visualisationBasis;
        public float visualisationPositionDelta = 0.01f;
        public float visualisationTimeDelta = 0.1f;

        JerkShutter jr;
        Reload re;
        public void HideAmmo(GameObject o)
        {
            o.SetActive(false);
            o.transform.SetParent(null);
        }
        public override void OnStart()
        {
            float delta = 0;
            jr = ((RangedWeapon)it).jerkShutter;
            re = ((RangedWeapon)it).reload;
            if (jr.chamberAmmo)
            {
                GameObject o = jr.chamberAmmo.gameObject;
                o.SetActive(true);
                o.transform.SetParent(visualisationBasis);
                o.transform.position = magazineBasis.position;
                GameManager.instance.TranslatePositionObject(o.transform, Vector3.zero, visualisationTimeDelta, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault / 10, re.magazineAmmo.Length * visualisationTimeDelta);
                o.transform.rotation = Quaternion.LookRotation(visualisationBasis.forward);
                delta += visualisationPositionDelta;
            }
            for (int i = 0; i < re.magazineAmmo.Length; i++)
            {
                GameObject o = re.magazineAmmo[i]?.gameObject;
                if (!o) break;
                o.SetActive(true);
                o.transform.SetParent(visualisationBasis);
                o.transform.position = magazineBasis.position;
                GameManager.instance.TranslatePositionObject(o.transform, -visualisationBasis.up * delta, visualisationTimeDelta, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault / 10, (re.magazineAmmo.Length - i - 1) * visualisationTimeDelta);
                //o.transform.localPosition = (AmmoVisualisationPointTransform.position - magazineTransform.position).normalized * delta;
                o.transform.rotation = Quaternion.LookRotation(visualisationBasis.forward);
                delta += visualisationPositionDelta;
            }
        }
        public override void Initialize() { }
        public override void OnUpdate() { }
        public override void OnEnd()
        {
            float timeDelta = 0;
            if (jr.chamberAmmo)
            {
                GameObject o = jr.chamberAmmo.gameObject;
                GameManager.instance.TranslatePositionObject(o.transform, magazineBasis.position - visualisationBasis.position, visualisationTimeDelta, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault, timeDelta, HideAmmo);
                timeDelta += visualisationTimeDelta;
            }
            for (int i = 0; i < re.magazineAmmo.Length; i++)
            {
                GameObject o = re.magazineAmmo[i]?.gameObject;
                if (!o) break;
                GameManager.instance.TranslatePositionObject(o.transform, magazineBasis.position - visualisationBasis.position, visualisationTimeDelta, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault, timeDelta, HideAmmo);
                timeDelta += visualisationTimeDelta;
            }
        }
    }
    public Shoot shoot;
    public JerkShutter jerkShutter;
    public Reload reload;
    public ShowAmmo showAmmo;


    public override void SetActions()
    {
        actions = new Action[] { shoot, reload, jerkShutter, showAmmo };
    }
    public override void Use()
    {
        if (mouse0KeyDown && !shoot.isActual && !jerkShutter.isActual && !reload.isActual && !showAmmo.isActual && jerkShutter.chamberAmmo) shoot.Start();
        if (mouse1KeyDown && !shoot.isActual && !jerkShutter.isActual && !reload.isActual && !showAmmo.isActual) jerkShutter.Start();
        if (rKeyUp && rKeyTimer < 0.5f && !shoot.isActual && !jerkShutter.isActual && !reload.isActual && !showAmmo.isActual) reload.Start();
        if (rKey && rKeyTimer >= 0.5f && !shoot.isActual && !jerkShutter.isActual && !reload.isActual && !showAmmo.isActual) showAmmo.Start();
    }
}
