using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Interaction interaction;

    GameObject player;
    public bool cameraLock = false;
    public bool moveLock = false;
    /*
    private float previousX = 0;
    private float previousY = 0;
    */

    private Movement move;

    private float holdFTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.gameObject;
        move = player.GetComponent<Movement>();
        interaction = GetComponent<Interaction>();
    }

    // Управление ГГ переезжает в FixedUpdate()
    void Update()
    {
        //    bool dodge = false;
        /*
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
        */
        if (Input.GetKey(KeyCode.F))
        {
            holdFTimer += Time.deltaTime;
        }
        else
        {
            holdFTimer = 0;
        }
        interaction.UpdateWP(Input.GetKeyDown(KeyCode.F) || holdFTimer > Interaction.instance.holdFTime);





        npc.npcUpdateContainer?.Invoke(Input.GetKey(KeyCode.Q), Input.GetKey(KeyCode.E));

        /*
        if ((x == previousX && x != 0) || (y == previousY && y != 0)) ;
        else dodge = false;

        previousX = x;
        previousY = y;

        move.Dodge(x, y, dodge);
        */
        
        if (move != null)
        {
            if (Input.GetKeyDown(KeyCode.Space)) move.Jump();
        }
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        if (mouseX != 0 || mouseY != 0) move.Turn(mouseX, mouseY, cameraLock);
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");


        if (move != null)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.W))
                move.Sprint(moveLock);
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && y > 0) move.Sprint(moveLock);
            if (x != 0 || y != 0) move.MoveOnGround(x, y, moveLock);
       //     if (Input.GetKeyDown(KeyCode.Space)) move.Jump();
        }
    }
}
