using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_Door_Medium : Interactable
{
    public Transform hinge;

    float angle = 90;
    float time = 1f;
    bool isOpened = false;

    float angleSpeed;
    float currentAngle;
    float multiplier;

    public void Open()
    {
        //GameManager.instance.TranslatePositionObject(door, openedLocalPos, time, GameManager.PositionTranslationObject.maxSpeedDefault, error);
        //transform.RotateAround(hinge.position, Vector3.up, angle);
        multiplier = 1;
    }
    public void Close()
    {
        //GameManager.instance.TranslatePositionObject(door, closedLocalPos, time, GameManager.PositionTranslationObject.maxSpeedDefault, error);
        //transform.RotateAround(hinge.position, Vector3.up, -angle);
        multiplier = -1;
    }
    // Start is called before the first frame update
    void Start()
    {
        angleSpeed = angle / time;
        currentAngle = 0;
        multiplier = 0;
    }

    public override void Interact()
    {
        if (isOpened)
        {
            isOpened = false;
            Close();
        }
        else
        {
            isOpened = true;
            Open();
        }
    }

    void Update()
    {
        if (multiplier != 0)
        {
            if ((currentAngle > 0 || multiplier > 0) && (currentAngle < angle || multiplier < 0))
            {
                float a = angleSpeed * multiplier * Time.deltaTime;
                currentAngle += a;
                transform.RotateAround(hinge.position, Vector3.up, a);
            }
            else
            {
                multiplier = 0;
            }
        }
    }
}
