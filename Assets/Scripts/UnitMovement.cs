using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TGS;
using System;

public class UnitMovement : MonoBehaviour
{
    enum State
    {
        IDLE,
        MOVING,
        MOVESELECT
    }

    State state;
    
    TerrainGridSystem tgs;
    
    short moveCounter;
    
    int endPositionIndex;
    int startCellIndex;
    int HighlightRange = 2;

    Vector3 startPosition, endPosition;

    bool isSelectingStart;
    bool unitSelected;

    List<int> moveList;
    List<int> cellIndices;
    List<Vector3> worldPositions;
    List<GameObject> LOSMarkers;

    Component[] units;

    private void Awake()
    {
 
    }

    void Start()
    {
        tgs = TerrainGridSystem.instance;
        state = State.MOVESELECT;
        isSelectingStart = true;
        tgs.OnCellClick += (grid, cellIndex, buttonIndex) => BuildPath(cellIndex);
        tgs.OnCellEnter += (grid, cellIndex) => ShowLineOfSight(cellIndex);

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
        transform.localScale = new Vector3(0.2f, 0.2f, 0.7f);
    }

    private void FormColumn()
    {
        transform.localScale = new Vector3(0.7f, 0.2f, 0.2f);
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
                }
                break;

            case State.MOVESELECT:
                if (Input.GetMouseButtonUp(0))
                {   //gets path when left mouse is released
                    int t_cell = tgs.cellHighlightedIndex;
                    //tgs.CellFadeOut(t_cell, Color.red, 50);
                    if (t_cell != -1)
                    {//checks if cell is selected
                        int startCell = tgs.CellGetIndex(tgs.CellGetAtPosition(transform.position, true));
                        float totalCost;
                        moveList = tgs.FindPath(startCell, t_cell, out totalCost);
                        if (moveList == null)
                            return;
                        Debug.Log("Cell Clicked: " + t_cell + ", Total move cost: " + totalCost);
                        //tgs.CellFadeOut(moveList, Color.green, 5f);
                        moveCounter = 0;
                        state = State.MOVING;
                    }
                    else
                    {
                        Debug.Log("No Cell");
                    }
                }
                break;
        }
    }

    void Move(Vector3 in_vec)
    {
        float speed = moveList.Count * 5f;
        float step = speed * Time.deltaTime;

        // target position must account for cube height since the cellGetPosition will return the center of the cell which is at floor.
        in_vec.y += transform.localScale.y * 0.5f;
        transform.position = Vector3.MoveTowards(transform.position, in_vec, step);

        // Check if character has reached next cell (we use a small threshold to avoid floating point comparison; also we check only xz plane since the character y position could be adjusted or limited
        // by the slope of the terrain).
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(in_vec.x, in_vec.z));
        if (dist <= 0.1f)
        {
            moveCounter++;
        }
    }

    private void BuildPath(int clickedCellIndex)
    {
        Debug.Log("Clicked on cell " + clickedCellIndex);

        DestroyLOSMarkers();

        if (isSelectingStart)
        {
            //Select start cell
            startCellIndex = clickedCellIndex;
            //tgs.CellToggleRegionSurface(startCellIndex, true, Color.yellow);
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

    private void InstantMovement()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
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
        if (neighbours != null)
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
        if (Input.GetKeyDown(KeyCode.LeftControl))
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

    }

    private void DestroyLOSMarkers()
    {
        if (LOSMarkers == null)
            LOSMarkers = new List<GameObject>();
        else
            LOSMarkers.ForEach((GameObject obj) => DestroyImmediate(obj));
    }
}
