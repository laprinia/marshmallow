using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalkingLane : MonoBehaviour
{
    public CharacterController2D controller;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, controller.m_walkingLaneY, transform.position.z);
    }
}