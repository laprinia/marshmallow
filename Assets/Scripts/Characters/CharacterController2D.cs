using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649, 0414

public class CharacterController2D : MonoBehaviour
{
    [Header("Parameters - Walking")]
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] [Range(0, 1)] private float m_CrouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField] [Range(0, .3f)] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] public float m_walkingLaneY = -3.0f;
    

    [Header("Settings")] [Space(20)]
    [SerializeField] private LayerMask  m_groundLayer; // A mask determining what is ground to the character
    [SerializeField] private Transform  m_groundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform  m_ceilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_crouchDisabledCollider; // A collider that will be disabled when crouching


    [Header("Debug Variables")] [Space(20)]
    [SerializeField] [ReadOnly] private bool m_isGrounded; // Whether or not the player is grounded.
    [SerializeField] [ReadOnly] public bool m_isFacingRight = true; // For determining which way the player is currently facing.
    [SerializeField] [ReadOnly] private bool m_isCrouching = false;
    [SerializeField] [ReadOnly] private bool m_wasCrouching = false;
    [SerializeField] [ReadOnly] private bool m_isClimbing = false;
    [SerializeField] [ReadOnly] private Rigidbody2D m_Rigidbody2D;
    [SerializeField] [ReadOnly] private Animator    m_animator;


    [Header("Debug Variables - Walking Function")] [Space(20)]
    [Help("   sin( time * f\u2081 + p\u2081 )*a\u2081  +  sin( time * f\u2082 + p\u2082 )*a\u2082  +  baseSpeed")]
    [Range(0.0f, 1.5f)] public float  m_baseSpeed = 1.0f;
    [Range(1f, 8f)]     public float  m_f1 = 4.0f;
    [Range(1f, 8f)]     public float  m_f2 = 6.0f;
    [Range(0f, 6.28f)]  public float  m_p1 = 0.0f;
    [Range(0f, 6.28f)]  public float  m_p2 = 0.0f;
    [Range(0f, 0.5f)]   public float  m_a1 = 0.2f;
    [Range(0f, 0.5f)]   public float  m_a2 = 0.15f;


    private Vector3     m_Velocity = Vector3.zero;
    private Vector3     m_desiredRight = Vector3.right;
    private float       m_horizontalMove = 0.0f;
    private float       m_rightAlignSpeed = 1.0f;
    private const float k_GroundedRadius = 0.2f; // Radius of the overlap circle to determine if grounded
    private const float k_CeilingRadius = 0.2f; // Radius of the overlap circle to determine if the player can stand up
    

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
        m_animator = GetComponentInChildren<Animator>();
        m_animator.SetBool("isFacingRight",true);

        // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW

        /*if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();*/
    }

    private void Update()
    {
        m_horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        m_animator.SetFloat("horizontalMove", Mathf.Abs(m_horizontalMove));

        if (m_horizontalMove != 0)
        {
            m_animator.SetBool("isFacingRight",m_horizontalMove>0);
        }

        if (Input.GetButtonDown("Crouch"))
        {
            m_isCrouching = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            m_isCrouching = false;
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_isGrounded;
        m_isGrounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_groundCheck.position, k_GroundedRadius, m_groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_isGrounded = true;

                // COMMENTED UNTIL FURTHER USE FOR A BETTER VISIBILITY IN THE INSPECTOR WINDOW
                /*if (!wasGrounded)
                    OnLandEvent.Invoke();*/
            }
        }

        if (!m_animator.GetBool("isClimbing"))
        {
            
            m_isClimbing = false;

            float t = Time.time;
            float pacing = m_a1 * Mathf.Sin(t * m_f1 + m_p1) + m_baseSpeed + Mathf.Sin(t * m_f2 + m_p2) * m_a2;

            transform.right = Vector3.Lerp(transform.right, m_desiredRight, m_rightAlignSpeed * Time.fixedDeltaTime);
            Move(m_horizontalMove * pacing * Time.fixedDeltaTime, m_isCrouching);
        }
        else
        {
            m_isClimbing = true;
        }
    }

    public void Move(float move, bool crouch)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_ceilingCheck.position, k_CeilingRadius, m_groundLayer))
            {
                crouch = true;
            }
        }

        //only control the player if grounded is turned on
        if (m_isGrounded)
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
                if (m_crouchDisabledCollider != null)
                    m_crouchDisabledCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_crouchDisabledCollider != null)
                    m_crouchDisabledCollider.enabled = true;

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
            if (move > 0 && !m_isFacingRight && !m_animator.GetBool("isInteracting"))
            {
                Flip();
            }

            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_isFacingRight && !m_animator.GetBool("isInteracting"))
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_isFacingRight = !m_isFacingRight;
        
        // Multiply the player's x local scale by -1.
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    // Getters
    public float GetHorizontalMove()
    {
        return m_horizontalMove;
    }
    public Vector2 GetVelocity()
    {
        return m_Rigidbody2D.velocity;
    }
    public Vector3 GetPosition()
    {
        return m_Rigidbody2D.transform.position;
    }

    // Setters
    public void SetHorizontalMove(float horizontalMove)
    {
        m_horizontalMove = horizontalMove;
    }
    public void SetVelocity(Vector2 velocity)
    {
        m_Rigidbody2D.velocity = velocity;
    }
}