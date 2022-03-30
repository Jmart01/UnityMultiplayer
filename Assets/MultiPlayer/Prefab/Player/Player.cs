using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Camera playerEye;
    
    PlayerInput playerInput;
    Animator animator;
    Vector2 moveInput;

    private NetworkVariable<Vector2> netMoveInput = new NetworkVariable<Vector2>();


    private void Awake()
    {
        if(playerInput == null)
        {
            playerInput = new PlayerInput();
        }
    }

    private void OnEnable()
    {
        if(playerInput != null)
        {
            playerInput.Enable();
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Disable();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("reached here");
        if(IsServer)
        {
            PlayerStart playerStart = FindObjectOfType<PlayerStart>();
            transform.position = playerStart.GetRandomSpawnPos();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)
        {
            playerInput.Gameplay.Move.performed += Move;
            playerInput.Gameplay.Move.canceled += Move;
            if(playerEye != null)
            {
                playerEye.enabled = true;
            }
        }
        
        animator = GetComponent<Animator>();
    }

    private void Move(InputAction.CallbackContext obj)
    {
        OnInputUpdatedServerRpc(obj.ReadValue<Vector2>());
    }

    [ServerRpc]
    private void OnInputUpdatedServerRpc(Vector2 newInputValue)
    {
        netMoveInput.Value = newInputValue;
    }

    // Update is called once per frame
    void Update()
    {
        //on the server and the client this is both called
        //however the move input value is something on the client but is 0 on the server
        if(IsServer)
        {
            transform.position += new Vector3(netMoveInput.Value.x, 0f, netMoveInput.Value.y) * Time.deltaTime * moveSpeed;
        }

        float currentMoveSpeed = netMoveInput.Value.magnitude * moveSpeed;
        if(animator != null)
        {
            animator.SetFloat("Speed", currentMoveSpeed);
        }

    }
}
