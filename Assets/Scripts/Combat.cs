using TGS;
using UnityEngine;

public class Combat : MonoBehaviour
{
    TerrainGridSystem tgs;
    public UnitStats unitStats;

    public int strength;
    private int distanceValue1;
    private int distanceValue2;

    private Ray mouseRay;

    private RaycastHit hit;
    private RaycastHit targetRayHit;

    private Vector3 unitPosition;
    private Vector3 playerToTargetDirection;

    private Quaternion unitRotation;

    //public float radiant1 = 1.7f;
    //float radiant2 = 0.55f;
    //float radiant3 = 90f;

    float[] radiantArray = new float[]{ 0.55f, 1.28f, 1.7f, 2.56f, 90f };
    void Start()
    {
        tgs = TerrainGridSystem.instance;

    }


    void Update()
    {
        //CheckWithinMeleeDistance();
        //RangedAttack();
        //MeasureDistanceFromPlayer();
        //CheckLineOfSight();

        //CheckRaySensorObjects();

    }

    void CheckRaySensorObjects()
    {
  
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

    private void RangedAttack(RaycastHit hit)
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(hit.collider.gameObject.tag == "Enemy")
            {
                Debug.Log("Attacking this object");
            }
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.tag == "Enemy")
        //        {
        //            Debug.Log("initiating Ranged Attack");
        //            hit.transform.GetComponent<Combat>().unitStats.strength -= 100;
        //        }
        //    }
        //}
    }

    private void CheckLineOfSight()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        unitPosition = this.transform.position;
        RaycastHit hit;
        //playerToTargetDirection = hit.point - unitPosition;

        for (int i = 0; i < radiantArray.Length; i++)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(radiantArray[i], 0, 1)), out hit, 5f))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(radiantArray[i], 0, 1)) * hit.distance, Color.yellow);
                RangedAttack(hit);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(radiantArray[i], 0, 1)) * 5f, Color.white);
            }
        }
    }
}


//LEGACYCODE
//float angleBetweenUnits = Vector3.Angle(this.gameObject.transform.position - hit.collider.transform.position, currentViewRadius);
//if (angleBetweenUnits < unitMaxFOVangle)
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



// if (Physics.Raycast(mouseRay.origin, playerToTargetDirection, out hit))
//{

//    Debug.DrawRay(unitPosition, playerToTargetDirection.normalized * 5, Color.white);
//    Debug.DrawRay(mouseRay.origin, mouseRay.direction * 50, Color.red);
//    return true;
//}
//return false;