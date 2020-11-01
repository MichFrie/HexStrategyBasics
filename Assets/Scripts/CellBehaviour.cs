using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class CellBehaviour : MonoBehaviour
{

    TerrainGridSystem tgs;
    List<Cell> allCells;


    void Start()
    {
        tgs = TerrainGridSystem.instance;
        InitialCellBehaviour();
    }

    private void InitialCellBehaviour()
    {
        allCells = tgs.cells;
        foreach (Cell cell in allCells)
        {
            int cellIndex = tgs.CellGetIndex(cell);
            if (tgs.CellGetTexture(cellIndex) == tgs.textures[1])
            {
                tgs.CellSetCrossCost(cellIndex, 2);
            }
            tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[1]);
        }
    }
}