using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        public Color enquiryColor;
        public Color replyColor;
        public Question[] questions;
    }
    static int currentPhilosophemeIndex;
    static Philosopheme currentPhilosopheme;
    static int currentQuestionIndex;
    static Question currentQuestion;

    static void GoToNextQuestion()
    {

    }

    public GameObject inscriptionsSpace;
    public GameObject inscriptionPrefab;
    public float revealTime = 0.25f;
    public Vector2 padding;

    public Philosopheme[] philosophemes;

    Transform camTransform;
    List<Inscription> currentInscriptions;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam = GameManager.instance.cam;
        camTransform = cam.gameObject.transform;

        Inscription.Init(cam, inscriptionsSpace, revealTime, padding);
        currentInscriptions = new List<Inscription>();

        currentPhilosophemeIndex = 0;
        currentQuestionIndex = 0;
        currentPhilosopheme = philosophemes[currentPhilosophemeIndex];
        currentQuestion = currentPhilosopheme.questions[currentQuestionIndex];
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < currentInscriptions.Count; i++)
        {
            if (!currentInscriptions[i])
            {
                currentInscriptions.RemoveAt(i);
                i--;
            }
        }

        RaycastHit hit;
        Physics.Raycast(camTransform.position, camTransform.forward, out hit);
        if (hit.collider)
        {
            GameObject go = hit.collider.gameObject;
            Health h = go.GetComponent<Health>();
            if (h)
            {
                bool isInList = false;
                foreach (Inscription i in currentInscriptions)
                {
                    if (i.targetObj == go)
                    {
                        isInList = true;
                        break;
                    }
                }
                if (!isInList)
                {
                    for (int i = 0; i < currentInscriptions.Count; i++) currentInscriptions[i].Collapse();
                    currentInscriptions.Add(Instantiate(inscriptionPrefab).GetComponent<Inscription>().SetTarget(go));
                }
            }
        }
    }
}
