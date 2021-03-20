using TGS;
using UnityEngine;

public class Combat : MonoBehaviour
{
    TerrainGridSystem tgs;
    UnitMovement unitMovement;
    public UnitStats unitStats;

    public int strength;
    int attackingUnit;
    int defendingUnit;

    Ray mouseRay;

    RaycastHit hit;
    RaycastHit targetRayHit;

    Vector3 unitPosition;
    Vector3 playerToTargetDirection;

    Quaternion unitRotation;

    Vector3 currentViewRadius;
    int unitMaxFovAngle = 70;
    int maxRange = 3; 

    void Start()
    {
        tgs = TerrainGridSystem.instance;
    }


    void Update()
    {
        CheckAngleToTarget();
    }


    void CheckAngleToTarget()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float angleBetweenUnits = 0f;
            if (Physics.Raycast(ray, out hit))
            {
                angleBetweenUnits = Vector3.Angle(hit.transform.position - this.transform.position, transform.forward);
            }
            int degree = Mathf.RoundToInt(angleBetweenUnits);
            if (degree < unitMaxFovAngle)
            {
                Debug.Log($"{degree} - possible");
                CheckDistanceToTarget(hit);
            }
            else
            {
                Debug.Log($"{degree} - not possible");
            }

        }

        void CheckDistanceToTarget(RaycastHit hit)
        {
            Vector3 unitPosition = this.transform.position;
            Vector3 targetPosition = hit.transform.position;
            attackingUnit = tgs.CellGetIndex(tgs.CellGetAtPosition(unitPosition, true));
            defendingUnit = tgs.CellGetIndex(tgs.CellGetAtPosition(targetPosition, true));

            int distanceToTarget = tgs.CellGetHexagonDistance(attackingUnit, defendingUnit);
            if(distanceToTarget <= maxRange)
            {
                RangedAttack();
            }
            else
            {
                Debug.Log("Target too far away");
            }
        }

        void RangedAttack()
        {
            Debug.Log("Initiating ranged attack");
        }
    }
    //if (angleBetweenUnits < unitMaxFovAngle)
    //{
    //    //if (Input.GetMouseButtonDown(1))
    //    //{
    //    Debug.Log(targetRayHit);
    //    if (Physics.SphereCast(unitPosition, 1f, playerToTargetDirection.normalized, out targetRayHit, 6f))
    //    {
    //        if (targetRayHit.collider.gameObject.tag == "Enemy")
    //        {
    //            Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * targetRayHit.distance, Color.yellow);
    //            Debug.Log("Attack now");
    //            return true;

    //        }
    //        else
    //        {
    //            Debug.Log("No attack possible");
    //            return false;
    //        }
    //    }
    //    Debug.Log("Out of sight");
    //    return false;



    //    if (Physics.Raycast(mouseRay.origin, playerToTargetDirection, out hit))
    //    {

    //        Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * 5, Color.white);
    //        Debug.DrawRay(mouseRay.origin, mouseRay.direction * 50, Color.red);
    //        return true;
    //    }
    //    return false;
    //}





    //private void CheckWithinMeleeDistance()
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        Vector3 unitPosition = transform.position;
    //        Cell cell = tgs.CellGetAtPosition(unitPosition, true);
    //        attackingUnit = tgs.CellGetIndex(cell);
    //        unitUnderAttack = tgs.cellLastClickedIndex;
    //        if (tgs.CellGetHexagonDistance(attackingUnit, unitUnderAttack) == 1)
    //            Debug.Log("Melee Combat Enabled");
    //        else
    //            Debug.Log("Too far away for Melee Combat");
    //    }
    //}
}