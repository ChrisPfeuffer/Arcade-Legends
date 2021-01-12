using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input Implementation")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference dashControl;

    [Header("CharacterController")]
    private CharacterController controller;

    [Header("Camera")]
    private Transform cameraMainTransform;

    [Header("Movement")]
    [SerializeField] private float playerSpeed = 6f;
    private Vector3 playerVelocity;
    private Vector2 movement;
    private float rotationSpeed = 10f;


    [Header("Jumping Parameters")]
    [SerializeField] private float jumpHeight = 0.7f;
    [SerializeField] private float gravityValue = -15f;
    private bool groundedPlayer;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForce = 1f;
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallSlidingSpeed = 1.0f;
    private bool walljumpingToRight;
    private bool walljumpingToLeft;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashingTime = 0.05f;


    #region - On Enable - 
    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        dashControl.action.Enable();
    }
    #endregion

    #region - On Disable - 
    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        dashControl.action.Disable();
    }
    #endregion

    #region - Start Function - 
    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
    }
    #endregion

    #region - Update Function - 
    void Update()
    {
        movement = movementControl.action.ReadValue<Vector2>();

        PlayerMovementOnGround();
        Wallbounce();
        playerDash();
        Jump();
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

        if (!walljumpingToRight && !walljumpingToLeft)
        {
            MoveTransformation(playerSpeed);
        }
    }
    private void MoveTransformation(float speed)
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * speed);
    }
    #endregion

    #region - Jump -
    private void Jump()
    {
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

    #region - Walljump -
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (!controller.isGrounded && hit.normal.y < 0.1f)
        {
            playerVelocity = new Vector3(playerVelocity.x, Mathf.Clamp(playerVelocity.y, -wallSlidingSpeed, float.MaxValue), playerVelocity.z);
            if (jumpControl.action.triggered && movement.x < 0 && walljumpingToRight == false)
            {
                string walljumpbool = "SetWallJumpToRightFalse";
                walljumpingToRight = true;
                WalljumpheightIfTrue(walljumpbool);


            } 
            else if (jumpControl.action.triggered && movement.x > 0 && walljumpingToLeft == false)
            {
                string walljumpbool = "SetWallJumpToLeftFalse";
                walljumpingToLeft = true;
                WalljumpheightIfTrue(walljumpbool);
            }
        }
        
    }

    private void WalljumpheightIfTrue(string textwalljumpbool)
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.5f * gravityValue);
        //Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);

        Invoke(textwalljumpbool, wallJumpTime);
    }

    private void Wallbounce()
    {

        // Actual wallbounce
        if (walljumpingToRight == true)
        {
            Vector3 move = new Vector3(wallJumpForce, 0, movement.y);
            move = cameraMainTransform.forward * -move.z + cameraMainTransform.right * -move.x;
            move.y = 0f;
            controller.Move(-move * Time.deltaTime * playerSpeed);
        }
        if (walljumpingToLeft == true)
        {
            Vector3 move = new Vector3(-wallJumpForce, 0, movement.y);
            move = cameraMainTransform.forward * -move.z + cameraMainTransform.right * -move.x;
            move.y = 0f;
            controller.Move(-move * Time.deltaTime * playerSpeed);

            Debug.Log(wallJumpForce);
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

    #region - Dash - 
    IEnumerator Dash()
    {
        float startTime = Time.time;

        while(Time.time < startTime + dashingTime)
        {
            MoveTransformation(dashSpeed);

            yield return null;
        }
    }

    private void playerDash()
    {
        if (dashControl.action.triggered)
        {
            StartCoroutine(Dash());
        }
    }
#endregion

    

    
}
