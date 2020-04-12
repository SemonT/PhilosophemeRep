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

    bool isOutside = true; // Каааастыль



    // Start is called before the first frame update
    void Start()
    {
        currentFloor = 1;
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
            if (!isOutside) Player.instance.transform.parent = box.transform; // кассстыль
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

        isOutside = false; // Каааааастыль
    }

    public void Call(int floor)
    {
        targetFloor = floor;
        Prepare();
    }
}
