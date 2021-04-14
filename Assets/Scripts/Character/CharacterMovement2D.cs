using System;
using UnityEngine;
using UnityEditor;

public class CharacterMovement2D : MonoBehaviour
{
    [Header("Parameters - Movement")] 
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private float runSpeed = 40f;


    [Header("Debug Variables")] [Space(20)] 
    [SerializeField] [ReadOnly] private float horizontalMove = 0f;
    [SerializeField] [ReadOnly] private bool jump = false;
    [SerializeField] [ReadOnly] private bool crouch = false;
    [Space(20)]
    [SerializeField] public Animator animator;
    

    [Header("Debug Variables - Walking Function")] [Space(20)]
    [Help("   sin( time * f\u2081 + p\u2081 )*a\u2081  +  sin( time * f\u2082 + p\u2082 )*a\u2082  +  baseSpeed")]
    [Range(0.5f, 1.5f)] public float baseSpeed = 1;
    [Range(1f,8f)] public float f1 = 4;
    [Range(1f,8f)] public float f2 = 6;
    [Range(0f,6.28f)] public float p1 = 0;
    [Range(0f,6.28f)] public float p2 = 0;
    [Range(0f,0.5f)]  public float a1 = 0.2f;
    [Range(0f,0.5f)] public float a2 = 0.15f;  
    

    // Update is called once per frame
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("horizontalMove",Mathf.Abs(horizontalMove));
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
        float t = Time.time;
        float pacing = a1 * Mathf.Sin(t * f1 + p1) + baseSpeed + Mathf.Sin(t * f2 + p2) * a2;
        controller.Move(horizontalMove * pacing * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}