using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    Transform cameraTransform;
    public float InteractionMaxDistance;
    public Image pointerImage;
    public Sprite pointerInactive;
    public Sprite pointerActive;
    bool isActive = false;

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
                if (!isActive) pointerImage.sprite = pointerActive;
                isActive = true;

                if (interact)
                {
                    interactable.Interact();
                }
            }
            else
            {
                if (isActive)
                {
                    pointerImage.sprite = pointerInactive;
                    isActive = false;
                }
            }
        } else
        {
            if (isActive)
            {
                pointerImage.sprite = pointerInactive;
                isActive = false;
            }
        }
    }
}
