using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inscription : MonoBehaviour
{
    string texT;
    Color coloR;

    static GameObject markPrefab;
    static Camera cam;
    static Transform camTransform;
    public static RectTransform spaceRectTransform;
    static float maxDistance;
    static float revealTime;
    static Vector2 padding;

    public static void Init(GameObject markPrefab, Camera cam, GameObject space, float maxDistance, float revealTime, Vector2 padding)
    {
        Inscription.markPrefab = markPrefab;
        Inscription.cam = cam;
        camTransform = cam.transform;
        spaceRectTransform = space.GetComponent<RectTransform>();
        Inscription.maxDistance = maxDistance;
        Inscription.revealTime = revealTime;
        Inscription.padding = padding;
    }

    public bool isQuestion;
    public bool isReply;
    public string text;
    public Color color = Color.white;
    public Transform inscriptionPoint;

    Renderer rendererrr;
    Collider colliderrrr;
    Mesh mesh;

    Mark mark;
    RectTransform markRectTransform;
    RectTransform fieldRectTransform;
    RectTransform cursorRectTransform;
    RectTransform backgroundRectTransform;
    TextMeshProUGUI textMesh;
    List<Image> images;

    Vector2 identityFieldDelta;
    float growDeltaMultiplier;
    float timer;
    public bool IsActive { get; private set; }
    bool hasBeenReplied;
    int counter;

    void Start()
    {
        rendererrr = GetComponent<Renderer>();
        if (!rendererrr)
        {
            colliderrrr = GetComponent<Collider>();
            if (!colliderrrr)
            {
                mesh = GetComponent<Mesh>();
                if (!mesh)
                {
                    rendererrr = GetComponentInChildren<Renderer>(false);
                    if (!rendererrr)
                    {
                        colliderrrr = GetComponentInChildren<Collider>(true);
                    }
                }
            }
        }

        mark = Instantiate(markPrefab).GetComponent<Mark>();
        markRectTransform = mark.GetComponent<RectTransform>();
        fieldRectTransform = mark.field.GetComponent<RectTransform>();
        cursorRectTransform = mark.cursor.GetComponent<RectTransform>();
        backgroundRectTransform = mark.background.GetComponent<RectTransform>();
        textMesh = mark.text.GetComponent<TextMeshProUGUI>();
        images = new List<Image>(mark.field.GetComponentsInChildren<Image>());
        images.Add(mark.cursor.GetComponent<Image>());

        growDeltaMultiplier = 0;
        timer = 0;
        hasBeenReplied = false;
        counter = -1;

        markRectTransform.SetParent(spaceRectTransform);
        SetHidden();

        if (!isQuestion)
        {
            if (isReply)
            {
                SetColor(color);
            }
            else
            {
                SetColor(color);
                SetText(text);
            }
        }
    }
    void LateUpdate()
    {
        if (IsActive)
        {
            Vector2 viewportPos = cam.WorldToViewportPoint(transform.position);
            if (!(
                viewportPos.x > 0 && viewportPos.x < 1 &&
                viewportPos.y > 0 && viewportPos.y < 1 &&
                InscriptionManager.CheckForVisibility(gameObject)
                ))
                if (timer > 0)
                {
                    growDeltaMultiplier = -1;
                }
                else
                {
                    SetHidden();
                    return;
                }
            float sizeMultiplier = 1;
            if (growDeltaMultiplier != 0)
            {
                bool isObjForward = Vector3.Dot(camTransform.forward, transform.position - camTransform.position) > 0;
                timer += Time.deltaTime * growDeltaMultiplier;
                if (timer < 0 || !isObjForward) { SetHidden(); return; }
                if (timer >= revealTime) { timer = revealTime; growDeltaMultiplier = 0; }
                sizeMultiplier = timer / revealTime;

                markRectTransform.localScale = Vector2.one * sizeMultiplier;
            }

            Collider[] colliderrrrs = GetComponentsInChildren<Collider>();
            Bounds[] boundsArray;
            if (colliderrrrs.Length > 0)
            {
                boundsArray = new Bounds[colliderrrrs.Length];
                for (int i = 0; i < boundsArray.Length; i++)
                {
                    boundsArray[i] = colliderrrrs[i].bounds;
                }
            }
            else if (rendererrr) boundsArray = new Bounds[1] { rendererrr.bounds };
            else if (colliderrrr) boundsArray = new Bounds[1] { colliderrrr.bounds };
            else if (mesh) boundsArray = new Bounds[1] { mesh.bounds };
            else boundsArray = new Bounds[1] { new Bounds(transform.position, Vector3.one) };

            Rect targetRect = GameManager.WorldBoundsToUIRectOverall(cam, spaceRectTransform, boundsArray);
            //Rect targetRect = GameManager.WorldBoundsToUIRect(cam, spaceRectTransform, objBounds);
            Rect spaceRect = spaceRectTransform.rect;
            Vector2 targetRectPos = new Vector2(targetRect.x + targetRect.width / 2, targetRect.y + targetRect.height / 2);
            //Vector2 targetPos = GameManager.WorldPositionToUIPos(cam, spaceRectTransform, transform.position);
            Vector2 targetPos;
            if (inscriptionPoint)
            {
                targetPos = GameManager.WorldPositionToUIPos(cam, spaceRectTransform, inscriptionPoint.position);
            }
            else
            {
                targetPos = targetRectPos;
            }

            Rect topRect = new Rect(spaceRect.x, targetRect.y + targetRect.height, spaceRect.width, spaceRect.y + spaceRect.height - targetRect.y - targetRect.height);
            //Rect rightRect = new Rect(targetRect.x + targetRect.width, spaceRect.y, spaceRect.x + spaceRect.width - targetRect.x - targetRect.width, spaceRect.height);
            //Rect leftRect = new Rect(spaceRect.x, spaceRect.y, targetRect.x - spaceRect.x, spaceRect.height);
            //Rect bottomRect = new Rect(spaceRect.x, spaceRect.y, spaceRect.width, targetRect.y - spaceRect.y);

            //Rect[] rects = new Rect[4] { topRect, rightRect, bottomRect, leftRect };

            markRectTransform.anchoredPosition = targetPos;

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
            fieldRectTransform.anchoredPosition = toPos;
            cursorRectTransform.anchoredPosition = new Vector2(toPos.x / 2, toPos.y / 2);
            cursorRectTransform.sizeDelta = new Vector2(cursorRectTransform.sizeDelta.x, toPos.magnitude);
            float a = Mathf.Atan2(toPos.y, toPos.x);
            cursorRectTransform.localRotation = Quaternion.Euler(0, 0, a * 180 / Mathf.PI - 90);

            //Rect r = bottomRect;
            //cursorRectTransform.anchoredPosition = new Vector2(r.x + r.width / 2, r.y + r.height / 2); ;
            //cursorRectTransform.sizeDelta = new Vector2(r.width, r.height);

        }
    }
    public void Reply()
    {
        if (!hasBeenReplied)
        {
            hasBeenReplied = true;
            SetText(InscriptionManager.CurrentQuestion.reply);
            SetColor(InscriptionManager.CurrentPhilosopheme.replyColor);
            InscriptionManager.OnQuestionReply();
        }
    }
    void SetText(string text)
    {
        //textMesh.SetText(text);
        textMesh.text = text;

        TMP_TextInfo textInfo = textMesh.GetTextInfo(textMesh.text);
        identityFieldDelta = new Vector2(
            2 * Mathf.Abs(textInfo.characterInfo[0].topLeft.x) + padding.x,
            2 * Mathf.Abs(textInfo.characterInfo[0].topLeft.y) + padding.y
        );
        backgroundRectTransform.sizeDelta = identityFieldDelta;
    }
    void SetColor(Color color)
    {
        foreach (Image i in images)
        {
            i.color = color;
        }
    }
    public void SetHidden()
    {
        IsActive = false;
        markRectTransform.anchoredPosition = Vector2.zero;
        markRectTransform.localScale = Vector2.zero;
        InscriptionManager.ForgetInscription(this);
    }
    public bool Show()
    {
        bool isVisible = InscriptionManager.CheckForVisibility(gameObject);
        if (!isVisible)
        {
            Transform[] transforms = GetComponentsInChildren<Transform>();
            if (transforms.Length <= 0) return false;
            foreach (Transform t in transforms)
            {
                if (InscriptionManager.CheckForVisibility(t.gameObject))
                {
                    isVisible = true;
                    break;
                }
            }
        }
        if (isVisible)
        {
            IsActive = true;
            growDeltaMultiplier = 1;

            if (isQuestion)
            {
                if (!hasBeenReplied && counter != InscriptionManager.questionCounter)
                {
                    counter = InscriptionManager.questionCounter;
                    SetText(InscriptionManager.CurrentQuestion.enquiry);
                    SetColor(InscriptionManager.CurrentPhilosopheme.enquiryColor);
                }
            }
            else if (isReply && counter != InscriptionManager.philosophemeCounter)
            {
                counter = InscriptionManager.philosophemeCounter;
                SetText(InscriptionManager.CurrentPhilosopheme.replyPrefix + " " + text);
            }
        }
        return isVisible;
    }
    public void Hide()
    {
        growDeltaMultiplier = -1;
    }
    void OnDisable()
    {
        if (markRectTransform) SetHidden();
    }
}