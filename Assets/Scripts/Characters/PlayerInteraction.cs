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
    [SerializeField] private float m_distanceToDescend;
    [SerializeField] private float m_heightToClimb;
    [SerializeField] private float m_distanceToClimb;
    [SerializeField] private float m_climbingTime;
    [SerializeField] private float m_beforeClimbOffsetX;
    [SerializeField] private float m_beforeClimbOffsetY;
    [SerializeField] private float m_afterClimbOffsetX;
    [SerializeField] private float m_afterClimbOffsetY;

    [Header("Debug Variables")] [Space(20)] 
    [SerializeField] [ReadOnly] private bool m_canControlCharacter = true;
    [SerializeField] [ReadOnly] private bool m_hasClimbed;
    [SerializeField] [ReadOnly] private Animator m_animator;
    [SerializeField] [ReadOnly] private Rigidbody2D m_rigidBody;
    [SerializeField] [ReadOnly] private CharacterController2D m_controller;
    [SerializeField] [ReadOnly] private float m_horizontalMove;
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
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x, transform.position.y) +
            Vector2.right * transform.localScale.x * m_distanceToInteract);
    }

    private void Update()
    {
        m_horizontalMove = m_controller.GetHorizontalMove();
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

        if (Input.GetKeyDown(jumpKey))
        {
            Physics2D.queriesStartInColliders = false;
            // Climbable raycast check
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x,
                m_distanceToClimb, m_interationLayerMask);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Climbable"))
            {
                Debug.Log("Has found climbable");
                m_animator.SetBool("isClimbing", true);
                m_canControlCharacter = false;
                m_controller.SetVelocity(Vector2.zero);
                m_hasClimbed = true;
                StartCoroutine(WaitCoroutine(m_climbingTime, hit.collider.gameObject));
            }
        }

        // Descending check
        if (m_hasClimbed && m_canControlCharacter)
        {
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x,
                m_distanceToClimb, m_interationLayerMask);

            if (hit.collider != null && Vector2.Distance(hit.collider.transform.position, transform.position) <
                m_distanceToDescend)
            {
                hit.collider.enabled = false;
                var position = transform.position;
                //Debug.Log(Vector2.Distance(hit.collider.transform.position, position));
                position = new Vector3(position.x + 2, -3.2561f, 0);
                transform.position = position;
            }
        }
    }

    IEnumerator WaitCoroutine(float secondsToWait, GameObject hitGO)
    {
        m_rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + m_beforeClimbOffsetY,
            gameObject.transform.position.y + m_beforeClimbOffsetX, gameObject.transform.position.z);
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

        //add restrictive colliders
        BoxCollider2D[] childrenColliders = hitGO.GetComponentsInChildren<BoxCollider2D>();
        foreach (var collider in childrenColliders)
        {
            collider.enabled = true;
        }

        m_canControlCharacter = true;
    }
}