using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")] [Space(5)] 
    [SerializeField] private float m_distanceToInteract;
    [SerializeField] private LayerMask m_interationLayerMask;

    [Header("Climbing")] [Space(20)]
    [SerializeField] private float m_distanceToObject;
    [SerializeField] private float m_climbingTime;
    [SerializeField] private float m_afterClimbOffsetX;
    [SerializeField] private float m_afterClimbOffsetY;

    [Header("Descending")] [Space(20)]
    [SerializeField] private float m_distanceToDescend;
    [SerializeField] private float m_descendTime;
    [SerializeField] private float m_afterDescendOffsetX;
    [SerializeField] private float m_afterDescendOffsetY;

    [Header("Debug Variables")] [Space(20)] 
    [SerializeField] [ReadOnly] public bool m_canControlCharacter = true;
    [SerializeField] [ReadOnly] private bool m_hasClimbed;
    [SerializeField] [ReadOnly] private bool m_canClimb = true;
    [SerializeField] [ReadOnly] private bool m_canDescend = false;
    [SerializeField] [ReadOnly] public Animator m_animator;
    [SerializeField] [ReadOnly] private Rigidbody2D m_rigidBody;
    [SerializeField] [ReadOnly] public CharacterController2D m_controller;
    [SerializeField] [ReadOnly] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] [ReadOnly] private KeyCode jumpKey = KeyCode.Space;


    private void Start()
    {
        m_animator   = GetComponentInChildren<Animator>();
        m_rigidBody  = GetComponent<Rigidbody2D>();
        m_controller = GetComponent<CharacterController2D>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        // Interaction-check Raycast
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y) + Vector2.right * transform.localScale.x * m_distanceToInteract);

        // Climbing-check Raycast
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.55f), new Vector2(transform.position.x, transform.position.y + 0.55f) + Vector2.right * transform.localScale.x * m_distanceToObject);

        // Descending-check Raycast
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y - 0.2f), new Vector2(transform.position.x, transform.position.y - 0.2f) + Vector2.right * transform.localScale.x * m_distanceToDescend);
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Physics2D.queriesStartInColliders = false;
            
            // Interactable raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x,
                m_distanceToInteract, m_interationLayerMask);

            if (hit.collider != null)
            {
                GameObject obj = hit.collider.gameObject;
                Rigidbody2D objRigidBody = obj.GetComponent<Rigidbody2D>();
                FixedJoint2D objFixedJoint = obj.GetComponent<FixedJoint2D>();
                
                if (objRigidBody.bodyType == RigidbodyType2D.Kinematic ||
                    objRigidBody.bodyType == RigidbodyType2D.Static)
                {
                    objRigidBody.bodyType = RigidbodyType2D.Dynamic;
                    objFixedJoint.enabled = true;
                    objFixedJoint.connectedBody = GetComponent<Rigidbody2D>();
                    m_animator.SetBool("isOnRightSide", transform.position.x > obj.transform.position.x);
                    //Debug.Log(transform.position.x + " " + obj.transform.position.x);
                    m_animator.SetBool("isInteracting", true);
                }
                else if (objRigidBody.bodyType == RigidbodyType2D.Dynamic)
                {
                    objRigidBody.bodyType = RigidbodyType2D.Static;
                    objFixedJoint.enabled = false;
                    m_animator.SetBool("isInteracting", false);
                }
            }
        }

        // Climbable raycast check
        if (Input.GetKeyDown(jumpKey))
        {
            Physics2D.queriesStartInColliders = false;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, m_distanceToObject, m_interationLayerMask);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Climbable") && m_canClimb)
            {
                Debug.Log("Climbing");

                m_animator.SetBool("isClimbing", true);
                m_canControlCharacter = false;
                m_hasClimbed = true;
                m_canClimb = false;
                m_canDescend = false;
                m_controller.SetVelocity(Vector2.zero);
                
                StartCoroutine(ClimbCoroutine(m_climbingTime));
            }
        }

        // Descending check
        if (m_canControlCharacter)
        {
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.2f), Vector2.right * transform.localScale.x, m_distanceToDescend, m_interationLayerMask);

            if (hit.collider != null)
            {
                Debug.Log("Descending");

                m_animator.SetBool("isDescending", true);
                m_canControlCharacter = false;
                m_hasClimbed = false;
                m_canClimb = false;
                m_controller.SetVelocity(Vector2.zero);

                StartCoroutine(DescendCoroutine(m_descendTime));
            }
        }
    }

    IEnumerator ClimbCoroutine(float secondsToWait)
    {
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        
        yield return new WaitForSeconds(secondsToWait);

        m_animator.SetBool("isClimbing", false);

        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        m_rigidBody.constraints = RigidbodyConstraints2D.None;
        
        float newPositionY = gameObject.transform.position.y + m_afterClimbOffsetY;
        float newPositionX = gameObject.transform.position.x;
        newPositionX += m_controller.m_isFacingRight ? m_afterClimbOffsetX : -m_afterClimbOffsetX;
        
        gameObject.transform.position = new Vector3(newPositionX, newPositionY, gameObject.transform.position.z);

        m_canControlCharacter = true;
        m_canClimb = true;
        m_canDescend = true;
    }

    IEnumerator DescendCoroutine(float secondsToWait)
    {
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(secondsToWait);

        m_animator.SetBool("isDescending", false);

        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        m_rigidBody.constraints = RigidbodyConstraints2D.None;

        float newPositionY = gameObject.transform.position.y + m_afterDescendOffsetY;
        float newPositionX = gameObject.transform.position.x;
        newPositionX += m_controller.m_isFacingRight ? m_afterDescendOffsetX : -m_afterDescendOffsetX;

        gameObject.transform.position = new Vector3(newPositionX, newPositionY, gameObject.transform.position.z);

        m_canControlCharacter = true;
        m_canDescend = true;
        m_canClimb = true;
    }
}