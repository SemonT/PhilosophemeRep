using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InscriptionManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string enquiry;
        public string reply;
    }

    [System.Serializable]
    public class Philosopheme
    {
        public Color enquiryColor = Color.black;
        public Color replyColor = Color.black;
        public string replyPrefix;
        public Question[] questions;
    }
    //public delegate void OnPhilosophemeUpdate();

    public static Camera cam;
    static float maxVisibleDistance;

    //static OnPhilosophemeUpdate PhilosophemeUpdateEvent;
    public static int questionCounter { get; private set; }
    public static int philosophemeCounter { get; private set; }
    static int currentPhilosophemeIndex;
    public static Philosopheme CurrentPhilosopheme { get; private set; }
    static int currentQuestionIndex;
    public static Question CurrentQuestion { get; private set; }
    public static Inscription lastInscription { get; set; }
    static Philosopheme[] philosophemes;

    public static bool VisibilityFilter(GameObject go)
    {
        if (GameManager.DefaultVisibilityFilter(go)) return true;

        Transform parent = go.GetComponentInParent<Inscription>()?.gameObject.transform;
        if (parent)
        {
            Transform[] childTransforms = parent.GetComponentsInChildren<Transform>();
            if (childTransforms.Length > 0)
            {
                foreach (Transform t in childTransforms)
                {
                    if (t == go.transform)
                        return true;
                }
            }
        }
        return false;
    }
    public static bool CheckForVisibility(GameObject go)
    {
        if (GameManager.CheckForLinearVisibility(cam.gameObject, go, maxVisibleDistance, ~0, QueryTriggerInteraction.Ignore, VisibilityFilter))
        {
            return true;
        }
        bool isVisible = false;
        Transform[] transforms = go.GetComponentsInChildren<Transform>();
        if (transforms.Length > 0)
        {
            foreach (Transform t in transforms)
            {
                if (GameManager.CheckForLinearVisibility(cam.gameObject, t.gameObject, maxVisibleDistance, ~0, QueryTriggerInteraction.Ignore, VisibilityFilter))
                {
                    isVisible = true;
                    break;
                }
            }
        }
        return isVisible;
    }
    //public static void SubscribeToPhilosophemeUpdateEvent(OnPhilosophemeUpdate f)
    //{
    //    PhilosophemeUpdateEvent += f;
    //}
    //public static void UnsubscribeToPhilosophemeUpdateEvent(OnPhilosophemeUpdate f)
    //{
    //    PhilosophemeUpdateEvent -= f;
    //}
    public static void ForgetInscription(Inscription i)
    {
        if (i == lastInscription) lastInscription = null;
    }
    public static void OnQuestionReply()
    {
        questionCounter++;
        currentQuestionIndex++;
        if (currentQuestionIndex >= CurrentPhilosopheme.questions.Length)
        {
            philosophemeCounter++;
            currentQuestionIndex = 0;
            currentPhilosophemeIndex++;
            if (currentPhilosophemeIndex >= philosophemes.Length)
            {
                print("_!_END OF PHILOSOPHEMES_!_");
                currentPhilosophemeIndex = 0;
            }
        }
        CurrentPhilosopheme = philosophemes[currentPhilosophemeIndex];
        CurrentQuestion = CurrentPhilosopheme.questions[currentQuestionIndex];
        //PhilosophemeUpdateEvent?.Invoke();
    }

    public float maxDistance = 10f;
    public float revealTime = 0.25f;
    public Vector2 padding;
    public Philosopheme[] philosophemesList;
    public GameObject inscriptionsSpace;
    public GameObject markPrefab;

    Transform camTransform;

    public void Awake()
    {
        cam = GameManager.instance.cam;
        maxVisibleDistance =  maxDistance;
        camTransform = cam.gameObject.transform;

        Inscription.Init(markPrefab, cam, inscriptionsSpace, maxDistance, revealTime, padding);

        questionCounter = 0;
        philosophemeCounter = 0;
        philosophemes = philosophemesList;
        currentPhilosophemeIndex = 0;
        currentQuestionIndex = 0;
        CurrentPhilosopheme = philosophemes[currentPhilosophemeIndex];
        CurrentQuestion = CurrentPhilosopheme.questions[currentQuestionIndex];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (
            !npc.isDialogueOpened
            )
        {
            RaycastHit[] hits = Physics.RaycastAll(camTransform.position, camTransform.forward, maxDistance);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                GameObject go = hit.collider.gameObject;
                Inscription inscr = go.GetComponent<Inscription>();
                if (!inscr)
                {
                    Inscription[] inscrs = go.GetComponentsInParent<Inscription>();
                    if (inscrs.Length > 0)
                        inscr = inscrs[0];
                }
                if (inscr && CheckForVisibility(go))
                {
                    if (lastInscription)
                    {
                        if (inscr != lastInscription && inscr.Show())
                        {
                            lastInscription.Hide();
                            lastInscription = inscr;
                        }
                    }
                    else
                    {
                        if (inscr.Show()) lastInscription = inscr;
                    }
                    break;
                }
            }
        }
        else if (lastInscription)
        {
            lastInscription.Hide();
            lastInscription = null;
        }
    }
}