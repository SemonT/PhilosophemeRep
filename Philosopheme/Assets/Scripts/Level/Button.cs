using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    public UnityEvent onPress;
    public float pressTimer = 0.25f;
    public float error = 0.0001f;
    public GameObject model;
    public Vector3 unpressedModelPos;
    public Vector3 pressedModelPos;

    Transform modelTransform;

    // Start is called before the first frame update
    void Start()
    {
        modelTransform = model.transform;
        unpressedModelPos = modelTransform.localPosition;
        pressedModelPos = -unpressedModelPos;
    }

    void Press()
    {
        GameManager.instance.TranslatePositionObject(modelTransform, pressedModelPos, pressTimer, GameManager.PositionTranslationObject.maxSpeedDefault, error, GameManager.PositionTranslationObject.delayDefault, Unpress);
    }

    void Unpress(object o)
    {
        GameManager.instance.TranslatePositionObject(modelTransform, unpressedModelPos, pressTimer, GameManager.PositionTranslationObject.maxSpeedDefault, error);
    }

    public override void Interact()
    {
        Press();
        onPress?.Invoke();
    }
}
