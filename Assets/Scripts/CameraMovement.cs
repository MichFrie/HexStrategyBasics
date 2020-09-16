﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    float speed = 0.05f;
    float zoomSpeed = 5.0f;
    float rotateSpeed = 0.5f;

    float minHeight = 4;
    float maxHeight = 40;

    Vector2 p1;
    Vector2 p2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 0.065f;
            zoomSpeed = 15.0f;
        }

        else
        {
            speed = 0.035f;
            zoomSpeed = 5.0f;

        }
        float hsp = transform.position.x * speed * Input.GetAxis("Horizontal");
        float vsp = transform.position.y * speed * Input.GetAxis("Vertical");
        float scrollSpeed = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        if(transform.position.y >= maxHeight && scrollSpeed > 0)
        {
            scrollSpeed = 0;
        }
        else if(transform.position.y <= minHeight && scrollSpeed < 0)
        {
            scrollSpeed = 0;
        }

        if((transform.position.y + scrollSpeed) > maxHeight)
        {
            scrollSpeed = maxHeight - transform.position.y;
        }
        else if((transform.position.y + scrollSpeed) < minHeight)
        {
            scrollSpeed = minHeight - transform.position.y;
        }

        Vector3 verticalMove = new Vector3(0, scrollSpeed, 0);
        Vector3 lateralMove = hsp * transform.right;
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= vsp;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;

        getCameraRotation();

    }

    void getCameraRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            p1 = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            p2 = Input.mousePosition;
        }

        float dx = (p2 - p1).x * rotateSpeed * Time.deltaTime;
        float dy = (p2 - p1).y * rotateSpeed * Time.deltaTime;

        transform.rotation *= Quaternion.Euler(new Vector3(0, dx, 0));
        transform.GetChild(0).transform.rotation *= Quaternion.Euler(new Vector3(-dy, 0, 0));
    }

}

