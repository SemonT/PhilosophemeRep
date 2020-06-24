using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class AI : MonoBehaviour
{
    public bool isHostile = true;
    public GameObject glaz;

    protected NavMeshAgent agent;
    protected Rigidbody body;
    protected Animator anime;
    protected float maxDist = 500;
    protected float minDist = 2.5f;
    protected float curDist = 10000;
    protected bool ranged;

    protected float halfAngle = 180f;
    protected float curAngle = 10000;

    protected Player player;

    protected Vector3 dd;

    // Переменная для милишных ИИ
    protected float deltaAttack = 0f;

    public abstract void OnDisable();

    public void Disable()
    {
        if (isHostile & agent) agent.isStopped = true;
        OnDisable();
    }

    protected bool Filter(GameObject go)
    {
        if (GameManager.DefaultVisibilityFilter(go))
        {
            return true;
        }
        else
        {

            Transform t = go.transform;
            do
            {      
                if (t.gameObject == gameObject)
                {
                    return true;
                }
                t = t.parent;
            }
            while (t);
        }
        
        return false;
    }

    protected virtual void GetTarget(GameObject other, Vector3 pos, Vector3 dir, float maxDistance, float minDistance, out float currentDistance, out float currentAngle, out Vector3 d)
    {

        bool isVisible = GameManager.CheckForLinearVisibility(glaz, player.gameObject, maxDistance, ~0, QueryTriggerInteraction.Ignore, Filter);

        Debug.DrawRay(pos, dir, Color.red);

        Vector3 deltaD = player.transform.position - transform.position;

   //     Debug.DrawRay(transform.position, deltaD, Color.cyan);
        currentDistance = deltaD.magnitude;
        currentAngle = Vector3.Angle(transform.forward, deltaD);

        deltaD.y = 0;
        d = deltaD;

        bool inVisie = currentAngle < halfAngle;

 //       print(deltaD);

        if (isVisible)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        body = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

        player = Player.instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Vector3 pos = transform.position + transform.up * 0.5f;

        foreach (GameObject obj in GameManager.instance.creatures)
        {
            if (obj != transform.gameObject)
            {
                Vector3 dir = obj.transform.position - pos;

                if (isHostile & obj.GetComponent<Player>() != null)
                {
                    GetTarget(obj, pos, dir, maxDist, minDist, out curDist, out curAngle, out dd);
                }
            }
        }
        // Ниже код, отвечающий за анимации

        float veloc = agent.velocity.magnitude;
        anime.SetFloat("Move", veloc);
 //       print("Скорость " + transform.name + " !! " + veloc);

        float tempDist = agent.stoppingDistance + deltaAttack;


 //       print(transform.name + " раз  " + tempDist + " и расстояние " + curDist);

        ranged = curDist <= tempDist;


        if (ranged & isHostile)
        {
            transform.LookAt(transform.position + dd);
            anime.SetTrigger("Attack");
        }

 //       print("Расстояние " + transform.name + " !! " + curDist + " А необходимо: " + tempDist);
    }



}
