using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CucumberAI : AI
{
    public bool alive = true;
    public override void OnDisable()
    {
        alive = false;
     //   print("Amd");

        ReneDescartes.instance.SetTrigger2();
        ReneDescartes.instance.trigger2 = true;

    }

    
}
