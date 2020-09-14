using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;
using System;

public class Movement : MonoBehaviour
{
    TerrainGridSystem tgs;

    Vector3 startPosition, endPosition;
    
    private int endPositionIndex;
    private int startCellIndex;
    private int HighlightRange = 2;
    
    private bool isSelectingStart;

    private List<int> cellIndices;
    private List<Vector3> worldPositions;
    private List<GameObject> LOSMarkers;


    void Start()
    {
        tgs = TerrainGridSystem.instance;
        isSelectingStart = true;
        tgs.OnCellClick += (grid, cellIndex, buttonIndex) => BuildPath(cellIndex);
        tgs.OnCellEnter += (grid, cellIndex) => ShowLineOfSight(cellIndex);
    }


    void Update()
    {
        //InstantMovement();
        if (Input.GetKeyDown(KeyCode.L))
            ShowRange(true);
        if (Input.GetKeyDown(KeyCode.R))
            ShowRange(false);
    }

    private void BuildPath(int clickedCellIndex)
    {
        Debug.Log("Clicked on cell " + clickedCellIndex);

        DestroyLOSMarkers();

        if (isSelectingStart)
        {
            //Select start cell
            startCellIndex = clickedCellIndex;
            tgs.CellToggleRegionSurface(startCellIndex, true, Color.yellow);
        }
        else
        {
            //End cell clicked, will show path
            //First color of start cell will be cleared

            tgs.CellToggleRegionSurface(startCellIndex, true, Color.white);

            //Get Path
            List<int> path = tgs.FindPath(startCellIndex, clickedCellIndex, 0, 0, 1);

            //Color Path
            if(path != null)
            {
                for (int k = 0; k < path.Count; k++)
                {
                    tgs.CellFadeOut(path[k], Color.green, 1f);
                }
            }
        }
        isSelectingStart = !isSelectingStart;
    }

    private void InstantMovement()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray))
            {
                startPosition = transform.position;
                endPositionIndex = tgs.cellLastClickedIndex;
                endPosition = tgs.CellGetPosition(endPositionIndex);
                transform.position = endPosition;
            }
        }
    }

    private void ShowRange(bool useLineOfSight = false)
    {
        List<int> neighbours = tgs.CellGetNeighbours(tgs.cellHighlightedIndex, HighlightRange);
        if(neighbours != null)
        {
            if (useLineOfSight)
            {
                tgs.CellTestLineOfSight(tgs.cellHighlightedIndex, neighbours); 
            }
            tgs.CellFlash(neighbours, Color.yellow, 1f);
        }
    }


    private void ShowLineOfSight(int targetCellIndex)
    {
        if (isSelectingStart)
            return;

        DestroyLOSMarkers();

        //Compute LOS and get list of cell indices and world positions
        bool isLOS = tgs.CellGetLineOfSight(startCellIndex, targetCellIndex, ref cellIndices, ref worldPositions);

        //Add small dots along the LOS
        worldPositions.ForEach((Vector3 obj) => {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            LOSMarkers.Add(sphere);
            sphere.transform.position = obj;
            sphere.transform.localScale = Vector3.one * 0.2f;
            sphere.GetComponent<Renderer>().material.color = isLOS ? Color.green : Color.red;
        });
    }

    private void DestroyLOSMarkers()
    {
        if (LOSMarkers == null)
            LOSMarkers = new List<GameObject>();
        else
            LOSMarkers.ForEach((GameObject obj) => DestroyImmediate(obj));
    }
}
