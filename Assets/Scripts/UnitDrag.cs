using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class UnitDrag : MonoBehaviour
{
  
	TerrainGridSystem tgs;
	
	private Vector3 screenPoint;
	private Vector3 offset;
    
	void Start()
    {
		tgs = TerrainGridSystem.instance;
	}

	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}

	void OnMouseDrag()
	{
		Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
		transform.position = cursorPosition;
	}

    void OnMouseUp()
    {
		Vector3 unitPosition = transform.position;
		Cell cell = tgs.CellGetAtPosition(unitPosition, true);
		int row = cell.row;
		int column = cell.column;
    }

}
