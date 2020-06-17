using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public Vector3 closePos;
    public Vector3 openPos;
    public float time = 0.5f;
    public UnityEvent OnClose;
    public UnityEvent OnOpen;
    public float openHoldTime = 0;
    public float error = 0.001f;

    bool isOpen;
    int transition;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = closePos;
        isOpen = false;
        transition = 0;
    }

    public void Open()
    {
        if (transition <= 0)
        {
            transition = 1;
            if (openHoldTime == 0)
            {
                GameManager.instance.TranslatePositionObject(transform, openPos, time, GameManager.PositionTranslationObject.maxSpeedDefault, error, 0, OnOpenCall);
            }
            else
            {
                void C(GameObject o)
                {
                    OnOpenCall(o);
                    GameManager.instance.TranslatePositionObject(transform, closePos, time, GameManager.PositionTranslationObject.maxSpeedDefault, error, openHoldTime, OnCloseCall);
                }
                GameManager.instance.TranslatePositionObject(transform, openPos, time, GameManager.PositionTranslationObject.maxSpeedDefault, error, 0, C);
            }
        }
    }
    public void Close()
    {
        if (transition >= 0)
        {
            transition = -1;
            GameManager.instance.TranslatePositionObject(transform, closePos, time, GameManager.PositionTranslationObject.maxSpeedDefault, error, 0, OnCloseCall);
        }
    }

    void OnCloseCall(GameObject o)
    {
        isOpen = false;
        transition = 0;
        OnClose?.Invoke();
    }
    void OnOpenCall(GameObject o)
    {
        isOpen = true;
        transition = 0;
        OnOpen?.Invoke();
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
}
