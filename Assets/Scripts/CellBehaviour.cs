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
    }

    void Update()
    {

    //old code, just use as reference
    //if(tgs.CellGetTexture(cellIndex) == tgs.textures[1]){

    //       tgs.CellSetCrossCost(cellIndex, 2);
    //    }
    }
}
