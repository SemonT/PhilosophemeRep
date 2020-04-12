using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : Interactable
{
    public abstract class Action
    {
        public string animationName;
        public Animation animation;

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
                End();
            }
        }
        public void End()
        {
            isActual = false;
            OnEnd();
        }
        public abstract void Initialize();
        public abstract void OnStart();
        public abstract void OnUpdate();
        public abstract void OnEnd();
    }

    public string className = "undefined";
    public Transform handleBasis;

    [HideInInspector] public bool isUsable = false;
    [HideInInspector] public Action[] actions = new Action[0];
    [HideInInspector] public Animator animator;

    [HideInInspector] public bool mouse0Key;
    [HideInInspector] public bool mouse1Key;
    [HideInInspector] public bool rKey;
    [HideInInspector] public bool mouse0KeyDown;
    [HideInInspector] public bool mouse1KeyDown;
    [HideInInspector] public bool rKeyDown;
    [HideInInspector] public bool mouse0KeyUp;
    [HideInInspector] public bool mouse1KeyUp;
    [HideInInspector] public bool rKeyUp;
    [HideInInspector] public float mouse0KeyTimer;
    [HideInInspector] public float mouse1KeyTimer;
    [HideInInspector] public float rKeyTimer;

    public void SetUsable(Animator animator)
    {
        isUsable = true;
        this.animator = animator;
        if (actions.Length > 0)
        {
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
        foreach (Action a in actions)
        {
            a.it = this;
            a.Initialize();
        }
        mouse0Key = false;
        mouse1Key = false;
        rKey = false;
        mouse0KeyDown = false;
        mouse1KeyDown = false;
        rKeyDown = false;
        mouse0KeyUp = false;
        mouse1KeyUp = false;
        rKeyUp = false;
        mouse0KeyTimer = 0;
        mouse1KeyTimer = 0;
        rKeyTimer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (isUsable) foreach (Action a in actions) if (a.isActual) a.Update();
    }

    public override void Interact()
    {
        Inventory inv = Inventory.instance;
        if (Inventory.isOpened)
        {
            inv.TakeItem(this);
        }
        else
        {
            inv.PickupItem(this);
        }
    }

    public void Use(bool mouse0Key, bool mouse1Key, bool rKey, bool mouse0KeyDown, bool mouse1KeyDown, bool rKeyDown, bool mouse0KeyUp, bool mouse1KeyUp, bool rKeyUp)
    {
        this.mouse0Key = mouse0Key;
        this.mouse1Key = mouse1Key;
        this.rKey = rKey;

        this.mouse0KeyDown = mouse0KeyDown;
        this.mouse1KeyDown = mouse1KeyDown;
        this.rKeyDown = rKeyDown;

        this.mouse0KeyUp = mouse0KeyUp;
        this.mouse1KeyUp = mouse1KeyUp;
        this.rKeyUp = rKeyUp;

        Use();

        if (mouse0Key)
            mouse0KeyTimer += Time.deltaTime;
        else
            mouse0KeyTimer = 0;
        if (mouse1Key)
            mouse1KeyTimer += Time.deltaTime;
        else
            mouse1KeyTimer = 0;
        if (rKey)
            rKeyTimer += Time.deltaTime;
        else
            rKeyTimer = 0;
    }
    public abstract void SetActions();
    public abstract void Use();
}
