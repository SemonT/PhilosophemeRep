using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Vector3 speed;
    private Vector3 prevPos;

    public Animator animator;
    public AnimationClip[] clips;

 //   public float damage = -50f;

    public Transform armTransform;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator) clips = animator.runtimeAnimatorController.animationClips;

        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 curPos = transform.position;
        speed = (prevPos - curPos) / Time.fixedTime;

    //    print("Скорость " + speed.magnitude);
        prevPos = curPos;
    }

    /*
    private void OnCollisionEnter(Collision other)
    {
        other.transform.GetComponent<Health>()?.HealthChange(damage);
    }
    */
    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}


