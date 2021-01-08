using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    
    private float playerSpeed = 6f;
    private float jumpHeight = 0.7f;
    private float gravityValue = -15f;
    private float rotationSpeed = 10f;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;



    private float wallJumpForce = 1.15f;
    private float wallJumpTime = 0.2f;
    private float wallSlidingSpeed = 1.0f;
    private bool walljumpingToRight;
    private bool walljumpingToLeft;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }
    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
    }

    void Update()
    {
        PlayerMovementOnGround();

    }

    #region - Walljump -
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (!controller.isGrounded && hit.normal.y < 0.1f)
        {
            Vector2 movement = movementControl.action.ReadValue<Vector2>();
            playerVelocity = new Vector3(playerVelocity.x, Mathf.Clamp(playerVelocity.y, -wallSlidingSpeed, float.MaxValue), playerVelocity.z);
            if (jumpControl.action.triggered && movement.x < 0)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.5f * gravityValue);
                //Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
                walljumpingToRight = true;
                Invoke("SetWallJumpToRightFalse", wallJumpTime);
            } 
            else if (jumpControl.action.triggered && movement.x > 0)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.5f * gravityValue);
                //Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
                walljumpingToLeft = true;
                Invoke("SetWallJumpToLeftFalse", wallJumpTime);
            }
        }
        
    }

    private void Wallbounce(Vector2 movementWall)
    {

        // Actual wallbounce
        if (walljumpingToRight == true)
        {
            Vector3 move = new Vector3(wallJumpForce, 0, movementWall.y);
            move = cameraMainTransform.forward * -move.z + cameraMainTransform.right * -move.x;
            move.y = 0f;
            controller.Move(-move * Time.deltaTime * playerSpeed);
        }
        if (walljumpingToLeft == true)
        {
            Vector3 move = new Vector3(-wallJumpForce, 0, movementWall.y);
            move = cameraMainTransform.forward * -move.z + cameraMainTransform.right * -move.x;
            move.y = 0f;
            controller.Move(-move * Time.deltaTime * playerSpeed);
        }

    }
    private void SetWallJumpToRightFalse()
    {
        walljumpingToRight = false;
    }
    private void SetWallJumpToLeftFalse()
    {
        walljumpingToLeft = false;
    }
    #endregion

    #region - PlayerMovementOnGround -
    private void PlayerMovementOnGround()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector2 movement = movementControl.action.ReadValue<Vector2>();

        if (!walljumpingToRight && !walljumpingToLeft)
        {
            Vector3 move = new Vector3(movement.x, 0, movement.y);
            move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
            move.y = 0f;
            controller.Move(move * Time.deltaTime * playerSpeed);
        }
        Debug.Log(movement);
        Wallbounce(movement);

        // Makes player jump
        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

    }
    #endregion
}
