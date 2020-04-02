using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    Transform cameraTransform;
    public float InteractionMaxDistance;
    public float InscriptionMaxDistance;
    public Image pointerImage;
    public Sprite pointerInactive;
    public Sprite pointerActive;

    bool isPointerActive = false;
    Inscription currentInscription;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GameManager.instance.cam.transform;
    }

    // Update is called once per frame

    public void UpdateWP(bool interact)
    {
        RaycastHit hit;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit);
        if (hit.distance < InteractionMaxDistance)
        {
            Interactable interactable = hit.transform?.gameObject.GetComponent<Interactable>();
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
        
        if (hit.distance < 100)
        {
            Inscription inscription = hit.transform?.gameObject.GetComponent<Inscription>();
            if (inscription)
            {
                if (currentInscription)
                {
                    if (currentInscription != inscription)
                    {
                        currentInscription.SetActive(false);
                        currentInscription = inscription;
                        currentInscription.SetActive(true);
                    }
                    else if (!currentInscription.isActive)
                    {
                        currentInscription.SetActive(true);
                    }
                }
                else
                {
                    currentInscription = inscription;
                    currentInscription.SetActive(true);
                }
            }
            else if (currentInscription)
            {
                currentInscription = null;
            }
        }
    }
}
