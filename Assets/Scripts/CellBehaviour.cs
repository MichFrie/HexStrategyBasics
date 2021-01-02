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

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            int cellIndex = tgs.cellLastClickedIndex;
            tgs.CellGetSettings();
        }
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
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[1]);
            }
            else if (tgs.CellGetTexture(cellIndex) == tgs.textures[2])
            {
                tgs.CellSetCrossCost(cellIndex, 4);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[2]);
            }
            else if (tgs.CellGetTexture(cellIndex) == tgs.textures[3])
            {
                tgs.CellSetCanCross(cellIndex, false);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[3]);
            }

        }
    }
}