using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertialDamager : MonoBehaviour
{
    public float damagePerMps = 20f;

    private void OnCollisionEnter(Collision collision)
    {
        Health h = collision.gameObject.GetComponent<Health>();
        if (h)
        {
            h.HealthChange(-collision.relativeVelocity.magnitude * damagePerMps);
        }
    }
}
