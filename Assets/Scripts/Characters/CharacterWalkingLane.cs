using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649, 0414

public class CharacterWalkingLane : MonoBehaviour
{
    public CharacterController2D controller;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, controller.m_walkingLaneY, transform.position.z);
    }
}