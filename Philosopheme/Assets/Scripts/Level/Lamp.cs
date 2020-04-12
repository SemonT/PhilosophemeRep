using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    static List<Lamp> allLamps = new List<Lamp>();

    public static void TurnOnAllLamps()
    {
        for (int i = 0; i < allLamps.Count; i++) allLamps[i].TurnOn();
    }
    public static void TurnOffAllLamps()
    {
        for (int i = 0; i < allLamps.Count; i++) allLamps[i].TurnOff();
    }

    public Light lightSource;
    public MeshRenderer lampRenderer;
    public Material onMaterial;
    public Material offMaterial;

    public void TurnOn()
    {
        lightSource.enabled = true;
        lampRenderer.material = onMaterial;
    }
    public void TurnOff()
    {
        lightSource.enabled = false;
        lampRenderer.material = offMaterial;
    }

    // Start is called before the first frame update
    void Start()
    {
        allLamps.Add(this);
    }
}
