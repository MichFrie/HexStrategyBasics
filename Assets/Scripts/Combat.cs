using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class Combat : MonoBehaviour
{
    TerrainGridSystem tgs;
    public UnitStats unitStats;
    public int strength;
    
    private Ray mouseRay;
    
    private RaycastHit hit;
    private RaycastHit targetRayHit;
    
    private Vector3 unitPosition;
    private Vector3 playerToTargetDirection;

    private float unitMaxFOVangle = 45f;
    private float viewRadius = 5f;

    //CheckWithinCombatDistance Variables
    int distanceValue1, distanceValue2;

    void Start()
    {
        tgs = TerrainGridSystem.instance;

    }


    void Update()
    {
        //CheckWithinMeleeDistance();
        RangedAttack();
        MeasureDistanceFromPlayer();
    }

    private void CheckWithinMeleeDistance()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 unitPosition = transform.position;
            Cell cell = tgs.CellGetAtPosition(unitPosition, true);
            distanceValue1 = tgs.CellGetIndex(cell);
            distanceValue2 = tgs.cellLastClickedIndex;
            if (tgs.CellGetHexagonDistance(distanceValue1, distanceValue2) == 1)
                Debug.Log("Melee Combat Enabled");
            else
                Debug.Log("Too far away for Melee Combat");
        }
    }
    private void MeasureDistanceFromPlayer()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 unitPosition = transform.position;
            Cell cell = tgs.CellGetAtPosition(unitPosition, true);
            distanceValue1 = tgs.CellGetIndex(cell);
            distanceValue2 = tgs.cellLastClickedIndex;
            Debug.Log(tgs.CellGetHexagonDistance(distanceValue1, distanceValue2));
        }
    }

    private void RangedAttack()
    {
        if (Input.GetMouseButtonDown(1) && CheckLineOfSight())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    Debug.Log("initiating Ranged Attack");
                    hit.transform.GetComponent<Combat>().unitStats.strength -= 100;
                }
            }
        }
    }

    private bool CheckLineOfSight()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        unitPosition = this.transform.position;
        playerToTargetDirection = hit.point - unitPosition;
        Vector3 currentViewRadius = this.gameObject.transform.right * viewRadius; 

        if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit))
        {
            float angleBetweenUnits = Vector3.Angle(this.gameObject.transform.position - hit.collider.transform.position, currentViewRadius);
            Debug.Log(angleBetweenUnits);
            if(angleBetweenUnits < unitMaxFOVangle)
            {
                //if (Input.GetMouseButtonDown(1))
                //{
                    Debug.Log(targetRayHit);
                    if (Physics.SphereCast(unitPosition, 1f, playerToTargetDirection.normalized, out targetRayHit, 6f))
                    {
                        if (targetRayHit.collider.gameObject.tag == "Enemy")
                        {
                            Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * targetRayHit.distance, Color.yellow);
                            Debug.Log("Attack");
                            return true;

                        }
                        else
                        {
                            Debug.Log("No attack possible");
                            return false;
                        }
                    }
                    Debug.Log("Out of sight");
                    return false;
                    //Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * 5, Color.white);
                    //Debug.DrawRay(mouseRay.origin, mouseRay.direction * 50, Color.red);
                //}
             
                //return false;
            }
            
        }
        return false;
    }
}
