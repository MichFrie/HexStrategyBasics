using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class Combat : MonoBehaviour
{
    TerrainGridSystem tgs;

    //CheckWithinCombatDistance Variables
    int distanceValue1, distanceValue2;

    void Start()
    {
        tgs = TerrainGridSystem.instance;
    }


    void Update()
    {
        //CheckWithinMeleeDistance();
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
}
