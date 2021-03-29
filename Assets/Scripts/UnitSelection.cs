using UnityEngine;


public class UnitSelection : MonoBehaviour
{

    GameObject unitInstance;
    GameObject[] allUnits;

    UnitStatsDisplay unitStatsDisplay;

    void Start()
    {
        unitStatsDisplay = GetComponent<UnitStatsDisplay>();    
    }


    void Update()
    {
        SelectUnit();

        if (Input.GetMouseButtonDown(1))
        {
            allUnits = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject unit in allUnits)
                unit.layer = 11;
        }
          
    }

    void SelectUnit()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                unitInstance = hit.collider.gameObject;
                unitInstance.layer = 12;
            }
            if(unitStatsDisplay != null)
            {
                GameObject go = hit.transform.gameObject;
                go.GetComponent<UnitStatsDisplay>().DisplayUnitStatsOnGui();
            }
        }
    }
}
