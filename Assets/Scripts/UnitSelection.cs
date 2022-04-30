using UnityEngine;


public class UnitSelection : MonoBehaviour
{
    GameObject unitInstance;
    GameObject officerInstance;
    GameObject[] allUnits;

    UnitStatsDisplay unitStatsDisplay;

    void Awake()
    {
        allUnits = GameObject.FindGameObjectsWithTag("Union");
        foreach (GameObject unit in allUnits)
            unit.layer = 11;
    }

    void Start()
    {
        unitStatsDisplay = GetComponent<UnitStatsDisplay>();
    }


    void Update()
    {
        SelectUnit();
        DeselectUnit();

    }


    void SelectUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 8)
                    return;
                if (hit.transform.gameObject.layer == 11)
                {
                    unitInstance = hit.collider.gameObject;
                    unitInstance.layer = 12;

                    GameObject go = hit.transform.gameObject;
                    go.GetComponent<UnitStatsDisplay>().DisplayUnitStatsOnGui();

                }
                if (hit.transform.gameObject.layer == 13)
                {
                    officerInstance = hit.collider.gameObject;
                    officerInstance.layer = 14;

                    GameObject go = hit.transform.gameObject;
                    go.GetComponent<OfficerStatsDisplay>().DisplayOfficerStatsOnGui();
                }

            }

        }
    }

    //hier nur die eine Unit, die ausgewählt wurde
    void DeselectUnit()
    {
        if (Input.GetMouseButtonDown(1))
        {
            allUnits = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject unit in allUnits)
                unit.layer = 11;
        }
    }
}
