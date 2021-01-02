using System.Collections.Generic;
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

    const int CELLS_ALL = -1;
    const int CELL_DEFAULT = 1;
    const int CELL_PLAYER = 2;
    const int CELL_ENEMY = 4;
    const int CELL_OBSTACLE = 8;
    const int CELLS_ALL_NAVIGATABLE = ~(CELL_OBSTACLE | CELL_PLAYER | CELL_ENEMY);

    State state;
    Formation formation;

    TerrainGridSystem tgs;

    short moveCounter;

    int startCellIndex;

    public float unitMovementPoints = 10;

    bool isSelectingStart;

    List<int> moveList;  
   
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
        if (Input.GetKeyDown(KeyCode.U))
        {
            MarkAllGameObjects();
        }
        if (Input.GetKeyDown(KeyCode.M))
            ShowRange(true);
        if (Input.GetKeyDown(KeyCode.T))
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
        unitMovementPoints += 10f;
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
                    {//check if cell is selected
                        int startCell = tgs.CellGetIndex(tgs.CellGetAtPosition(transform.position, true));
                        float totalCost;
                        //builds a path from startCell to targetCell
                        moveList = tgs.FindPath(startCell, targetCell, out totalCost);
                        if (moveList == null)
                            return;

                        //check if path exceeds unitRange
                        if (unitMovementPoints >= totalCost)
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
    void Move(Vector3 targetPos)
    {
        float speed = moveList.Count * 5f;
        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // Check if unit has reached next cell
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPos.x, targetPos.z));
        if (dist <= 0.1f)
        {
            moveCounter++;
        }
    }

    private void BuildPath(int clickedCellIndex)
    {
        //Debug.Log("Clicked on cell " + clickedCellIndex);

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
        List<int> neighbours = tgs.CellGetNeighbours(tgs.cellLastClickedIndex, (int)unitMovementPoints);
        if (neighbours != null)
        {
            if (useLineOfSight)
            {
                tgs.CellTestLineOfSight(tgs.cellHighlightedIndex, neighbours, CELLS_ALL_NAVIGATABLE);
            }
            tgs.CellFlash(neighbours, Color.yellow, 1f);
        }
    }

    public void PositionUnitInCenterOfCell()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        Bounds bounds = tgs.CellGetRectWorldSpace(cellIndex);
        transform.position = bounds.center;
    }

    private void MarkAllGameObjects()
    {
        GameObject[] gameobjects;
        gameobjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in gameobjects)
        {
            Cell cell = tgs.CellGetAtPosition(g.transform.position, true);
            int cellIndex = tgs.CellGetIndex(cell);
            tgs.CellSetCrossCost(cellIndex, 12000);
            tgs.CellSetGroup(cellIndex, CELL_ENEMY);
        }
    }
}
