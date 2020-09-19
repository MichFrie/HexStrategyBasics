using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class Combat : MonoBehaviour
{
    TerrainGridSystem tgs;
    EnemyBehaviour enemy;
    public UnitStats unitStats;
    public int strength;

    //CheckWithinCombatDistance Variables
    int distanceValue1, distanceValue2;

    void Start()
    {
        tgs = TerrainGridSystem.instance;
        enemy = GetComponent<EnemyBehaviour>();
    }


    void Update()
    {
        //CheckWithinMeleeDistance();
        RangedAttack();
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
                Debug.Log("Error");
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
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.tag == "Confederate")
                {
                    Debug.Log("Attack successful");
                    hit.transform.GetComponent<Combat>().unitStats.strength -= 100;
                }
            }
        }
    }
}
