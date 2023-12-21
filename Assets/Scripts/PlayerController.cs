using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    private Vector3 moveDirection;

    private float speed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;

    [SerializeField] private float jumpForce = 5f;
    private float ySpeed;
    private float originalStepOffset;
    [SerializeField] private float jumpButtonGracePeriod = 0.2f;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    //private bool isGrounded;

    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 2f;   
    private bool isCrouching;
   
    private bool isSprinting;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        speed = moveSpeed;
        originalStepOffset = characterController.stepOffset;
    }
    void Update()
    {
        ApplyGravity();
        MoveThePlayer();
        Sprint();
        Jump();
        Crouch();
    }
    void ApplyGravity()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;
    }
    void MoveThePlayer()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
        }
        moveDirection.y = ySpeed;
        moveDirection *= speed * Time.deltaTime;
        characterController.Move(moveDirection);

        moveDirection.y = 0f;

        float velocityZ = Vector3.Dot(moveDirection.normalized, transform.forward);
        float velocityX = Vector3.Dot(moveDirection.normalized, transform.right);
        animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }
    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
            isSprinting = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = moveSpeed;
            isSprinting = false;
        }
        animator.SetBool("isSprinting", isSprinting);
    }
    void Jump()
    {
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            animator.SetBool("isGrounded", true);
            //isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpForce;
                animator.SetBool("isJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0f;
            animator.SetBool("isGrounded", false);
            //isGrounded = false;

            if ((isJumping && ySpeed < 0)||ySpeed<-2)
            {
                animator.SetBool("isFalling", true);
            }
        }
    }
    void Crouch()
    {
        if(characterController.isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            { 
                speed=crouchSpeed;
                animator.SetBool("isCrouching", true);
            }
            if(Input.GetKeyUp(KeyCode.LeftControl))
            {
                speed = moveSpeed;
                animator.SetBool("isCrouching", false);
            }
        }
    }
}
