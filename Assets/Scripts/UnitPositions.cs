using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TGS;

public class UnitPositions : MonoBehaviour
{
    TerrainGridSystem tgs;

    void Start()
    {
        tgs = TerrainGridSystem.instance;
        PositionUnitInCenterOfCell();
           }

    public void PositionUnitInCenterOfCell()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position,true);
        int cellIndex = tgs.CellGetIndex(cell);
        Bounds bounds = tgs.CellGetRectWorldSpace(cellIndex);
        transform.position = bounds.center;
        
    }
}
