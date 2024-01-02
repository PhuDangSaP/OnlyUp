using System.Collections;
using System.Collections.Generic;
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
    private bool isClimbing;

    [SerializeField] private LayerMask obstacleLayer;
    private bool playerInAction;

    private bool playerControl = true;

    [Header("ActionArea")]
    public List<PlayerAction> PlayerActions;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        speed = moveSpeed;

    }
    void Update()
    {
        MoveThePlayer();
        if (!playerControl)
            return;
        ApplyGravity();
        Sprint();
        Crouch();
        Jump();
        Action();
        //Climb();
        CheckJumpIfBed();
        SlowMotion();

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

            if (characterController.isGrounded)
            {
                if (isCrouching)
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerCrouch, 0.1f);
                }
                else if (isSprinting)
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerSprint, 1f);
                }
                else
                {
                    SoundManager.PlaySound(SoundManager.Sound.PlayerWalk, Random.Range(0.2f, 0.6f));

                }

            }
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
    void Jump()
    {
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ObstacleChecker.ObstacleInfo hit1 = ObstacleChecker.CheckObstacle(transform.position, transform, 0.9f, obstacleLayer);
            ObstacleChecker.ObstacleInfo hit2 = ObstacleChecker.CheckClimb(transform.position, transform, 0.9f, obstacleLayer);
            if (!hit1.hitFound&&!hit2.hitFound)
            {
                jumpButtonPressedTime = Time.time;
            }

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
                animator.SetBool("isJumping", true);
                ySpeed = jumpForce;
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
    void Action()
    {
        if ((Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) || animator.GetBool("isFalling"))
        {
            ObstacleChecker.ObstacleInfo hitdata = ObstacleChecker.CheckClimb(transform.position, transform, 0.5f, obstacleLayer);
            if (hitdata.hitFound)
            {
                foreach (var action in PlayerActions)
                {
                    if (action.name == "Climb")
                    {
                        if (action.CheckIfAvailable(hitdata, transform))
                        {
                            if (animator.GetBool("isFalling"))
                                animator.SetBool("isFalling", false);
                            StartCoroutine(PerformAction(action));
                            return;
                        }
                    }
                }
            }
        }
        if ((Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) && !playerInAction)
        {
            ObstacleChecker.ObstacleInfo hitdata = ObstacleChecker.CheckObstacle(transform.position, transform, 0.9f, obstacleLayer);
            if (hitdata.hitFound)
            {
                foreach (var action in PlayerActions)
                {
                    if (action.CheckIfAvailable(hitdata, transform))
                    {
                        StartCoroutine(PerformAction(action));
                        break;
                    }
                }
            }
        }
    }
    IEnumerator PerformAction(PlayerAction action)
    {
        playerInAction = true;
        SetControl(false);

        animator.CrossFade(action.AnimationName, 0.2f);
        yield return null;
        yield return new WaitForSeconds(animator.GetNextAnimatorStateInfo(0).length);
        SetControl(true);
        playerInAction = false;
    }

    //void Climb()
    //{
    //    if ((Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) || animator.GetBool("isFalling"))
    //    {
    //        if (Physics.Raycast(transform.position + Vector3.up * 1.3f, transform.forward, out var firstHit, 1f, obstacleLayer))
    //        {
    //            if (!Physics.Raycast(transform.position + Vector3.up * 1.7f, transform.forward, out var check, 1f, obstacleLayer))
    //            {
    //                if (Physics.Raycast(firstHit.point + transform.forward * 0.2f + Vector3.up * 2, Vector3.down, out var secondHit))
    //                {
    //                    GameObject.Find("Sphere").gameObject.transform.position = secondHit.point;
    //                    StartCoroutine(ClimbAnimation(secondHit.point + transform.forward * 0.7f));
    //                }
    //            }
    //        }
    //    }
    //}
    //IEnumerator ClimbAnimation(Vector3 targetPos)
    //{
    //    isClimbing = true;
    //    animator.SetTrigger("Climb");
    //    yield return new WaitForSeconds(1.5f);
    //    isClimbing = false;
    //}
    void PlayLandingSound()
    {
        SoundManager.PlaySound(SoundManager.Sound.Landing);
    }

    void CheckJumpIfBed()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, 1))
        {
            if (hit.collider.gameObject.tag == "Bed")
            {
                ySpeed = 10;
                isJumping = true;
                SoundManager.PlaySound(SoundManager.Sound.BedBounce, 0.8f);
            }

        }
    }
    void SlowMotion()
    {
        if (Input.GetMouseButton(1))
        {
            Time.timeScale = 0.3f;
        }
        else Time.timeScale = 1;
    }
    void SetControl(bool hasControl)
    {
        this.playerControl = hasControl;
        characterController.enabled = hasControl;
        if (!hasControl)
        {
            animator.SetFloat("VelocityX", 0);
            animator.SetFloat("VelocityZ", 0);
        }
    }

}
