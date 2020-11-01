﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;
using TreeEditor;
using System;

public class TGSTest : MonoBehaviour
{
    private Ray mouseRay;
    private RaycastHit hit;
    private RaycastHit targetRayHit;
    private Vector3 unitPosition;
    private Vector3 playerToTargetDirection;
    TerrainGridSystem tgs;
    void Start()
    {
        tgs = TerrainGridSystem.instance;

    }

    private void Update()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        unitPosition = this.transform.position;
        playerToTargetDirection = hit.point - unitPosition;

        if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit))
        { //Get the data of the location it hit
            if (Input.GetMouseButton(1))
            {
                if (Physics.SphereCast(unitPosition, 1f, playerToTargetDirection.normalized, out targetRayHit, 6f))
                {
                    if (targetRayHit.collider.gameObject.tag == "Enemy")
                    {
                        Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * targetRayHit.distance, Color.yellow);
                        Debug.Log("Attack");
                    }
                    else
                    {
                        Debug.Log("No attack possible");
                    }
                }
                //Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * 5, Color.white);
                Debug.DrawRay(mouseRay.origin, mouseRay.direction * 50, Color.red);
            }
        }
    }
}

