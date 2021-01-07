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



    private float wallSlidingSpeed = 1.0f;

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
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
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

    #region Walljump
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (!controller.isGrounded && hit.normal.y < 0.1f)
        {
            playerVelocity = new Vector3(playerVelocity.x, Mathf.Clamp(playerVelocity.y, -wallSlidingSpeed, float.MaxValue), playerVelocity.z);
            if (jumpControl.action.triggered)
            {
                Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

            }
        }
        #endregion
    }
}
