using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inscription : MonoBehaviour
{
    static Camera cam;
    static Transform camTransform;
    static RectTransform spaceRectTransform;
    static float revealTime;
    static Vector2 padding;

    public static void Init(Camera cam, GameObject space, float revealTime, Vector2 padding)
    {
        Inscription.cam = cam;
        camTransform = cam.transform;
        spaceRectTransform = space.GetComponent<RectTransform>();
        Inscription.revealTime = revealTime;
        Inscription.padding = padding;
    }
    public static Vector2 WorldPositionToRectPos(Camera cam, RectTransform rect, Vector3 pos)
    {
        Vector2 viewportPos = cam.WorldToViewportPoint(pos);
        Vector2 centerPos = viewportPos - Vector2.one * 0.5f;
        Vector2 canvasPos = new Vector2(
            rect.sizeDelta.x * centerPos.x,
            rect.sizeDelta.y * centerPos.y
            );
        return canvasPos;
    }
    public static Rect ObjectToRect(Camera cam, RectTransform rectTransform, GameObject go)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        Vector3 cen = renderer.bounds.center;
        Vector3 ext = renderer.bounds.extents;

        Vector2[] extentPoints = new Vector2[8]
        {
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z)),
            WorldPositionToRectPos(cam, rectTransform, new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z))
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

    public RectTransform field;
    public GameObject cursor;
    public GameObject background;
    public TextMeshProUGUI textMesh;
    public GameObject targetObj { get; private set; }

    RectTransform cursorRectTransform;
    RectTransform backgroundRectTransform;
    Transform targetObjTransform;
    Bounds targetObjBounds;
    RectTransform rectTransform;
    Vector2 identityFieldDelta;
    float growDeltaMultiplier;
    float timer;
    bool remove;


    public Inscription SetTarget(GameObject target)
    {
        targetObj = target;
        targetObjTransform = targetObj.transform;

        Renderer targetObjRenderer = targetObj.GetComponent<Renderer>();
        targetObjBounds = targetObjRenderer.bounds;

        return this;
    }
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        growDeltaMultiplier = 1;
        timer = 0;
        remove = false;
        backgroundRectTransform = background.GetComponent<RectTransform>();
        cursorRectTransform = cursor.GetComponent<RectTransform>();

        rectTransform.SetParent(spaceRectTransform);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector2.zero;

        TMP_TextInfo textInfo = textMesh.GetTextInfo(textMesh.text);
        identityFieldDelta = new Vector2(
            2 * Mathf.Abs(textInfo.characterInfo[0].topLeft.x) + padding.x,
            2 * Mathf.Abs(textInfo.characterInfo[0].topLeft.y) + padding.y
        );
        
        backgroundRectTransform.sizeDelta = identityFieldDelta;
    }
    void LateUpdate()
    {
        if (remove)
        {
            Remove();
        }
        else
        {
            if (rectTransform && targetObjTransform)
            {
                Vector2 viewportPos = cam.WorldToViewportPoint(targetObjTransform.position);
                RaycastHit hit;
                Physics.Linecast(camTransform.position, targetObjTransform.position, out hit);
                if (!(
                    viewportPos.x > 0 && viewportPos.x < 1 &&
                    viewportPos.y > 0 && viewportPos.y < 1 &&
                    hit.collider.gameObject == targetObj
                    ))
                    if (timer > 0)
                    {
                        growDeltaMultiplier = -1;
                    }
                    else
                    {
                        remove = true;
                        return;
                    }
                float sizeMultiplier = 1;
                if (growDeltaMultiplier != 0)
                {
                    bool isObjForward = Vector3.Dot(camTransform.forward, targetObjTransform.position - camTransform.position) > 0;
                    timer += Time.deltaTime * growDeltaMultiplier;
                    if (timer < 0 || !isObjForward) { remove = true; return; }
                    if (timer >= revealTime) { timer = revealTime; growDeltaMultiplier = 0; }
                    sizeMultiplier = timer / revealTime;

                    rectTransform.localScale = Vector2.one * sizeMultiplier;
                }

                Rect targetRect = ObjectToRect(cam, spaceRectTransform, targetObj);
                Rect spaceRect = spaceRectTransform.rect;
                Vector2 targetRectPos = new Vector2(targetRect.x + targetRect.width / 2, targetRect.y + targetRect.height / 2);
                Vector2 targetPos = WorldPositionToRectPos(cam, spaceRectTransform, targetObjTransform.position);

                Rect topRect = new Rect(spaceRect.x, targetRect.y + targetRect.height, spaceRect.width, spaceRect.y + spaceRect.height - targetRect.y - targetRect.height);
                //Rect rightRect = new Rect(targetRect.x + targetRect.width, spaceRect.y, spaceRect.x + spaceRect.width - targetRect.x - targetRect.width, spaceRect.height);
                //Rect leftRect = new Rect(spaceRect.x, spaceRect.y, targetRect.x - spaceRect.x, spaceRect.height);
                //Rect bottomRect = new Rect(spaceRect.x, spaceRect.y, spaceRect.width, targetRect.y - spaceRect.y);

                //Rect[] rects = new Rect[4] { topRect, rightRect, bottomRect, leftRect };

                rectTransform.anchoredPosition = targetPos;

                Vector2 toPos = new Vector2(targetRectPos.x / 2, spaceRect.y + spaceRect.height - identityFieldDelta.y);
                if (topRect.height >= identityFieldDelta.y * 2)
                {
                    toPos.y = topRect.y + identityFieldDelta.y;
                }
                toPos -= targetPos;
                //for (int i = 0; i < rects.Length; i++)
                //{
                //    Rect r = rects[i];
                //    toPos.x = targetRectPos.x / 2;
                //    if (r.width >= identityFieldDelta.x && r.height >= identityFieldDelta.y)
                //    {
                //        if (r == topRect)
                //        {
                //            toPos.y = topRect.y + identityFieldDelta.y;
                //            //toPos = new Vector2(targetRectPos.x, topRect.y + identityFieldDelta.y);
                //        }

                //        break;
                //    }
                //}
                field.anchoredPosition = toPos;
                cursorRectTransform.anchoredPosition = new Vector2(toPos.x / 2, toPos.y / 2);
                cursorRectTransform.sizeDelta = new Vector2(cursorRectTransform.sizeDelta.x, toPos.magnitude);
                float a = Mathf.Atan2(toPos.y, toPos.x);
                cursorRectTransform.localRotation = Quaternion.Euler(0, 0, a * 180 / Mathf.PI - 90);

                //Rect r = bottomRect;
                //cursorRectTransform.anchoredPosition = new Vector2(r.x + r.width / 2, r.y + r.height / 2); ;
                //cursorRectTransform.sizeDelta = new Vector2(r.width, r.height);

            }
            else if (timer > 0)
            {
                remove = true;
            }
        }
    }
    public void Collapse()
    {
        growDeltaMultiplier = -1;
    }
    public void Remove()
    {
        Destroy(gameObject);
    }
}
