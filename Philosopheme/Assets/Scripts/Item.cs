using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Interactable
{
    public abstract class Action
    {
        public string animationName;

        [HideInInspector] public Item it;
        [HideInInspector] public float animationLength;
        [HideInInspector] public float timer;
        [HideInInspector] public bool isActual;
        public void Start()
        {
            isActual = true;
            timer = animationLength;
            if (animationLength > 0) it.animator.SetTrigger(animationName);
            OnStart();
        }
        public void Update()
        {
            OnUpdate();

            if (timer > 0)
                timer -= Time.deltaTime;
            else
            {
                isActual = false;
            }
        }
        public void End()
        {
            isActual = false;
            OnEnd();
        }
        public abstract void OnStart();
        public abstract void OnUpdate();
        public abstract void OnEnd();
    }

    public string className = "undefined";
    public Transform handle;
    public Transform forwardPointer;

    [HideInInspector] public bool isUsable = false;
    [HideInInspector] public Action[] actions;
    [HideInInspector] public Animator animator;

    public void SetUsable(Animator animator)
    {
        isUsable = true;
        this.animator = animator;
        foreach (Action action in actions)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                foreach (AnimationClip clip in clips)
                {
                    if (clip.name == action.animationName)
                    {
                        action.animationLength = clip.length;
                    }
                }
            }
        }
    }
    public void SetUnusable()
    {
        isUsable = false;
        animator = null;
        foreach (Action action in actions)
        {
            action.animationLength = 0;
        }
    }

    void Start()
    {
        SetActions();
        foreach (Action a in actions) a.it = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (isUsable) foreach (Action a in actions) if (a.isActual) a.Update();
    }

    public override void Interact()
    {
        Inventory inv = Inventory.instance;
        if (inv.isOpened)
        {
            inv.TakeItem(this);
        }
        else
        {
            inv.PickupItem(this);
        }
    }

    public abstract void SetActions();
    public abstract void Use(bool mouse0Key, bool mouse1Key, bool rKey);
}
