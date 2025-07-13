using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootMovement : MonoBehaviour
{
    //very basic WASD movement using newest Unity Input system
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Movement inputActions;
    private Foot foot;

    private void Awake()
    {
        inputActions = new Movement();
        foot = FindObjectOfType<Foot>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if(!foot.dragging && foot.canKick)
        {
            Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f);
            transform.position += move * moveSpeed * Time.deltaTime;
        }      
    }
}
