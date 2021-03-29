using System;
using TGS;
using UnityEngine;


public class DebugGui : MonoBehaviour
{
    TerrainGridSystem tgs;
    string textFieldString = "";
    string textFieldNew = "";
    void Start()
    {
        tgs = TerrainGridSystem.instance;
    }

    

    void OnGUI()
    {
        //textFieldString = GUI.TextArea(new Rect(10, 40, 200, 30), "cell Index" + tgs.cellLastClickedIndex.ToString());
        textFieldString = GUI.TextArea(new Rect(10, 90, 200, 30), "cell Row" + tgs.CellGetRow(tgs.cellLastClickedIndex).ToString());
        textFieldString = GUI.TextArea(new Rect(10, 130, 200, 30), "cell Column" + tgs.CellGetColumn(tgs.cellLastClickedIndex).ToString());
        //textFieldNew = GUI.TextArea(new Rect(10, 90, 200, 30), "cell Index" + tgs.CellGetPosition);
    }
}
