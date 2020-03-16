using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 2f;
    public float strafeCoff = 0.4f;
    public float vertLimit = 65f;
    public float sprintBoost = 2f;

    private float pitch;

    // В настройках проекта есть настрока чувствительности мыши, что нужно будет перетащить в настройки игры и эту переменную использовать оттудова
    public float mouseSense = 1.5f;
    public float jumpForce = 100f;
    public float dodgeForce = 500f;

    [Range(0.5f, 1f)] float maxTimer = 0.5f;
    private int pressCount = 0;
    private float secondTimer = 0f;

    private float jumpOrigin;
    private Transform cam;
    private Rigidbody playerRB;
    private Health health;

    // Надо переписать через RigidBody, иначе проходит сквозь объекты
    public void MoveOnGround(float x, float y, bool moveLock)
    {
        if (!moveLock) 
            transform.position = Vector3.MoveTowards(transform.position, transform.position + strafeCoff * transform.right * x + transform.forward * y, speed * Time.deltaTime);
    }

    public void Turn(float mouseX, float mouseY, bool cameraLock)
    {
        if (!cameraLock) transform.Rotate(0, mouseSense * mouseX, 0);

        pitch -= mouseSense * mouseY;
        pitch = Mathf.Clamp(pitch, -vertLimit, vertLimit);

        cam.localEulerAngles = new Vector3(pitch, 0, 0);   
    }

    public void Jump()
    {
        Vector3 origin = transform.localPosition - jumpOrigin * transform.up;

        RaycastHit hit;
        Physics.Raycast(origin, origin - transform.up, out hit, 0.15f);

        if (hit.collider)
        {
            playerRB.AddForce(jumpForce * transform.up);
        }
    }

    public void Dodge(float x, float y, bool pressed)
    {
        if (pressed) pressCount++;

        if(secondTimer > maxTimer)
        {
            secondTimer = 0f;
            pressCount = 0;
        }

        secondTimer += Time.deltaTime;
        if(pressCount == 2)
        {
            playerRB.AddForce(dodgeForce * new Vector3(x, y, 0));
        }
    }

    // Надо переписать через RigidBody, иначе проходит сквозь объекты
    public void Sprint(bool moveLock)
    {
        if (!moveLock && health.curStamina > 0)
        {
            health.ResetStaminaTimer();
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, sprintBoost * speed * Time.deltaTime);

            health.curStamina -= health.staminaDrain * Time.deltaTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        playerRB = transform.GetComponent<Rigidbody>();
        health = transform.GetComponent<Health>();

        jumpOrigin = (transform.GetChild(1).localScale.y / 2) - 0.01f; 
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 origin = transform.localPosition - jumpOrigin * transform.up;
        Debug.DrawLine(origin, origin - 0.1f * transform.up, Color.red);
        */
    }
}
