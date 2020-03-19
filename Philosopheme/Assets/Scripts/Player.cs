using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Animator animator;
    public AnimationClip[] clips;

    public float damage = -50f;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnCollisionEnter(Collision other)
    {
        other.transform.GetComponent<Health>()?.HealthChange(damage);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}


