using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ИИ ближнего боя, подходит и убивает, можно добавить вариант с камикадзе
// При движении пусть вращается будто ползёт
public class MeleeAI : AI
{
    public GameObject damager;
    private Damager d;


    public override void OnDisable()
    {

    }
    protected override void Start()
    {
        base.Start();
        deltaAttack = 2f;

        d = damager.GetComponent<Damager>();
    }

    protected override void Update()
    {
        base.Update();

        d.isActive = ranged;
    }
}
