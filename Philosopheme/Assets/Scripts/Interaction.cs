using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public static Interaction instance;

    public float InteractionMaxDistance;
    public float InscriptionMaxDistance;
    public float holdFTime = 0.5f;
    public Image pointerImage;
    public Sprite pointerInactive;
    public Sprite pointerActive;

    bool isPointerActive = false;

    private void Awake()
    {
        if (!instance) instance = this;
    }
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public void UpdateWP(bool interact)
    {
        if (GameManager.regardHit.distance < InteractionMaxDistance)
        {
            Interactable interactable = GameManager.regardHit.transform?.gameObject.GetComponent<Interactable>();
            if (interactable && interactable.isInteractable)
            {
                if (!isPointerActive) pointerImage.sprite = pointerActive;
                isPointerActive = true;
                if (interact)
                {
                    interactable.Interact();
                }
            }
            else
            {
                if (isPointerActive)
                {
                    pointerImage.sprite = pointerInactive;
                    isPointerActive = false;
                }
            }
        } else
        {
            if (isPointerActive)
            {
                pointerImage.sprite = pointerInactive;
                isPointerActive = false;
            }
        }
    }
}
