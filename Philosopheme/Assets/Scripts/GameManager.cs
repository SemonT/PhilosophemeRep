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
        public GameObject[] bulletEffects;
        public GameObject[] clubHits;
        public GameObject[] clubEffects;
    }
    public delegate bool VisibilityFilter(GameObject go);

    public List<GameObject> creatures = new List<GameObject>();

    public static GameManager instance;


    public delegate void Function();
    public void InvokeNextFrame(Function function)
    {
        try
        {
            StartCoroutine(_InvokeNextFrame(function));
        }
        catch
        {
            Debug.Log("Trying to invoke " + function.ToString() + " but it doesnt seem to exist");
        }
    }
    private IEnumerator _InvokeNextFrame(Function function)
    {
        yield return null;
        function();
    }

    public static bool DefaultVisibilityFilter(GameObject go)
    {
        Renderer r = go.GetComponent<Renderer>();
        if (r && r.material.shader.name == "Custom/Glass1") return true;
        return false;
    }
    public static bool CheckForLinearVisibility(GameObject go1, GameObject go2, float maxDistance, int layerMask, QueryTriggerInteraction q, VisibilityFilter f)
    {
        Vector3 pos1 = go1.transform.position;
        Vector3 pos2 = go2.transform.position;
        Vector3 dir = pos2 - pos1;
        float distance = dir.magnitude;
        if (distance > maxDistance) return false;

        if (go2.name == "Eye1")
        {
            print("Eye1");
        }

        bool isVisible = true;
        RaycastHit[] hits = Physics.RaycastAll(pos1, dir, distance, layerMask, q);
        //Debug.DrawRay(pos1, dir.normalized * distance, Color.red, Time.deltaTime);
        if (hits.Length == 0) return true;

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject go = hits[i].collider.gameObject;
            if (
                go == go1 || go == go2 ||
                f != null && f.Invoke(go)
                ) continue;
            isVisible = false;
            break;
        }
        return isVisible;
    }
    public static Vector2 WorldPositionToUIPos(Camera cam, RectTransform rect, Vector3 pos)
    {
        Vector2 viewportPos = cam.WorldToViewportPoint(pos);
        Vector2 centerPos = viewportPos - Vector2.one * 0.5f;
        Vector2 canvasPos = new Vector2(
            rect.sizeDelta.x * centerPos.x,
            rect.sizeDelta.y * centerPos.y
            );
        return canvasPos;
    }
    public static Rect WorldBoundsToUIRect(Camera cam, RectTransform rectTransform, Bounds b)
    {
        Vector3 cen = b.center;
        Vector3 ext = b.extents;

        Vector2[] extentPoints = new Vector2[8]
        {
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z)),
            WorldPositionToUIPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z))
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }
    public static Rect WorldBoundsToUIRectOverall(Camera cam, RectTransform rectTransform, Bounds[] boundsArray)
    {
        Rect[] rects = new Rect[boundsArray.Length];
        for (int i = 0; i < rects.Length; i++)
        {
            rects[i] = WorldBoundsToUIRect(cam, rectTransform, boundsArray[i]);
        }
        Rect overallRect = rects[0];
        foreach (Rect r in rects)
        {
            overallRect.min = Vector2.Min(overallRect.min, r.min);
            overallRect.max = Vector2.Max(overallRect.max, r.max);
        }
        return overallRect;
    }

    public Camera cam;
    public Light[] mainLights;
    public MaterialPack[] materialPacks;

    List<PositionTranslationObject> positionTranslationObjects = new List<PositionTranslationObject>();
    [HideInInspector] public float camDefaultFieldOfView;

    public static void TurnOnMainLights()
    {
        for (int i = 0; i < instance.mainLights.Length; i++) instance.mainLights[i].enabled = true;
    }
    public static void TurnOffMainLights()
    {
        for (int i = 0; i < instance.mainLights.Length; i++) instance.mainLights[i].enabled = false;
    }

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

    void TranslatePositionObject(PositionTranslationObject o)
    {
        for (int i = 0; i < positionTranslationObjects.Count; i++)
        {
            Transform t = positionTranslationObjects[i].transform;
            if (o.transform == t)
            {
                positionTranslationObjects.RemoveAt(i);
                break;
            }
        }
        positionTranslationObjects.Add(o);
    }
    public void TranslatePositionObject(ref PositionTranslationObject o)
    {
        for (int i = 0; i < positionTranslationObjects.Count; i++)
        {
            Transform t = positionTranslationObjects[i].transform;
            if (o.transform == t)
            {
                positionTranslationObjects.RemoveAt(i);
                break;
            }
        }
        positionTranslationObjects.Add(o);
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime)
    {
        TranslatePositionObject(new PositionTranslationObject(t, target, smoothTime));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed)
    {
        TranslatePositionObject(new PositionTranslationObject(t, target, smoothTime, maxSpeed));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed, float error)
    {
        TranslatePositionObject(new PositionTranslationObject(t, target, smoothTime, maxSpeed, error));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed, float error, float delay)
    {
        TranslatePositionObject(new PositionTranslationObject(t, target, smoothTime, maxSpeed, error, delay));
    }
    public void TranslatePositionObject(Transform t, Vector3 target, float smoothTime, float maxSpeed, float error, float delay, PositionTranslationObject.OnTranslationFinish Func)
    {
        TranslatePositionObject(new PositionTranslationObject(t, target, smoothTime, maxSpeed, error, delay, Func));
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
