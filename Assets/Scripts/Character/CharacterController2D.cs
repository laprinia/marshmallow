using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [Header("Parameters - Walking")]
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] [Range(0, 1)] private float m_CrouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] [Range(0, .3f)] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] public float m_walkingLaneY = -3.0f;


    [Header("Parameters - Jumping")] [Space(20)]
    [SerializeField] public float m_JumpForce = 400f; // Amount of force added when the player jumps.
    [SerializeField] public Vector3 jumpRightDir = new Vector3(0.5f, 0.5f, 0f);
    [SerializeField] public float jumpAlignSpeed = 20f;
    [SerializeField] public float landAlignSpeed = 30f;
    

    [Header("Settings")] [Space(20)]
    [SerializeField] public Animator animator;
    [SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider; // A collider that will be disabled when crouching


    [Header("Debug Variables")] [Space(20)]
    [SerializeField] private bool m_AirControl = false; // Whether or not a player can steer while jumping;
    [SerializeField] [ReadOnly] private float horizontalMove = 0f;
    [SerializeField] [ReadOnly] private bool m_Grounded; // Whether or not the player is grounded.
    [SerializeField] [ReadOnly] private bool m_FacingRight = true; // For determining which way the player is currently facing.
    [SerializeField] [ReadOnly] private bool jump = false;
    [SerializeField] [ReadOnly] private bool crouch = false;
    [SerializeField] [ReadOnly] private bool m_wasCrouching = false;


    [Header("Debug Variables - Walking Function")]
    [Space(20)]
    [Help("   sin( time * f\u2081 + p\u2081 )*a\u2081  +  sin( time * f\u2082 + p\u2082 )*a\u2082  +  baseSpeed")]
    [Range(0.5f, 1.5f)] public float baseSpeed = 1;
    [Range(1f, 8f)] public float f1 = 4;
    [Range(1f, 8f)] public float f2 = 6;
    [Range(0f, 6.28f)] public float p1 = 0;
    [Range(0f, 6.28f)] public float p2 = 0;
    [Range(0f, 0.5f)] public float a1 = 0.2f;
    [Range(0f, 0.5f)] public float a2 = 0.15f;


    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 desiredRight = Vector3.right;
    private float rightAlignSpeed = 1f;
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    

    // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW

    /*[Header("Events")]
    [Space(20)]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnCrouchEvent;*/


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator.SetBool("isFacingRight",true);

        // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW

        /*if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();*/
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (horizontalMove != 0)
        {
            animator.SetBool("isFacingRight",horizontalMove>0);
        }
       
     
        
        animator.SetFloat("horizontalMove", Mathf.Abs(horizontalMove));
       // animator.SetFloat("horizontalMove", (horizontalMove));
        
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;

                // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW
                /*if (!wasGrounded)
                    OnLandEvent.Invoke();*/
            }
        }

        transform.right = Vector3.Lerp(transform.right, desiredRight, rightAlignSpeed * Time.fixedDeltaTime);
        float t = Time.time;
        float pacing = a1 * Mathf.Sin(t * f1 + p1) + baseSpeed + Mathf.Sin(t * f2 + p2) * a2;
        Move(horizontalMove * pacing * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
               
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;

                    // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW
                    //OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;

                    // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW
                    //OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight && !animator.GetBool("isInteracting"))
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

            // Tilt the character to face up-right and then down-right
            desiredRight = jumpRightDir.normalized;
            rightAlignSpeed = jumpAlignSpeed;
            Vector3 fallRightDir = jumpRightDir;
            fallRightDir.y = -fallRightDir.y;
            fallRightDir.y = -fallRightDir.y;

            StartCoroutine(ChangeDesiredRight(fallRightDir, landAlignSpeed, 0.25f));
            StartCoroutine(ChangeDesiredRight(Vector3.right, landAlignSpeed, 0.5f));
        }
    }

    IEnumerator ChangeDesiredRight(Vector3 newRight, float alignSpeed, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        desiredRight = newRight;
        rightAlignSpeed = alignSpeed;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}