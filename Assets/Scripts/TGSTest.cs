using System.Collections;
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
    // Start is called before the first frame update

    void Start()
    {
        tgs = TerrainGridSystem.instance;
       
    }

    private void Update()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        unitPosition = this.transform.position;
        //unitPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
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






    //void FixedUpdate()
    //{
    //    LineOfSight();
    //}

    //void LineOfSight()
    //{
    //    int layerMask = 1 << 8;
    //    RaycastHit[] hits;
    //    layerMask = ~layerMask;
    //    hits = Physics.RaycastAll(transform.position, transform.right, 6f, layerMask);

    //    for (int i = 0; i < hits.Length; i++)
    //    { 
    //        RaycastHit hit = hits[i];
    //        //Debug.Log(hit.transform.gameObject.name);
    //        if(hit.transform.gameObject.tag == "Enemy")
    //        {
    //            //Debug.Log("Enemy");
    //        }
    //        else if(hit.transform.gameObject.tag == "Obstacle")
    //        {
    //            //Debug.Log("Obstacle");
    //        }
    //    }
    //}

