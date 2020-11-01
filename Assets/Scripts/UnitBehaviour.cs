using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;
using UnityEditor;

public class UnitBehaviour : MonoBehaviour
{
    TerrainGridSystem tgs;

    //Unit Highlight Materials
    
    public Transform unitPrefab;

    private void Start()
    {
        tgs = TerrainGridSystem.instance;
       
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //ExtendLine();
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

    private void ExtendLine()
    {
        Transform point = Instantiate(unitPrefab);
        point.localPosition = transform.position + (Vector3.forward / 2f) * 1.8f;
    }
}
