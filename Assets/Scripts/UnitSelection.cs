using UnityEngine;


public class UnitSelection : MonoBehaviour
{

    GameObject unitInstance;
    GameObject[] allUnits;
    void Start()
    {
        
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
        }
    }
}






//Legacy Code

//OLD METHOD FOR UNIT SELECTION
//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//RaycastHit hit;
//if (Physics.Raycast(ray, out hit))
//{
//    if ((hit.collider.gameObject.tag == "Enemy") || hit.collider.gameObject.tag == "Unit")
//        return;
//}
