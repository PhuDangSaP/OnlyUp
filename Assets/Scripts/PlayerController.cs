using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private Transform mainCamera;

    private Vector3 moveDirection;

    private float speed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [SerializeField] private float jumpForce = 5f;
    private float ySpeed;

    [SerializeField] private float jumpButtonGracePeriod = 0.2f;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    //private bool isGrounded;

    private float crouchHeight = 1.3f;
    private float standHeight = 1.5f;
    private float crouchCenter = 0.6f;
    private float standCenter = 0.81f;
    private bool isCrouching;

    private bool isSprinting;

    private readonly float DistanceCanClimbLedge = 0.2f;
    [SerializeField] private LayerMask vaultLayer;
    private bool canClimb;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        speed = moveSpeed;

    }
    void Update()
    {
        ApplyGravity();
        MoveThePlayer();
        Sprint();
        Jump();
        Crouch();
        Climb();
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
            moveDirection = Quaternion.Euler(0f, mainCamera.rotation.eulerAngles.y, 0f) * moveDirection;
            float characterRotationAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, mainCamera.rotation.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, characterRotationAngle, 0f);
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
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && Input.GetAxis("Vertical") > 0)
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
            animator.SetBool("isGrounded", false);
            //isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("isFalling", true);
            }
        }
    }


    void Crouch()
    {
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isCrouching = !isCrouching;
                if (isCrouching)
                {
                    speed = crouchSpeed;
                    characterController.height = crouchHeight;
                    characterController.center = new Vector3(0, crouchCenter, 0);
                }
                else
                {
                    speed = moveSpeed;
                    characterController.height = standHeight;
                    characterController.center = new Vector3(0, standCenter, 0);
                }
                animator.SetBool("isCrouching", isCrouching);
            }

        }
    }


    void Climb()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Physics.Raycast(transform.position + Vector3.up * 1.7f, transform.forward, out var firstHit, 1f, vaultLayer))
            {
                if (!Physics.Raycast(transform.position+Vector3.up*2f,transform.forward,out var check,1f,vaultLayer))
                {
                    if (Physics.Raycast(firstHit.point + transform.forward * 0.15f + Vector3.up * 2, Vector3.down, out var secondHit))
                    {
                        print(secondHit.point);
                        GameObject.Find("Sphere").gameObject.transform.position = secondHit.point;
                        //characterController.Move(secondHit.point);
                    }
                }

            }
        }
    }


}
