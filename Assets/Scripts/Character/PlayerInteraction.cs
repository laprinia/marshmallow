using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float distanceToInteract;

    [SerializeField] private LayerMask interactionLayerMask;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position,
            new Vector2(transform.position.x, transform.position.y) +
            Vector2.right * transform.localScale.x * distanceToInteract);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Physics2D.queriesStartInColliders = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x,
                distanceToInteract, interactionLayerMask);
            if (hit.collider != null)
            {
                GameObject obj = hit.collider.gameObject;
                Rigidbody2D objRigidBody = obj.GetComponent<Rigidbody2D>();
                FixedJoint2D objFixedJoint = obj.GetComponent<FixedJoint2D>();
                if (objRigidBody.bodyType == RigidbodyType2D.Kinematic || objRigidBody.bodyType == RigidbodyType2D.Static)
                {
                    objRigidBody.bodyType= RigidbodyType2D.Dynamic;
                    objFixedJoint.enabled = true;
                    objFixedJoint.connectedBody = GetComponent<Rigidbody2D>();
                }else if (objRigidBody.bodyType == RigidbodyType2D.Dynamic)
                {
                    objRigidBody.bodyType= RigidbodyType2D.Static;
                    objFixedJoint.enabled = false;
                }
            }
        }
    }
}