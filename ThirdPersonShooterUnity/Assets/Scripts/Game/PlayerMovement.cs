using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float playerSpeed = 7.0f;            //  Speed of the player movement
    [SerializeField]
    private float jumpForce = 10.0f;             //  Force of the player jumping
    [SerializeField]
    private float stepDown = 30.0f;              //  Force applied to the player when falling without jumping
    [SerializeField]
    private float pushPower = 2.0f;
    [SerializeField]
    [Tooltip("Fall velocity when the player is on a slope")]
    private float fallSlopeVelocity = 8f;      //  Slope fall velocity affected too by the gravity
    [SerializeField]
    [Tooltip("Force in the Y-axis when the player in on a slope")]
    private float fallSlopeForce = 10.0f;        //  Force that affects in the fall of the player in the slopes

    //  Movement variables
    private Vector3 m_PlayerMovement;   //  Movement Vector
    private Vector3 m_Movement;         //  Input movement Vector
    private float m_Speed;              //  Speed of the player, this changes if the player run
    private float m_Gravity = 20.0f;    //  Gravity
    private float m_FallVelocity;       //  Variable that saves and applies gravity to the player
    private bool m_IsJump;              //  Control if the player has jumped
    //  Slope variables
    private bool m_OnSlope = false;     //  If the player is on a slope
    private Vector3 m_HitNormal;        //  Get the normal of a plane
    //  Camera Variables
    private Vector3 m_CameraForward;    //  Vectors to control camera orientation
    private Vector3 m_CameraSideward;

    private float massRigibody;         //  Mass of the external objects moved by the player

    private Animator m_Animator;            //  Animator component 
    private CharacterController controller; //  Character Controller componet

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        m_Speed = playerSpeed;
    }

    private void Update()
    {
        float horizontalMove = Input.GetAxis("Horizontal");                     //  Get the input axis (w,a,s,d)
        float verticalMove = Input.GetAxis("Vertical");

        m_Movement.Set(horizontalMove, 0f, verticalMove);                       //  Set the movement vector with the input
        m_Movement = Vector3.ClampMagnitude(m_Movement, 1f);

        bool hasHorizontalInput = !Mathf.Approximately(horizontalMove, 0f);     //  Approximates and equalizes the input value if it is zero
        bool hasVerticalInput = !Mathf.Approximately(verticalMove, 0f);
        bool isMove = hasHorizontalInput || hasVerticalInput;                   //  Determine whether or not the player is movement

        CameraDirection();
        //  Set the player movement vector multiply the movement vector and the currently speed of the player
        m_PlayerMovement = (m_Movement.x * m_CameraSideward + m_Movement.z * m_CameraForward) * m_Speed;

        Gravity();
        Jump();
        //  If the player jumped, the extra fall force does not apply
        if (controller.isGrounded && !m_IsJump)
        {
            m_PlayerMovement = m_PlayerMovement + Vector3.down * stepDown;      //  Apply more force when the player fall
        }

        controller.Move((m_PlayerMovement) * Time.deltaTime);                 //  Move the player using the character controller
        //Debug.Log(controller.velocity.magnitude);
        m_IsJump = !controller.isGrounded;

        if (isMove)
        {
            //  If the player is movement, then do an action
            Action();
        }
        else
        {
            m_Animator.SetFloat("ForwardMove", 0);        //  When the player is not moving or the input vector does not have any value, the player play the idle animation in zero
            m_Animator.SetFloat("SideMove", 0);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_HitNormal = hit.normal;                   //  Take the normal of the plane where the player is walking

        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        massRigibody = body.mass;

        // Apply the push
        body.linearVelocity = pushDir * pushPower / massRigibody;
    }

    private void Action()
    {
        /*
         *  Determine if the player is walking, running, etc
         *  Play the animations for each action
         */
        if (Input.GetButton("Sprint"))
        {
            if (m_Movement.z > 0 && !m_Animator.GetBool("IsAiming"))
            {
                m_Speed = playerSpeed * 1.5f;           //  Multiply the initial speed to be able to run, if the speed change the blendtree of the animation have to change as well
                m_Animator.SetFloat("ForwardMove", (m_Movement.z * 2));    //  The animator controller take the speed
            }
            
            m_Animator.SetFloat("SideMove", (m_Movement.x));
        }
        else
        {
            m_Speed = playerSpeed;
            m_Animator.SetFloat("ForwardMove", m_Movement.z);
            m_Animator.SetFloat("SideMove", m_Movement.x);
        }
    }

    private void CameraDirection()
    {
        /*
         *  Get the camera orientation to make the player move with the camera orientation
         *  Forward and sideward
         * */
        m_CameraForward = Camera.main.transform.forward;
        m_CameraSideward = Camera.main.transform.right;

        m_CameraForward.y = 0;
        m_CameraSideward.y = 0;

        m_CameraForward = m_CameraForward.normalized;
        m_CameraSideward = m_CameraSideward.normalized;
    }

    private void Gravity()
    {
        /*
         *  Gravity applies to the player
         *  Take the gravity variable nad it is apply when the player is on the ground or when the player jumps
         * */
        if (controller.isGrounded)
        {
            m_FallVelocity = -m_Gravity * Time.deltaTime;
            m_PlayerMovement.y = m_FallVelocity;
        }
        else
        {
            m_FallVelocity -= m_Gravity * Time.deltaTime;
            m_PlayerMovement.y = m_FallVelocity;
            m_Animator.SetFloat("AirSpeed", controller.velocity.y);
            //  control if the player is falling for a long time
            if(controller.velocity.y <= -25 || controller.velocity.y >= 20)
            {
                m_Animator.SetTrigger("Jump");
            }
        }
        m_Animator.SetBool("IsGrounded", controller.isGrounded);
        FallSlope();
    }

    private void Jump()
    {
        /*
         *  When the player press the space bar the jump force is apply to the player
         * */
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            m_IsJump = true;
            m_FallVelocity = jumpForce;                 //  Set the fall velocity when the player jump with the jump force to be able to the player goes up
            m_PlayerMovement.y = m_FallVelocity;
            m_Animator.SetTrigger("Jump");              //  Set the trigger in the animator to jump
        }
    }

    private void FallSlope()
    {
        /*
         *  When the player is in a slope the player goes down,
         *  calculating the angle between the Y-axis and the normal slope and 
         *  comparing it with the slope limit of the character's controller to know 
         *  if the player has to fall down the slope that he cannot climb
         *  and compare if the slope is a platform with a 90 degrees plane to nor allow
         *  the player to fall down from that slope
         * */
        m_OnSlope = Vector3.Angle(m_HitNormal, Vector3.up) > controller.slopeLimit && Vector3.Angle(m_HitNormal, Vector3.up) < 89;

        if (m_OnSlope)
        {
            /*
             * Set the player position applying a the fall velocity to every X-Z axis calculating the inclination of the slope
             * And apply a force on the Y-axis to avoid bouncing
             * */
            m_PlayerMovement.x += ((1f - m_HitNormal.y) * m_HitNormal.x) * fallSlopeVelocity;
            m_PlayerMovement.z += ((1f - m_HitNormal.y) * m_HitNormal.z) * fallSlopeVelocity;
            m_PlayerMovement.y -= fallSlopeForce;
        }
    }

    private void OnAnimatorMove()
    {
    }
}
