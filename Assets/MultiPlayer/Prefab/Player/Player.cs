using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class Player : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Camera playerEye;
    [SerializeField] Transform playerSpringArm;
    PlayerInput playerInput;
    Animator animator;
    Vector2 moveInput;
    CharacterController characterController;
    float gravityVelocity;

    float rotationVelocity;
    float targetRot = 0f;
    float rotationSmootheTime = .1f;

    
    [Header("Camera Movement")]
    Vector2 mouseInput;
    float cameraYaw;
    float cameraPitch;


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
            characterController = GetComponent<CharacterController>();
            playerInput.Gameplay.Move.performed += Move;
            playerInput.Gameplay.Move.canceled += Move;
            playerInput.Gameplay.CursorMove.performed += MouseMove;
            playerInput.Gameplay.CursorMove.canceled += MouseMove;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            if(playerEye != null)
            {
                playerEye.enabled = true;
            }
        }
        
        animator = GetComponent<Animator>();
    }

    private void MouseMove(InputAction.CallbackContext obj)
    {
        mouseInput = obj.ReadValue<Vector2>();
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
        float currentMoveSpeed = netMoveInput.Value.magnitude * moveSpeed;
        UpdateCameraRot();
        //on the server and the client this is both called
        //however the move input value is something on the client but is 0 on the server
        if(IsServer)
        {
            if(!characterController.isGrounded)
            {
                gravityVelocity += -9.8f * Time.deltaTime;
            }

            Vector3 inputDir = new Vector3(netMoveInput.Value.x, 0.0f, netMoveInput.Value.y).normalized;
            if(netMoveInput.Value != Vector2.zero)
            {
                targetRot = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + playerSpringArm.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref rotationVelocity, rotationSmootheTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            //transform.position += new Vector3(netMoveInput.Value.x, 0f, netMoveInput.Value.y) * Time.deltaTime * moveSpeed;
            Vector3 targetDir = Quaternion.Euler(0.0f, targetRot, 0.0f) * Vector3.forward;
            characterController.Move(targetDir.normalized * (currentMoveSpeed * Time.deltaTime) + new Vector3(0.0f, gravityVelocity, 0.0f) * Time.deltaTime);

        }

        
        if(animator != null)
        {
            animator.SetFloat("Speed", currentMoveSpeed);
        }

    }

    void UpdateCameraRot()
    {
        Debug.Log("Reached Here");
        float timeMulti = 10 * Time.deltaTime;
        cameraYaw += mouseInput.x * timeMulti;
        cameraPitch += mouseInput.y * timeMulti;

        if(IsServer)
        {
            Debug.Log("currently moving spring arm");
            playerSpringArm.rotation = Quaternion.Euler(-cameraPitch, cameraYaw, 0.0f);
        }
    }
}
