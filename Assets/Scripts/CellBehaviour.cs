using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;

public class CellBehaviour : MonoBehaviour
{

    TerrainGridSystem tgs;
    List<Cell> allCells;

    int group1 = 2;
    int group2 = 4;
    int group3 = 8;
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
            //green = wood;
            if (tgs.CellGetTexture(cellIndex) == tgs.textures[1])
            {
                tgs.CellSetCrossCost(cellIndex, 2);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[1]);
                tgs.CellSetGroup(cellIndex, group1);
            }
            //apple = swamp
            else if (tgs.CellGetTexture(cellIndex) == tgs.textures[2])
            {
                tgs.CellSetCrossCost(cellIndex, 4);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[2]);
                tgs.CellSetGroup(cellIndex, group2);
            }
            //map = mountain
            else if (tgs.CellGetTexture(cellIndex) == tgs.textures[3])
            {
                tgs.CellSetCanCross(cellIndex, false);
                tgs.CellToggleRegionSurface(cellIndex, false, tgs.textures[3]);
            }

        } 
    }
}