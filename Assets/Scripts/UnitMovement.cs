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

    enum FACING
    {
        Facing0,
        Facing60,
        Facing120,
        Facing180,
        Facing240,
        Facing300
    }
    
    [SerializeField] FACING facing;
    public enum UNITSELECTION
    {
        SELECTED, 
        NOT_SELECTED
    }

    const int CELLS_ALL = -1;
    const int CELL_DEFAULT = 1;
    const int CELL_PLAYER = 2;
    const int CELL_ENEMY = 4;
    const int CELL_OBSTACLE = 8;
    const int CELLS_ALL_NAVIGATABLE = ~(CELL_OBSTACLE | CELL_PLAYER | CELL_ENEMY);

    public Cell backOfCell;
    public Cell frontOfCell;
    public Cell topLeftOfCell;
    public Cell topRightOfCell;
    public Cell bottomLeftOfCell;
    public Cell bottomRightOfCell;
    
    public Cell targetPoint_Front;
    public Cell targetPoint_FrontRight;
    public Cell targetPoint_FrontLeft;
    public Cell targetPoint_Back;
    public Cell targetPoint_BackLef;
    public Cell targetPoint_BackRight;

    STATE state;
    FORMATION formation;
    UNITSELECTION selection;

    TerrainGridSystem tgs;
   
    public UnitStats stats;

    short moveCounter;

    int startCellIndex;
    
    int cone = 95;

    float unitMovementPoints;

    bool isSelectingStart;

    List<int> moveList;

    public Cell targetPoint;
   
    void Start()
    {
        tgs = TerrainGridSystem.instance;

        //stats = ScriptableObject.CreateInstance<UnitStats>();
        unitMovementPoints = GetUnitMovementPoints();
        //unitMovementPoints = 10;
        
        state = STATE.MOVESELECT;
        formation = FORMATION.IDLE;
        facing = FACING.Facing0;
        isSelectingStart = true;
        tgs.OnCellClick += (grid, cellIndex, buttonIndex) => BuildPath(cellIndex);
        //tgs.OnCellEnter += (grid, cellIndex) => ShowLineOfSight(cellIndex);
    }

    float GetUnitMovementPoints()
    {
        print(stats.movementPoints);
        return stats.movementPoints;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            GetUnitMovementPoints();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowLineOfSight();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowMovementRange();
        }
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
        if ((Input.GetKeyDown(KeyCode.I)))
        { 
            ShowCellSide();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //GetConeViaTargetPoint();
            GetConeViaVectorAngle();
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
    }

    void FormColumn()
    {
        transform.localScale = new Vector3(0.7f, 0.2f, 0.7f);
        formation = FORMATION.COLUMN;
    }

    void RotateRight()
    {
        transform.rotation *= Quaternion.Euler(0, 60, 0);
        CalculateFacing();
    }



    void RotateLeft()
    {
        transform.rotation *= Quaternion.Euler(0, -60, 0);
        CalculateFacing();
    }    
    
    public void CalculateFacing()
    {
        int angle = Mathf.Abs((int)transform.eulerAngles.y);
        
        switch (angle)
        {
            case 0: CheckAnglesFor0();
                facing = FACING.Facing0;
                break;
            case 60: CheckAnglesFor60();
                facing = FACING.Facing60;
                break;
            case 120: CheckAnglesFor120();
                facing = FACING.Facing120;
                break;
            case 180: CheckAnglesFor180();
                facing = FACING.Facing180;
                break;
            case 240: CheckAnglesFor240();
                facing = FACING.Facing240;
                break;
            case 300: CheckAnglesFor300();
                facing = FACING.Facing300;
                break;
            default: break;
        }
    }

    void ShowCellSide()
    {
        CalculateFacing();

        tgs.CellColorTemp(frontOfCell, Color.green, 3f);
        tgs.CellColorTemp(topLeftOfCell, Color.green, 3f);
        tgs.CellColorTemp(topRightOfCell, Color.green, 3f);
        tgs.CellColorTemp(backOfCell, Color.red, 3f);
        tgs.CellColorTemp(bottomLeftOfCell, Color.red, 3f);
        tgs.CellColorTemp(bottomRightOfCell, Color.red, 3f);
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
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        List<int> neighbours = tgs.CellGetNeighbours(cellIndex, (int)unitMovementPoints);
       
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

        backOfCell = tgs.CellGetAtPosition(column, row - 1);
        frontOfCell = tgs.CellGetAtPosition(column, row + 1);
        topLeftOfCell = tgs.CellGetAtPosition(column -1, row + 1);
        topRightOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column - 1, row);
        bottomRightOfCell = tgs.CellGetAtPosition(column + 1, row);
    }


    void CheckAnglesFor60()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        
        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column - 1, row);
        frontOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        topLeftOfCell = tgs.CellGetAtPosition(column, row + 1);
        topRightOfCell = tgs.CellGetAtPosition(column + 1, row);
        bottomLeftOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column, row -1);
    }

    void CheckAnglesFor120()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        frontOfCell = tgs.CellGetAtPosition(column + 1, row);
        topLeftOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        topRightOfCell = tgs.CellGetAtPosition(column, row - 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column, row + 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column - 1, row);
    }  
    void CheckAnglesFor180()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column, row + 1);
        frontOfCell = tgs.CellGetAtPosition(column, row - 1);
        topLeftOfCell = tgs.CellGetAtPosition(column + 1, row);
        topRightOfCell = tgs.CellGetAtPosition(column - 1, row);
        bottomLeftOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
    }

    void CheckAnglesFor240()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
        frontOfCell = tgs.CellGetAtPosition(column - 1, row);
        topLeftOfCell = tgs.CellGetAtPosition(column, row - 1);
        topRightOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column + 1, row);
        bottomRightOfCell = tgs.CellGetAtPosition(column, row + 1);
    }
    void CheckAnglesFor300()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        backOfCell = tgs.CellGetAtPosition(column + 1, row);
        frontOfCell = tgs.CellGetAtPosition(column - 1, row + 1);
        topLeftOfCell = tgs.CellGetAtPosition(column - 1, row);
        topRightOfCell = tgs.CellGetAtPosition(column, row + 1);
        bottomLeftOfCell = tgs.CellGetAtPosition(column, row - 1);
        bottomRightOfCell = tgs.CellGetAtPosition(column + 1, row + 1);
    }

    void GetConeViaTargetPoint()
    {
        List<int> coneIndices = new List<int>();
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);
        CalculateConesViaTargetPoints();
        int targetCellIndex = tgs.CellGetIndex(targetPoint);
        tgs.GetCellsWithinCone(cellIndex, targetCellIndex,  cone, coneIndices);

        foreach (var c in coneIndices)
        {
            tgs.CellFlash(c, Color.cyan, 2f);
            
        }
    }

    void GetConeViaVectorAngle()
    {
        List<int> coneIndices = new List<int>();
        Cell cell = tgs.CellGetAtPosition(transform.position, true);
        int cellIndex = tgs.CellGetIndex(cell);

        Vector2 startPos = tgs.cells[cellIndex].center;
        Vector2 endPos = tgs.cells[tgs.cellLastClickedIndex].center;
        Vector2 direction = endPos - startPos;
        float maxDistance = Vector2.Distance(startPos, endPos);
        direction /= maxDistance;
        tgs.CellGetWithinCone(cellIndex, direction, maxDistance, 60.0f, coneIndices);

       
        
        foreach (var c in coneIndices)
        {
            tgs.CellFlash(c, Color.cyan, 2f);
        }
    }   
    
    void CalculateConesViaTargetPoints()
    {
        Cell cell = tgs.CellGetAtPosition(transform.position, true);

        int row = cell.row;
        int column = cell.column;

        switch (facing)
        {
            case FACING.Facing0: targetPoint = tgs.CellGetAtPosition(column, row +3);
                break;
            case FACING.Facing60: targetPoint = tgs.CellGetAtPosition(column + 3, row + 2);
                break;
            case FACING.Facing120: targetPoint = tgs.CellGetAtPosition(column + 3, row - 1);
                break;
            case FACING.Facing180: targetPoint = tgs.CellGetAtPosition(column, row - 3);
                break;
            case FACING.Facing240: targetPoint = tgs.CellGetAtPosition(column - 3, row - 1);
                break;
            case FACING.Facing300: targetPoint = tgs.CellGetAtPosition(column - 3, row + 2);
                break;
                
            default: break;
        }
    }
}
