﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class PositionTranslationObject
    {
        public Transform transform;
        public Vector3 target;
        public float smoothTime;
        public float maxSpeed;
        public float error;

        public bool isReached = false;

        public Vector3 currentVelocity;
        public Vector3 prevFramePos;

        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            maxSpeed = -1;
            error = 0.01f;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime, float maxSpeed)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            this.maxSpeed = maxSpeed;
            error = 0.01f;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime, float maxSpeed, float error)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            this.maxSpeed = maxSpeed;
            this.error = error;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
    }
    public static GameManager instance;

    List<PositionTranslationObject> positionTranslationObjects = new List<PositionTranslationObject>();

    private void Awake()
    {
        if (!instance) instance = this;
    }
    public Light mainLight;
    [HideInInspector] public float mainLightDefaultIntencity;
    public Camera cam;
    [HideInInspector]  public float camDefaultFieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        mainLightDefaultIntencity = mainLight.intensity;
        camDefaultFieldOfView = cam.fieldOfView;
    }

    void Update()
    {
        ObjectsPositionTranslations();
    }

    void ObjectsPositionTranslations()
    {
        for (int i = 0; i < positionTranslationObjects.Count; i++)
        {
            PositionTranslationObject o = positionTranslationObjects[i];
            if (o.transform && o.transform.gameObject && (o.target - o.transform.localPosition).magnitude > o.error && o.transform.localPosition == o.prevFramePos)
            {
                if (o.maxSpeed == -1)
                    o.transform.localPosition = Vector3.SmoothDamp(o.transform.localPosition, o.target, ref o.currentVelocity, o.smoothTime);
                else
                    o.transform.localPosition = Vector3.SmoothDamp(o.transform.localPosition, o.target, ref o.currentVelocity, o.smoothTime, o.maxSpeed);
                o.prevFramePos = o.transform.localPosition;
            }
            else
            {
                o.isReached = true;
                positionTranslationObjects.Remove(o);
                i--;
            }
        }
    }

    public void TranslatePositionObject(ref PositionTranslationObject o)
    {
        positionTranslationObjects.Add(o);
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime)
    {
        positionTranslationObjects.Add(new PositionTranslationObject(t, target, smoothTime));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed)
    {
        positionTranslationObjects.Add(new PositionTranslationObject(t, target, smoothTime, maxSpeed));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed, float error)
    {
        positionTranslationObjects.Add(new PositionTranslationObject(t, target, smoothTime, maxSpeed, error));
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
