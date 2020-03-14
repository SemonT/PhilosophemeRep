using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 2f;
    public float strafeCoff = 0.4f;

    // В настройках проекта есть настрока чувствительности мыши, что нужно будет перетащить в настройки игры и эту переменную использовать оттудова
    public float mouseSense = 1.5f;
    private Transform cam;

    public void MoveOnGround(float x, float y)
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + strafeCoff * transform.right * x + transform.forward * y, speed * Time.deltaTime);
    }

    public void Turn(float mouseX, float mouseY)
    {
        transform.Rotate(0, mouseSense * mouseX, 0, Space.Self);

        if(Mathf.Abs(cam.rotation.x) < 30f)
            cam.Rotate(mouseSense * -mouseY, 0, 0, Space.Self);
    }


    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
