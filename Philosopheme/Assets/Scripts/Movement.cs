using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
 //   public Transform h;

    public float speed = 2f;
    public float strafeCoff = 0.4f;
    public float vertLimit = 65f;
    public float sprintBoost = 2f;
    public const float EPSILON = 0.01f;

    private float pitch;

    // В настройках проекта есть настройка чувствительности мыши, что нужно будет перетащить в настройки игры и эту переменную использовать оттудова
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

    private float speedForce = 0f;

    public void MoveOnGround(float x, float y, bool moveLock)
    {
        // Некорректно на случай, если ГГ что-то сильно пнёт и нужно будет замедлиться
        if (!moveLock && playerRB.velocity.magnitude < speed)
        {
            // F = m * a = m * v / t
            speedForce = (playerRB.mass * (speed - playerRB.velocity.magnitude)) / Time.fixedDeltaTime;
            playerRB.AddForce((transform.forward * y + transform.right * strafeCoff * x) * speedForce, ForceMode.Acceleration);
        }
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
        Physics.Raycast(origin, -transform.up, out hit);

        if (hit.distance < 0.15f && hit.collider != null)
        { 
            playerRB.AddForce(jumpForce * transform.up);
        }
    }

    // Недоработано
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

    public void Sprint(bool moveLock)
    {
        float curSpeed = playerRB.velocity.magnitude;
        if (!moveLock && health.curStamina > 0 && curSpeed <= (sprintBoost * speed))
        {
            speedForce = (playerRB.mass * (speed * sprintBoost - curSpeed)) / Time.fixedDeltaTime;
            playerRB.AddForce(transform.forward * speedForce, ForceMode.Force);

            health.StaminaDrain(health.staminaDrain, true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = transform.GetChild(0);
        playerRB = transform.GetComponent<Rigidbody>();
        health = transform.GetComponent<Health>();

        //Метод нужно унифицировать.. когда-нибудь
        jumpOrigin = (transform.GetChild(1).localScale.y / 2) - EPSILON - transform.GetChild(1).localPosition.y;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 origin2 = transform.localPosition - jumpOrigin * transform.up;
        Debug.DrawRay(origin2, -transform.up, Color.red);
    }
}
//   transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, sprintBoost * speed * Time.deltaTime);