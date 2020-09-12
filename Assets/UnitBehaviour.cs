using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;
using UnityEditor;

public class UnitBehaviour : MonoBehaviour
{
    TerrainGridSystem tgs;

    //Unit Highlight Materials
    public Material[] materials;

    private void Start()
    {
        tgs = TerrainGridSystem.instance;
        materials = new Material[2];
    }

    private void Update()
    {
        //SelectedUnitHighlight();
    }

    private void SelectedUnitHighlight()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray))
            {
                GetComponent<Renderer>().material = materials[1];
            }
            else
            {
                GetComponent<Renderer>().material = materials[0];
            }
        }
    }
}
