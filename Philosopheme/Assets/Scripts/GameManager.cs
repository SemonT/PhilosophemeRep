using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public class PositionTranslationObject
    {
        public delegate void OnTranslationFinish(GameObject o);
        public Transform transform;
        public Vector3 target;
        public float smoothTime;
        public float maxSpeed;
        public float error;
        public float delay;
        public OnTranslationFinish Func;

        static public float maxSpeedDefault { get; } = -1f;
        static public float errorDefault { get; } = 0.01f;
        static public float delayDefault { get; } = 0f;

        public bool isReached = false;

        public Vector3 currentVelocity;
        public Vector3 prevFramePos;

        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            maxSpeed = maxSpeedDefault;
            error = errorDefault;
            delay = delayDefault;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime, float maxSpeed)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            this.maxSpeed = maxSpeed;
            error = errorDefault;
            delay = delayDefault;
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
            delay = delayDefault;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime, float maxSpeed, float error, float delay)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            this.maxSpeed = maxSpeed;
            this.error = error;
            this.delay = delay;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
        public PositionTranslationObject(Transform transform, Vector3 target, float smoothTime, float maxSpeed, float error, float delay, OnTranslationFinish Func)
        {
            this.transform = transform;
            this.target = target;
            this.smoothTime = smoothTime;
            this.maxSpeed = maxSpeed;
            this.error = error;
            this.delay = delay;
            this.Func = Func;
            currentVelocity = Vector3.zero;
            prevFramePos = transform.localPosition;
        }
    }
    [System.Serializable]
    public class MaterialPack
    {
        public string packName;
        public GameObject[] bulletHits;
        public GameObject[] bulletHoles;
        public GameObject[] bulletRicochets;
        public GameObject[] bulletHitEffects;
        public GameObject[] clubHits;
    }
    public static GameManager instance;

    public Camera cam;
    public MaterialPack[] materialPacks;

    List<PositionTranslationObject> positionTranslationObjects = new List<PositionTranslationObject>();
    [HideInInspector] public float camDefaultFieldOfView;

    private void Awake()
    {
        if (!instance) instance = this;
        camDefaultFieldOfView = cam.fieldOfView;
        MaterialModel.Initialise();
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
                if (o.delay > 0)
                {
                    o.delay -= Time.deltaTime;
                }
                else
                {
                    if (o.maxSpeed == -1)
                        o.transform.localPosition = Vector3.SmoothDamp(o.transform.localPosition, o.target, ref o.currentVelocity, o.smoothTime);
                    else
                        o.transform.localPosition = Vector3.SmoothDamp(o.transform.localPosition, o.target, ref o.currentVelocity, o.smoothTime, o.maxSpeed);
                    o.prevFramePos = o.transform.localPosition;
                }
            }
            else
            {
                o.Func?.Invoke(o.transform.gameObject);
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
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed, float error, float delay)
    {
        positionTranslationObjects.Add(new PositionTranslationObject(t, target, smoothTime, maxSpeed, error, delay));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed, float error, float delay, PositionTranslationObject.OnTranslationFinish Func)
    {
        positionTranslationObjects.Add(new PositionTranslationObject(t, target, smoothTime, maxSpeed, error, delay, Func));
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
