using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Elevator : MonoBehaviour
{
    public Transform[] floors;
    public float speed = 0.5f;
    public float error = 0.01f;
    public UnityEvent onFloorReach;
    public UnityEvent onPrepare;
    public Transform box;

    int currentFloor;
    int targetFloor;
    bool isWaiting;

    // Start is called before the first frame update
    void Start()
    {
        currentFloor = 0;
        isWaiting = false;
        box.localPosition = floors[currentFloor].localPosition;
    }

    public void SetTargetFloor(int i)
    {
        targetFloor = i;
    }

    public void Prepare()
    {
        isWaiting = true;
        onPrepare?.Invoke();
    }
    public void Go()
    {
        if (isWaiting)
        {
            Player.instance.transform.parent = box.transform;
            isWaiting = false;
            Vector3 targetPos = floors[targetFloor].localPosition;
            GameManager.instance.TranslatePositionObject(box.transform, targetPos, (box.localPosition - targetPos).magnitude / speed, GameManager.PositionTranslationObject.maxSpeedDefault, error, 0, OnFloorReachCall);
        }
    }

    void OnFloorReachCall(GameObject o)
    {
        Player.instance.transform.parent = null;
        currentFloor = targetFloor;
        onFloorReach?.Invoke();
    }
}
