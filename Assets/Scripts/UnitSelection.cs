using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class UnitSelection : MonoBehaviour
{

    void Start()
    {
        GetComponent<UnitMovement>().enabled = false;
    }

    void Update()
    {
        SelectUnit();
    }

    void SelectUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                hit.transform.gameObject.GetComponent<UnitMovement>().enabled = true;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<UnitMovement>().enabled = false;
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
