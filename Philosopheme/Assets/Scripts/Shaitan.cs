using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaitan : MonoBehaviour
{
    public float forceMultiplier = 0.1f;
    public float minPeriod = 0.1f;
    public float maxPeriod = 5f;
    public float minDistance = 3f;
    public float maxDistance = 15f;
    float a;
    float b;
    float period;
    float timer;

    Transform playerTransform;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = Player.instance.transform;
        a = (maxPeriod - minPeriod) / (maxDistance - minDistance);
        b = minPeriod - minDistance * a;
        print("a: " + a + "\t b: " + b);
        timer = 0;
        period = 999999999f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > period)
        {
            rb.AddForce(RandomVector3(-forceMultiplier / period, forceMultiplier / period), ForceMode.Impulse);
            timer = 0;
            print("force (" + period + ")");
        }
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance >= maxDistance)
        {
            period = 999999999f;
        }
        else if (distance <= minDistance)
        {
            period = minPeriod;
        }
        else
        {
            period = a * distance + b;
        }
        print("distance: " + distance + "\tperiod:" + period);
    }

    public Vector3 RandomVector3(float min, float max)
    {
        return new Vector3(Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
    }
}
