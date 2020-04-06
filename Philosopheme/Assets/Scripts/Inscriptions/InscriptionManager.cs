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
        public Question[] questions;
    }
    public delegate void OnPhilosophemeUpdate();

    static OnPhilosophemeUpdate PhilosophemeUpdateEvent;
    static int currentPhilosophemeIndex;
    public static Philosopheme CurrentPhilosopheme { get; private set; }
    static int currentQuestionIndex;
    public static Question CurrentQuestion { get; private set; }
    public static Inscription lastInscription { get; set; }
    static Philosopheme[] philosophemes;

    public static void SubscribeToPhilosophemeUpdateEvent(OnPhilosophemeUpdate f)
    {
        PhilosophemeUpdateEvent += f;
    }
    public static void UnsubscribeToPhilosophemeUpdateEvent(OnPhilosophemeUpdate f)
    {
        PhilosophemeUpdateEvent -= f;
    }
    public static void ForgetInscription(Inscription i)
    {
        if (i == lastInscription) lastInscription = null;
    }
    static void OnQuestionReply()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex >= CurrentPhilosopheme.questions.Length)
        {
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
        PhilosophemeUpdateEvent?.Invoke();
    }

    public float maxDistance = 10f;
    public float revealTime = 0.25f;
    public Vector2 padding;
    public Philosopheme[] philosophemesList;
    public Camera cam;
    public GameObject inscriptionsSpace;
    public GameObject markPrefab;

    Transform camTransform;

    private void Awake()
    {
        camTransform = cam.gameObject.transform;

        Inscription.Init(markPrefab, cam, inscriptionsSpace, maxDistance, revealTime, padding, OnQuestionReply);

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
            RaycastHit hit;
            Physics.Raycast(camTransform.position, camTransform.forward, out hit);
            if (hit.collider)
            {
                GameObject go = hit.collider.gameObject;
                Inscription inscr = go.GetComponent<Inscription>();
                if (inscr)
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