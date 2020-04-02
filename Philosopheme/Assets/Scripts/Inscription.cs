using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inscription : MonoBehaviour
{
    [System.Serializable]
    public class Parameters
    {
        public Vector2 bounds;
        public float emission = 2f;
        public float whiteCorrection = 1.4f;
        public float maxDistance = 8f;
        public float revealTime = 0.3f;
    }
    public static Parameters parameters;

    public string inscription;
    public Transform centerBasis;
    public GameObject textField;
    public GameObject background;
    public Color initialColor;

    TextMeshPro textMesh;
    MeshRenderer backgroundMesh;
    Vector3 normalScale = Vector3.zero;
    Vector3 normalPos = Vector3.zero;
    float maxInscriptionAngleSize;
    [HideInInspector] public bool isActive = false;
    bool isGrowing = false;
    bool isWaiting = false;
    float timer = 0f;

    Vector2 Vector3To2Y(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    float DegreeToRad(float a)
    {
        return a * Mathf.PI / 180;
    }
    Color CorrectColor(Color c)
    {
        c.r = 1 - (1 - c.r) / parameters.whiteCorrection;
        c.g = 1 - (1 - c.g) / parameters.whiteCorrection;
        c.b = 1 - (1 - c.b) / parameters.whiteCorrection;
        return c;
    }
    Color InvertColor(Color c)
    {
        c.r = 1 - c.r;
        c.g = 1 - c.g;
        c.b = 1 - c.b;
        return c;
    }
    void SetBackgroundColor(Color c, float emission)
    {
        backgroundMesh.material.SetColor("_EmissionColor", c * emission);
    }

    void SetText(string text, bool invert)
    {
        textMesh.text = text;
        TMP_TextInfo textInfo = textMesh.GetTextInfo(textMesh.text);
        normalScale = new Vector3(
            2 * (Mathf.Abs(textInfo.characterInfo[0].topLeft.x) + parameters.bounds.x),
            2 * (Mathf.Abs(textInfo.characterInfo[0].topLeft.y) + parameters.bounds.y),
            1f
        );
        background.transform.localScale = normalScale;

        if (invert)
        {
            SetBackgroundColor(CorrectColor(InvertColor(initialColor)), parameters.emission);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        textMesh = textField.GetComponent<TextMeshPro>();
        backgroundMesh = background.GetComponent<MeshRenderer>();
        SetBackgroundColor(CorrectColor(initialColor), parameters.emission);
        SetText(inscription, false);
        maxInscriptionAngleSize = GameManager.instance.cam.fieldOfView * 1.1f;
        centerBasis.localScale = Vector3.zero;
    }

    public void SetActive(bool a)
    {
        isWaiting = false;
        if (a)
        {
            isActive = true;
            isGrowing = true;
        }
        else
        {
            isGrowing = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Vector3 camPos = GameManager.instance.cam.gameObject.transform.position;
            Vector3 objPos = transform.position;
            Vector3 camObjVec = objPos - camPos;
            bool goodDistance = camObjVec.magnitude < parameters.maxDistance;
            if (goodDistance || timer > 0)
            {
                if (!isWaiting)
                {
                    if (isGrowing)
                    {
                        timer += Time.deltaTime;
                        if (timer >= parameters.revealTime)
                        {
                            timer = parameters.revealTime;
                            isWaiting = true;
                        }
                    }
                    else
                    {
                        timer -= Time.deltaTime;
                        if (timer <= 0)
                        {
                            timer = 0;
                            isActive = false;
                            normalPos = Vector3.zero;
                        }
                    }
                }

                Vector3 camForwardVec = GameManager.instance.cam.gameObject.transform.forward;          //Debug.DrawRay(camPos, camForwardVec, Color.green, Time.deltaTime);

                float camObjAngle = Vector2.Angle(Vector3To2Y(camForwardVec), Vector3To2Y(camObjVec));
                bool isVisible = camObjAngle < maxInscriptionAngleSize / 2 && goodDistance;

                Vector3 centerNormalVec = centerBasis.right * normalScale.x * centerBasis.lossyScale.x / 2;                //Debug.DrawRay(centerBasis.position, centerNormalVec, Color.blue, Time.deltaTime);
                Vector3 centerPos = centerBasis.position;
                Vector3 camCenterVec = centerPos - camPos;                                              //Debug.DrawLine(centerPos, camPos, Color.cyan, Time.deltaTime);
                float halfSize = centerNormalVec.magnitude;
                float maxHalfSize = camCenterVec.magnitude * Mathf.Tan(DegreeToRad(maxInscriptionAngleSize / 2));
                //float inscriptionAngleSize = Vector3.Angle(camObjVec, camCenterVec);
                bool isFit = halfSize <= maxHalfSize;

                if (isVisible)
                {
                    float ranglePos = DegreeToRad(Vector2.Angle(Vector3To2Y(camForwardVec), Vector3To2Y(camCenterVec)));
                    float rangleSize = Mathf.Atan2(centerNormalVec.magnitude, camCenterVec.magnitude);
                    float maxRanglePos = DegreeToRad(maxInscriptionAngleSize / 2) - rangleSize;
                    if (maxRanglePos < 0) maxRanglePos = 0;

                    if (ranglePos > maxRanglePos)
                    {
                        float a = ranglePos - maxRanglePos;
                        if (Vector3.Cross(camForwardVec, camCenterVec).y < 0) a *= -1;
                        float sinA = Mathf.Sin(a);
                        float cosA = Mathf.Cos(a);

                        Vector3 targetVec = new Vector3(
                            camCenterVec.x * cosA - camCenterVec.z * sinA,
                            camCenterVec.y,
                            camCenterVec.x * sinA + camCenterVec.z * cosA
                        );
                        targetVec *= camCenterVec.magnitude / targetVec.magnitude;

                        Vector2 pos1 = Vector3To2Y(camPos);
                        Vector2 pos2 = Vector3To2Y(objPos);
                        Vector2 vec1 = Vector3To2Y(targetVec);
                        Vector2 vec2 = Vector3To2Y(transform.right);

                        float a1 = vec1.y / vec1.x;
                        float a2 = vec2.y / vec2.x;
                        float b1 = pos1.y - a1 * pos1.x;
                        float b2 = pos2.y - a2 * pos2.x;

                        //float f1(float x)
                        //{
                        //    return a1 * x + b1;
                        //}
                        //float f2(float x)
                        //{
                        //    return a2 * x + b2;
                        //}

                        //Debug.DrawLine(new Vector3(-9999, camPos.y, f1(-9999)), new Vector3(9999, camPos.y, f1(9999)), Color.red, Time.deltaTime);
                        //Debug.DrawLine(new Vector3(-9999, camPos.y, f2(-9999)), new Vector3(9999, camPos.y, f2(9999)), Color.blue, Time.deltaTime);
                        //print("F1 = " + a1 + " * x + " + b1);
                        //print("F2 = " + a2 + " * x + " + b2);

                        float xCross = (b2 - b1) / (a1 - a2);
                        float yCross = a1 * xCross + b1;

                        Vector3 resultPos = new Vector3(xCross, centerPos.y, yCross);

                        //Debug.DrawRay(camPos, camCenterVec, Color.red, Time.deltaTime);
                        //Debug.DrawRay(camPos, targetVec, Color.blue, Time.deltaTime);
                        normalPos = resultPos - objPos;
                    }
                }
                else
                {
                    SetActive(false);
                }
                if (2 * maxHalfSize < normalScale.x * transform.lossyScale.x)
                {
                    centerBasis.localScale = Vector3.one * 2 * maxHalfSize * (1 - 0.01f) * timer / (parameters.revealTime * normalScale.x * transform.lossyScale.x);
                }
                else
                {
                    centerBasis.localScale = Vector3.one * timer / parameters.revealTime;
                }
                centerBasis.position = objPos + normalPos * timer / parameters.revealTime;
                transform.rotation = Quaternion.LookRotation(camPos - centerBasis.position, Vector3.up);
            }
            else
            {
                isActive = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SetText("Мясной панч!", true);
        }
    }
}
