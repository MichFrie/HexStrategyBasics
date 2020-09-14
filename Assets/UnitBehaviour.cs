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
    public Transform unitPrefab;

    private void Start()
    {
        tgs = TerrainGridSystem.instance;
        materials = new Material[2];
    }

    private void Update()
    {
        //SelectedUnitHighlight();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExtendLine();
        }
    }

    private void OnMouseOver()
    {
        //StartCoroutine("ShowToolTip");
    }

    private void OnMouseExit()
    {
        //StopCoroutine("ShowToolTip");
    }

    private IEnumerator ShowToolTip()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Tooltip");
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

    private void ExtendLine()
    {
        Transform point = Instantiate(unitPrefab);
        point.localPosition = transform.position + (Vector3.forward / 2f) * 1.8f;
    }
}
