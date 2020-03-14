using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject player;
    private Movement move;
    Interaction interaction;
    // Start is called before the first frame update
    void Start()
    {
        move = player.GetComponent<Movement>();
        interaction = GetComponent<Interaction>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        if (x != 0 || y != 0)  move.MoveOnGround(x, y);
        if (mouseX != 0 || mouseY != 0) move.Turn(mouseX, mouseY);
        interaction.UpdateWP(Input.GetKeyDown(KeyCode.E));

    }
}
