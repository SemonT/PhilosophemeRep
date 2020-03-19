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

    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.gameObject;
        move = player.GetComponent<Movement>();
        interaction = GetComponent<Interaction>();
    }

    // Update is called once per frame
    void Update()
    {
    //    bool dodge = false;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        interaction.UpdateWP(Input.GetKeyDown(KeyCode.F));
        npc.npcUpdateContainer.Invoke(Input.GetKey(KeyCode.Q), Input.GetKey(KeyCode.E));

        /*
        if ((x == previousX && x != 0) || (y == previousY && y != 0)) ;
        else dodge = false;

        previousX = x;
        previousY = y;

        move.Dodge(x, y, dodge);
        */
        if (move != null)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.W))
                move.Sprint(moveLock);
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && y > 0) move.Sprint(moveLock);
            if (x != 0 || y != 0) move.MoveOnGround(x, y, moveLock);
            if (mouseX != 0 || mouseY != 0) move.Turn(mouseX, mouseY, cameraLock);
            if (Input.GetKeyDown(KeyCode.Space)) move.Jump();
        }

    }
}
