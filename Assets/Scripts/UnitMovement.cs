﻿using System.Collections.Generic;
using TGS;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    enum State
    {
        IDLE,
        MOVING,
        MOVESELECT
    }

    enum Formation
    {
        IDLE,
        LINE,
        COLUMN
    }

    State state;
    Formation formation;

    TerrainGridSystem tgs;

    short moveCounter;

    int startCellIndex;

    float unitMovementPoints = 10;

    bool isSelectingStart;
 
    

    List<int> moveList;
    List<int> cellIndices;
    List<Vector3> worldPositions;
    List<GameObject> LOSMarkers;



    void Start()
    {
        tgs = TerrainGridSystem.instance;
        state = State.MOVESELECT;
        formation = Formation.IDLE;
        isSelectingStart = true;
        tgs.OnCellClick += (grid, cellIndex, buttonIndex) => BuildPath(cellIndex);
        //tgs.OnCellEnter += (grid, cellIndex) => ShowLineOfSight(cellIndex);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.M))
            ShowRange(true);
        if (Input.GetKeyDown(KeyCode.R))
            ShowRange(false);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateLeft();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateRight();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            FormLine();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            FormColumn();
        }
        MoveSelectedUnit();
    }

  

    private void FormLine()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        formation = Formation.LINE;
        unitMovementPoints -= 2f;
    }

    private void FormColumn()
    {
        transform.localScale = new Vector3(0.7f, 0.2f, 0.7f);
        formation = Formation.COLUMN;
        unitMovementPoints -= 2f;
    }

    private void RotateRight()
    {
        transform.rotation *= Quaternion.Euler(0, 60, 0);
    }

    private void RotateLeft()
    {
        transform.rotation *= Quaternion.Euler(0, -60, 0);
    }

    public void MoveSelectedUnit()
    {
        // Check Unit state
        switch (state)
        {
            case State.IDLE:
                break;

            case State.MOVING:
                if (moveCounter < moveList.Count)
                {
                    Move(tgs.CellGetPosition(moveList[moveCounter]));
                }
                else
                {
                    moveCounter = 0;
                    state = State.MOVESELECT;
                    PositionUnitInCenterOfCell();
                }
                break;

            case State.MOVESELECT:
                if (Input.GetMouseButtonUp(0) && this.gameObject.layer == 12)
                {   //definition of target cell
                    int targetCell = tgs.cellHighlightedIndex;
                    if (targetCell != -1)
                    {//checks if cell is selected
                        int startCell = tgs.CellGetIndex(tgs.CellGetAtPosition(transform.position, true));
                        float totalCost;
                        //builds a path from startCell to targetCell
                        moveList = tgs.FindPath(startCell, targetCell, out totalCost);
                        if (moveList == null)
                            return;

                        //Debug.Log("Cell Clicked: " + targetCell + ", Total move cost: " + totalCost);

                        //check if path exceeds unitRange
                        if (moveList.Count < CalculateUnitMovementPoints())
                        {
                            moveCounter = 0;
                            state = State.MOVING;
                            unitMovementPoints -= totalCost;
                            Debug.Log("UnitMovementPoints: " + unitMovementPoints);
                        }
                        else
                        {
                            Debug.Log("Movement Range exceeded");
                        }

                    }
                    else
                    {
                        Debug.Log("No Cell");
                    }
                }
                break;
        }
    }

    float CalculateUnitMovementPoints()
    {
        if (formation == Formation.COLUMN)
        {
            unitMovementPoints *= 2f;
            formation = Formation.IDLE;
            return unitMovementPoints;
        }

        else
        {
            formation = Formation.IDLE;
            return unitMovementPoints;
        }
    }

    void Move(Vector3 in_vec)
    {
        float speed = moveList.Count * 5f;
        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, in_vec, step);

        // Check if unit has reached next cell
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(in_vec.x, in_vec.z));
        if (dist <= 0.1f)
        {
            moveCounter++;
        }
    }

    private void BuildPath(int clickedCellIndex)
    {
        //Debug.Log("Clicked on cell " + clickedCellIndex);

        DestroyLOSMarkers();

        if (isSelectingStart)
        {
            //Select start cell
            startCellIndex = clickedCellIndex;
        }
        else
        {
            //End cell clicked, will show path
            //First color of start cell will be cleared

            //tgs.CellToggleRegionSurface(startCellIndex, true, Color.yellow);

            //Get Path
            List<int> path = tgs.FindPath(startCellIndex, clickedCellIndex, 0, 0, 1);

            //Color Path
            if (path != null)
            {
                for (int k = 0; k < path.Count; k++)
                {
                    tgs.CellFadeOut(path[k], Color.green, 1f);
                }
            }
        }
        isSelectingStart = !isSelectingStart;
    }

    private void ShowRange(bool useLineOfSight = false)
    {
        List<int> neighbours = tgs.CellGetNeighbours(tgs.cellLastClickedIndex, (int)unitMovementPoints - 1);
        if (neighbours != null)
        {
            if (useLineOfSight)
            {
                tgs.CellTestLineOfSight(tgs.cellHighlightedIndex, neighbours);
            }
            tgs.CellFlash(neighbours, Color.yellow, 5f);
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

    public void PositionUnitInCenterOfCell()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        Bounds bounds = tgs.CellGetRectWorldSpace(cellIndex);
        transform.position = bounds.center;
    }
}
