using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

#pragma warning disable 0649, 0414

public class CameraManager : MonoBehaviour
{
    [Header("Orientation Blending")] [Space(10)]
    [SerializeField] private float blendFactor = 0.0005f;
    [SerializeField] private float facingRightScreenX = 0.374f;
    [SerializeField] private float facingLeftScreenX = 0.626f;
    [SerializeField] [ReadOnly] private float currentScreenX = 0.374f;

    [Header("Borders")] [Space(10)]
    [SerializeField] private float leftBorder = -81.0f;
    [SerializeField] [ReadOnly] private float oldLeftBorder;
    [SerializeField] [ReadOnly] private float currentDeadZoneWidth;

    [Header("Debug Variables")] [Space(10)]
    [SerializeField] [ReadOnly] private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] [ReadOnly] private CharacterController2D characterController;
    [SerializeField] [ReadOnly] private CinemachineFramingTransposer cameraSettings;
  
    void Start()
    {
        cameraSettings = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        characterController = GetComponent<CharacterController2D>();

        oldLeftBorder = leftBorder;
        currentDeadZoneWidth = cameraSettings.m_DeadZoneWidth;

        // The camera DZH is set by default to 0 to force the camera centered inside the level background
        // And then set it back to 1.0f to never change its position on the Y axis
        cameraSettings.m_DeadZoneHeight = 1.0f;
    }

    void Update()
    {
        // Modify the Dead Zone Width if a border is nerby
        if (gameObject.transform.position.x < leftBorder)
        {
            StartCoroutine(LerpDeadZoneWidth(1.0f));
        }
        else
        {
            if (gameObject.transform.position.x > leftBorder && currentDeadZoneWidth > 0.9f)
            {
                StartCoroutine(LerpDeadZoneWidth(0.1f));
            }
        }

        // Modify the character position inside the camera based on its orientation
        if (gameObject.transform.position.x > (oldLeftBorder + 5.0f))
        {
            if (characterController.m_isFacingRight && currentScreenX > facingRightScreenX)
            {
                leftBorder = oldLeftBorder;
                currentScreenX -= blendFactor;
            }

            if (!characterController.m_isFacingRight && currentScreenX < facingLeftScreenX)
            {
                leftBorder = oldLeftBorder + 6.0f;
                currentScreenX += blendFactor;
            }

            cameraSettings.m_ScreenX = currentScreenX;
        }
    }

    IEnumerator LerpDeadZoneWidth(float newDeadZoneWidth)
    {
        if(newDeadZoneWidth > currentDeadZoneWidth)
        {
            while (currentDeadZoneWidth < newDeadZoneWidth)
            {
                cameraSettings.m_DeadZoneWidth += blendFactor;
                currentDeadZoneWidth = cameraSettings.m_DeadZoneWidth;
                yield return null;
            }
        }
        else
        {
            while (currentDeadZoneWidth > newDeadZoneWidth)
            {
                cameraSettings.m_DeadZoneWidth -= blendFactor;
                currentDeadZoneWidth = cameraSettings.m_DeadZoneWidth;
                yield return null;
            }
        }
    }
}
