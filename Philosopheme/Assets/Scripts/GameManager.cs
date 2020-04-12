using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<GameObject> creatures = new List<GameObject>();

    public static GameManager instance;
    private void Awake()
    {
        if (!instance) instance = this;
    }
    public Light mainLight;
    [HideInInspector] public float mainLightDefaultIntencity;
    public Camera cam;
    [HideInInspector]  public float camDefaultFieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        mainLightDefaultIntencity = mainLight.intensity;
        camDefaultFieldOfView = cam.fieldOfView;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
