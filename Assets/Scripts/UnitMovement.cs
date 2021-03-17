using System;
using System.Collections.Generic;
using TGS;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    enum STATE
    {
        IDLE,
        MOVING,
        MOVESELECT
    }

    enum FORMATION
    {
        IDLE,
        LINE,
        COLUMN
    }

    enum FRONTS
    {
        FrontOFCell, 
        BackOfCell, 
        TopLeftOfCell, 
        TopRightOfCell, 
        BottomLeftOfCell, 
        BottomRightOfCell
    }

    const int CELLS_ALL = -1;
    const int CELL_DEFAULT = 1;
    const int CELL_PLAYER = 2;
    const int CELL_ENEMY = 4;
    const int CELL_OBSTACLE = 8;
    const int CELLS_ALL_NAVIGATABLE = ~(CELL_OBSTACLE | CELL_PLAYER | CELL_ENEMY);

    STATE state;
    FORMATION formation;

    TerrainGridSystem tgs;

    short moveCounter;

    int startCellIndex;

    public float unitMovementPoints = 10;

    bool isSelectingStart;

    List<int> moveList;  
   
    void Start()
    {
        tgs = TerrainGridSystem.instance;
        state = STATE.MOVESELECT;
        formation = FORMATION.IDLE;
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
            ShowLineOfSight();
        if (Input.GetKeyDown(KeyCode.T))
            ShowMovementRange();
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            DefineFrontFacing();
        }
        MoveSelectedUnit();
    }

    void DefineFrontFacing()
    {
        int cellIndex = tgs.CellGetNeighbour(tgs.cellLastClickedIndex, CELL_SIDE.Bottom);
        tgs.CellFlash(cellIndex, Color.cyan, 1f);
    }

    void FormLine()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        formation = FORMATION.LINE;
        unitMovementPoints -= 2f;
    }

    void FormColumn()
    {
        transform.localScale = new Vector3(0.7f, 0.2f, 0.7f);
        formation = FORMATION.COLUMN;
        unitMovementPoints -= 2f;
        unitMovementPoints += 10f;
    }

    void RotateRight()
    {
        transform.rotation *= Quaternion.Euler(0, 60, 0);
        DisplayFronts();
    }



    void RotateLeft()
    {
        transform.rotation *= Quaternion.Euler(0, -60, 0);
    }    
    
    void DisplayFronts()
    {
        int angle = Mathf.Abs((int)transform.eulerAngles.y);

        switch (angle)
        {
            case 0: CheckAnglesFor0(); break;
            case 60: CheckAnglesFor60(); break;
            case 120: CheckAnglesFor120();  break;
            case 180: CheckAnglesFor180(); break;
            case 240: CheckAnglesFor240(); break;
            case 300: CheckAnglesFor300(); break;
            default: break;
        }
    }


    public void MoveSelectedUnit()
    {
        switch (state)
        {
            case STATE.IDLE:
                break;

            case STATE.MOVING:
                if (moveCounter < moveList.Count)
                {
                    Move(tgs.CellGetPosition(moveList[moveCounter]));
                }
                else
                {
                    moveCounter = 0;
                    state = STATE.MOVESELECT;
                    PositionUnitInCenterOfCell();
                }
                break;

            case STATE.MOVESELECT:
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
                            state = STATE.MOVING;
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

    void BuildPath(int clickedCellIndex)
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

    void ShowMovementRange()
    {
        List<int> neighbours = tgs.CellGetNeighbours(tgs.cellLastClickedIndex, (int)unitMovementPoints);
       
        if (neighbours != null)
        {
            tgs.CellFlash(neighbours, Color.yellow, 1f);
        }
    }

    void ShowLineOfSight()
    {
        List<int> neighbours = tgs.CellGetNeighbours(tgs.cellLastClickedIndex, 10);
        if(neighbours != null)
        {
            int group1 = tgs.CellGetGroup(1);
            int group2 = tgs.CellGetGroup(2);
            tgs.CellTestLineOfSight(tgs.cellHighlightedIndex, neighbours, group2);
            tgs.CellFlash(neighbours, Color.red, 1f);
        }       
    }

    /// <summary>
    /// IDEE Fire Cone
    /// 
    /// wenn Distanz zum Ziel kleiner max Range und FrontOfCell/TopLeftOfCell/TopRightOfCell Distanz zum Ziel kleiner Unit - Ziel Distanz, dann Feuer möglich 
    /// </summary>

    public void PositionUnitInCenterOfCell()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        Bounds bounds = tgs.CellGetRectWorldSpace(cellIndex);
        transform.position = bounds.center;
    }

    void MarkAllGameObjects()
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

    void CheckZoneOfControl()
    {
        List<int> cellIndexNew = tgs.CellGetNeighboursWithinRange(tgs.cellLastClickedIndex, 0,1);
        foreach(var item in cellIndexNew)
        {
            tgs.CellFlash(item, Color.cyan, 1f);
        }
    }

    void CheckAnglesFor0()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        Cell backOfCell = tgs.CellGetAtPosition(column, row - 1);
        Cell frontOfCell = tgs.CellGetAtPosition(column, row + 1);
        Cell topLeftOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        Cell topRightOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        Cell bottomLeftOfCell = tgs.CellGetAtPosition(column - 1, row);
        Cell bottomRightOfCell = tgs.CellGetAtPosition(column + 1, row);

        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(frontOfCell, Color.blue, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.yellow, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.black, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.cyan, 3f);
    }


    void CheckAnglesFor60()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        
        int row = cell.row;
        int column = cell.column;

        Cell backOfCell = tgs.CellGetAtPosition(column - 1, row);
        Cell frontOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        Cell topLeftOfCell = tgs.CellGetAtPosition(column, row + 1);
        Cell topRightOfCell = tgs.CellGetAtPosition(column + 1, row);
        Cell bottomLeftOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        Cell bottomRightOfCell = tgs.CellGetAtPosition(column, row -1);
        
        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(frontOfCell, Color.blue, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.yellow, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.black, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.cyan, 3f);
    }

    void CheckAnglesFor120()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        Cell backOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        Cell frontOfCell = tgs.CellGetAtPosition(column + 1, row);
        Cell topLeftOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        Cell topRightOfCell = tgs.CellGetAtPosition(column, row - 1);
        Cell bottomLeftOfCell = tgs.CellGetAtPosition(column, row + 1);
        Cell bottomRightOfCell = tgs.CellGetAtPosition(column - 1, row);

        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(frontOfCell, Color.blue, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.yellow, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.black, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.cyan, 3f);
    }  
    void CheckAnglesFor180()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        Cell backOfCell = tgs.CellGetAtPosition(column, row + 1);
        Cell frontOfCell = tgs.CellGetAtPosition(column, row - 1);
        Cell topLeftOfCell = tgs.CellGetAtPosition(column + 1, row);
        Cell topRightOfCell = tgs.CellGetAtPosition(column - 1, row);
        Cell bottomLeftOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        Cell bottomRightOfCell = tgs.CellGetAtPosition(column - 1, row + 1);

        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(frontOfCell, Color.blue, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.yellow, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.black, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.cyan, 3f);
    }

    void CheckAnglesFor240()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        Cell backOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        Cell frontOfCell = tgs.CellGetAtPosition(column - 1, row);
        Cell topLeftOfCell = tgs.CellGetAtPosition(column, row - 1);
        Cell topRightOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        Cell bottomLeftOfCell = tgs.CellGetAtPosition(column + 1, row);
        Cell bottomRightOfCell = tgs.CellGetAtPosition(column, row + 1);

        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(frontOfCell, Color.blue, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.yellow, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.black, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.cyan, 3f);
    }
    void CheckAnglesFor300()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        Cell backOfCell = tgs.CellGetAtPosition(column + 1, row);
        Cell frontOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        Cell topLeftOfCell = tgs.CellGetAtPosition(column - 1, row);
        Cell topRightOfCell = tgs.CellGetAtPosition(column, row + 1);
        Cell bottomLeftOfCell = tgs.CellGetAtPosition(column, row - 1);
        Cell bottomRightOfCell = tgs.CellGetAtPosition(column + 1, row + 1);

        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(frontOfCell, Color.blue, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.yellow, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.black, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.cyan, 3f);
    }



  
}
