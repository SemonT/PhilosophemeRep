using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialModel : MonoBehaviour
{
    public float density = 3f;
    public string packName = "default";
    [HideInInspector] public GameManager.MaterialPack pack;

    public static MaterialModel defaultMaterialModel;
    public static void Initialise()
    {
        defaultMaterialModel = GameManager.instance.gameObject.AddComponent<MaterialModel>();
    }

    void Start()
    {
        foreach (GameManager.MaterialPack mp in GameManager.instance.materialPacks)
        {
            if (mp.packName == packName)
            {
                pack = mp;
                break;
            }
        }
    }
}
