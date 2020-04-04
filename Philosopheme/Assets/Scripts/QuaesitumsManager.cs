using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuaesitumsManager : MonoBehaviour
{
    public Inscription.Quaesitum[] quaesitums;

    // Start is called before the first frame update
    void Awake()
    {
        Inscription.Initialise(quaesitums, 0, 0);
    }
}
