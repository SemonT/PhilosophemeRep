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
    [System.Serializable]
    public class Philosopheme
    {
        public string initial = "А";
        public string ultimate = "Б";
    }
    [System.Serializable]
    public class Quaesitum
    {
        public Color initialColor = Color.clear;
        public Color ultimateColor = Color.clear;
        public Philosopheme[] philosophemes;
    }
    public static Parameters parameters;
    public static Quaesitum[] quaesitums;
    public static int currentQuaesitumI;
    public static Quaesitum currentQuaesitum;
    public static int currentPhilosophemeI;
    public static Philosopheme currentPhilosopheme;
    public static List<Inscription> philosophemeInscriptions = new List<Inscription>();

    public static void Initialise(Quaesitum[] quaesitums, int currentQuaesitumI, int currentPhilosophemeI)
    {
        Inscription.quaesitums = quaesitums;
        Inscription.currentQuaesitumI = currentQuaesitumI;
        Inscription.currentPhilosophemeI = currentPhilosophemeI;
        currentQuaesitum = quaesitums[currentQuaesitumI];
        currentPhilosopheme = currentQuaesitum.philosophemes[currentPhilosophemeI];
    }
    static Vector2 Vector3To2Y(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    static float DegreeToRad(float a)
    {
        return a * Mathf.PI / 180;
    }
    static Color CorrectColor(Color c)
    {
        c.r = 1 - (1 - c.r) / parameters.whiteCorrection;
        c.g = 1 - (1 - c.g) / parameters.whiteCorrection;
        c.b = 1 - (1 - c.b) / parameters.whiteCorrection;
        return c;
    }
    static Color InvertColor(Color c)
    {
        c.r = 1 - c.r;
        c.g = 1 - c.g;
        c.b = 1 - c.b;
        return c;
    }
    static void GoNext()
    {
        currentPhilosophemeI++;
        if (currentPhilosophemeI >= currentQuaesitum.philosophemes.Length)
        {
            currentPhilosophemeI = 0;
            currentQuaesitumI++;
            if (currentQuaesitumI < quaesitums.Length)
            {
                currentQuaesitum = quaesitums[currentQuaesitumI];

                foreach (Inscription i in philosophemeInscriptions)
                {
                    i.SetBackgroundAndCursorColor(CorrectColor(currentQuaesitum.initialColor), parameters.emission);
                }
            }
            else
            {
                print("END OF PHILOSOPHEMES!!!");
                return;
            }
        }
        currentPhilosopheme = currentQuaesitum.philosophemes[currentPhilosophemeI];
        foreach (Inscription i in philosophemeInscriptions)
        {
            i.SetText(currentPhilosopheme.initial);
        }
    }

    public bool isPhilosopheme = false;
    public string inscription;
    public Color initialColor;
    public Transform centerBasis;
    public Transform textField;
    public GameObject text;
    public GameObject background;
    public GameObject cursor;

    TextMeshPro textMesh;
    MeshRenderer backgroundMesh;
    MeshRenderer cursorMesh;
    Vector3 normalScale = Vector3.zero;
    Vector3 normalPos = Vector3.zero;
    float maxInscriptionAngleSize;
    [HideInInspector] public bool isActive = false;
    bool isGrowing = false;
    bool isWaiting = false;
    float timer = 0f;
    ParticleSystem effect;
    Vector3 normalShapeScale;
    float normalStartSize;
    float normalStartSpeed;
    int normalEmissionCount;
    bool activated = false;

    public void NotimeDeactivate()
    {
        SetActive(false);
        timer = 0;
        centerBasis.localPosition = Vector3.zero;
        centerBasis.localScale = Vector3.zero;
        print("Nuuuu");
    }
    public void ActivatePhilosopheme()
    {
        if (isPhilosopheme && !activated)
        {
            activated = true;
            Color newColor;
            if (currentQuaesitum.ultimateColor == Color.clear)
            {
                newColor = CorrectColor(InvertColor(currentQuaesitum.initialColor));
            }
            else
            {
                newColor = CorrectColor(currentQuaesitum.ultimateColor);
            }
            PlayEffect();
            SetBackgroundAndCursorColor(newColor, parameters.emission);
            SetText(currentPhilosopheme.ultimate);

            philosophemeInscriptions.Remove(this);
            print(philosophemeInscriptions.Count);
            GoNext();
        }
        else
        {
            print("IS NOT PHILOSOPHEME!!!");
        }
    }
    void SetBackgroundAndCursorColor(Color c, float emission)
    {
        backgroundMesh.material.SetColor("_EmissionColor", c * emission);
        cursorMesh.material.SetColor("_EmissionColor", c * emission);
    }
    public void PlayEffect()
    {
        if (centerBasis.localScale.x > 0)
        {
            ParticleSystem.MainModule main;
            ParticleSystem.ShapeModule shape;
            ParticleSystem.Burst burst;

            main = effect.main;

            shape = effect.shape;
            shape.scale = new Vector3(normalShapeScale.x * background.transform.localScale.x, normalShapeScale.y * background.transform.localScale.y, normalShapeScale.z);

            main.startSize = normalStartSize * centerBasis.localScale.x;

            main.startSpeed = normalStartSpeed * centerBasis.localScale.x;

            burst = effect.emission.GetBurst(0);
            burst.count = normalEmissionCount * background.transform.localScale.x * background.transform.localScale.y;
            effect.emission.SetBurst(0, burst);

            string parameterName = "_EmissionColor";
            effect.GetComponent<Renderer>().material.SetColor(parameterName, backgroundMesh.GetComponent<Renderer>().material.GetColor(parameterName));

            effect.Play();
        }
    }
    void SetText(string text)
    {
        textMesh.text = text;
        TMP_TextInfo textInfo = textMesh.GetTextInfo(textMesh.text);
        normalScale = new Vector3(
            2 * (Mathf.Abs(textInfo.characterInfo[0].topLeft.x) + parameters.bounds.x),
            2 * (Mathf.Abs(textInfo.characterInfo[0].topLeft.y) + parameters.bounds.y),
            1f
        );
        background.transform.localScale = normalScale;
        textField.transform.localPosition = new Vector3(0, normalScale.y, 0);
        cursor.transform.localScale = new Vector3(cursor.transform.localScale.x, 0, cursor.transform.localScale.z);
    }
    // Start is called before the first frame update
    void Start()
    {
        textMesh = text.GetComponent<TextMeshPro>();
        backgroundMesh = background.GetComponent<MeshRenderer>();
        cursorMesh = cursor.GetComponent<MeshRenderer>();
        maxInscriptionAngleSize = GameManager.instance.cam.fieldOfView * 1.1f;
        centerBasis.localScale = Vector3.zero;
        centerBasis.gameObject.SetActive(true);

        effect = background.GetComponentInChildren<ParticleSystem>();
        normalShapeScale = effect.shape.scale;
        ParticleSystem.MainModule main = effect.main;
        normalStartSize = main.startSize.constant;
        normalStartSpeed = main.startSpeed.constant;
        normalEmissionCount = (int)effect.emission.GetBurst(0).count.constant;

        if (isPhilosopheme)
        {
            philosophemeInscriptions.Add(this);
            SetBackgroundAndCursorColor(CorrectColor(currentQuaesitum.initialColor), parameters.emission);
            SetText(currentPhilosopheme.initial);
        }
        else
        {
            SetBackgroundAndCursorColor(CorrectColor(initialColor), parameters.emission);
            SetText(inscription);
        }
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

                Vector3 fieldPos = textField.position;
                Vector3 fieldObjVec = objPos - fieldPos;

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
                        Vector2 vec2 = Vector3To2Y(centerNormalVec);

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
                else if (isGrowing) SetActive(false);

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

                // Требует оптимизации
                if (centerBasis.localScale.x != 0)
                {
                    cursor.transform.localScale = new Vector3(
                    cursor.transform.localScale.x,
                    fieldObjVec.magnitude / transform.lossyScale.y / centerBasis.localScale.x,
                    cursor.transform.localScale.z
                    );
                    cursor.transform.localPosition = new Vector3(-centerBasis.localPosition.x / (2 * centerBasis.localScale.x), textField.localPosition.y / 2, textField.localPosition.z - 0.01f);
                    //Debug.DrawRay(fieldPos, fieldObjVec, Color.blue, Time.deltaTime);
                    cursor.transform.localRotation = Quaternion.LookRotation(Vector3.back, new Vector3(-centerBasis.localPosition.x / centerBasis.localScale.x, -textField.localPosition.y, 0));
                }
                else
                {
                    cursor.transform.localScale = new Vector3(cursor.transform.localScale.x, 0, cursor.transform.localScale.z);
                }
            }
            else
            {
                isActive = false;
            }
        }
    }
}
