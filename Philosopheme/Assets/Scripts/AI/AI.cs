using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AI : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Rigidbody body;
    protected Animator anime;
    protected float maxDist = 500;
    protected float minDist = 2.5f;
    protected float curDist;
    protected bool ranged;

    protected float halfAngle = 180f;
    protected float curAngle;

    protected Player player;

    // Переменная для милишных ИИ
    protected  float deltaAttack = 0f;

    protected virtual void GetTarget(GameObject other, Vector3 pos, Vector3 dir, float maxDistance, float minDistance, out float currentDistance, out float currentAngle)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit);

        /*
        print(hit.transform.root.name);
        print(hit.collider.gameObject.transform.name);
        */
        // ГОВНО
        Debug.DrawRay(pos, dir, Color.red);

        Vector3 newPos = hit.collider.gameObject.transform.position;
        if (hit.transform.Equals(transform))
        {
            Vector3 newDir = player.transform.position - newPos;

            Physics.Raycast(newPos, dir, out hit);
            Debug.DrawRay(newPos, dir, Color.yellow);
        }

        


        float temp = hit.distance;
        currentDistance = 1000000;

        bool isPlayer = hit.transform.GetComponent<Player>() != null;

        currentAngle = Vector3.Angle(transform.forward, dir);
        bool inVisie = currentAngle < halfAngle;

        if ((temp <= maxDistance && isPlayer && inVisie) || (temp <= minDist && isPlayer))
        {
            agent.SetDestination(other.transform.position);
            currentDistance = temp;
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
                

                GetTarget(obj, pos, dir, maxDist, minDist, out curDist, out curAngle);
            }
        }
        // Ниже код, отвечающий за анимации

        float veloc = agent.velocity.magnitude;
        anime.SetFloat("Move", veloc);
 //       print("Скорость " + transform.name + " !! " + veloc);

        float tempDist = agent.stoppingDistance + deltaAttack;

        

        ranged = curDist <= tempDist;
        if (ranged)
        {
            print(curAngle);
            transform.LookAt(player.transform);
            anime.SetTrigger("Attack");
        }

 //       print("Расстояние " + transform.name + " !! " + curDist + " А необходимо: " + tempDist);
    }



}
