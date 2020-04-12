using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : AI
{

}


































/*
protected void GetTarget(GameObject other, Vector3 pos, Vector3 dir, float maxDistance, float minDistance)
{  
    RaycastHit hit; 
    Physics.Raycast(pos, dir, out hit);

    if (hit.distance <= maxDistance && hit.transform.GetComponent<Player>() != null)
        agent.SetDestination(other.transform.position);
}

// Start is called before the first frame update
protected void Start()
{
    agent = GetComponent<NavMeshAgent>();
}

// Update is called once per frame
protected void Update()
{
    Vector3 pos = transform.position + Vector3.up;

    foreach (GameObject obj in GameManager.instance.creatures)
    {
        if (obj != transform.gameObject)
        {
            Vector3 dir = obj.transform.position - pos;
            Debug.DrawRay(pos, dir, Color.red);

            GetTarget(obj, pos, dir, 500, 0);
        }
    }
}
*/
