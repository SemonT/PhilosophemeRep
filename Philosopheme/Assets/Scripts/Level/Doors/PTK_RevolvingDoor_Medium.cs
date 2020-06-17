using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_RevolvingDoor_Medium : MonoBehaviour
{
    float circleTime = 20f;

    float angleSpeed;

    // Start is called before the first frame update
    void Start()
    {
        angleSpeed = 2 * Mathf.PI / circleTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation *= new Quaternion(0, 1, 0, angleSpeed * Time.deltaTime);
    }
}
