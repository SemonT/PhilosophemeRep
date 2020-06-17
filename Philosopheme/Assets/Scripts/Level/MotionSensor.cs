using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MotionSensor : MonoBehaviour
{
    public UnityEvent onDetect;
    public UnityEvent onLose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            onDetect?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            onLose?.Invoke();
        }
    }
}
