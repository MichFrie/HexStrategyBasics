using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class UnitSelection : MonoBehaviour
{

    void Start()
    {
    }

    // Update is called once per frame
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
                Debug.Log(hit.transform.gameObject.name);
                hit.transform.gameObject.GetComponent<UnitMovement>().enabled = true;
                
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                hit.transform.gameObject.GetComponent<UnitMovement>().enabled = false;
                
            }
        }
    }
}
