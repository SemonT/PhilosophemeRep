using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{

    public float lifeTime = 25f;
    public float damage = 9f;
    public float lifeAfterCollide = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        lifeTime -= Time.deltaTime;
        if (lifeTime < 0) Destroy(transform.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.GetComponent<Health>()?.HealthChange(-damage);
        Destroy(transform.gameObject, lifeAfterCollide);
    }

}
