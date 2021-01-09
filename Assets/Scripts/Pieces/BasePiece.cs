using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public abstract class BasePiece : EventTrigger
{

    public bool extraMovement;
    public static bool turnExtra = false;
    

    public enum pieceType { KING, QUEEN, BISHOP, ROOK, KNIGHT, PAWN, UNKNOWN = -1 };
    

    [SerializeField] public pieceType typePiece = pieceType.UNKNOWN;



    public Color mColor = Color.clear;
    public bool mIsFirstMove = true;



    protected Cell mOriginalCell = null;
    public Cell mCurrentCell = null;

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;

    protected Cell mTargetCell = null;

    protected Vector3Int mMovement = Vector3Int.one;
    public List<Cell> mHighlightedCells = new List<Cell>();

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        mPieceManager = newPieceManager;
        mColor = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        mRectTransform = GetComponent<RectTransform>();
    }

    public void setExtraMovement(bool value)
    {
        extraMovement = value;
    }

    public virtual void Place(Cell newCell)
    {
        // Cell stuff
        mCurrentCell = newCell;
        mOriginalCell = newCell;
        mCurrentCell.mCurrentPiece = this;
        // Object stuff
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);

        this.gameObject.name = mCurrentCell.mBoardPosition.x + "+" + mCurrentCell.mBoardPosition.y;
    }

    public void Reset()
    {
        Kill();

        mIsFirstMove = true;

        Place(mOriginalCell);
    }

    public virtual void Kill()
    {
        // Clear current cell
        mCurrentCell.mCurrentPiece = null;

        // Remove piece
        gameObject.SetActive(false);
    }

    public bool HasMove()
    {
        CheckPathing();

        // If no moves
        if (mHighlightedCells.Count == 0)
            return false;
        // If moves available
        return true;
    }

    #region Movement
    private void CreateCellPath(int xDirection, int yDirection, int movement)
    {
        // Target position
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        // Check each cell
        for (int i = 1; i <= movement; i++)
        {

            currentX += xDirection;
            currentY += yDirection;

            // Get the state of the target cell
            CellState cellState = CellState.None;
            try
            {
                cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, this);
                // If enemy, add to list, break
                if (cellState == CellState.Enemy)
                {
                    mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
                    break;
                }

                // If the cell is not free, break
                if (cellState != CellState.Free)
                    break;
                // Add to list
            }
            catch (Exception)
            {

            }
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
       

          
            



          
        }
    }

    public virtual void CheckPathing()
    {
        // Horizontal
        CreateCellPath(1, 0, mMovement.x);
        CreateCellPath(-1, 0, mMovement.x);

        // Vertical 
        CreateCellPath(0, 1, mMovement.y);
        CreateCellPath(0, -1, mMovement.y);

        // Upper diagonal
        CreateCellPath(1, 1, mMovement.z);
        CreateCellPath(-1, 1, mMovement.z);

        // Lower diagonal
        CreateCellPath(-1, -1, mMovement.z);
        CreateCellPath(1, -1, mMovement.z);
    }

    protected void ShowCells()
    {
        foreach (Cell cell in mHighlightedCells)
        {
            cell.mOutlineImage.enabled = true;
        }
    }

    protected void ClearCells()
    {
        foreach (Cell cell in mHighlightedCells)
            cell.mOutlineImage.enabled = false;

        mHighlightedCells.Clear();
    }

    public virtual void Move()
    {

        // First move switch
        mIsFirstMove = false;

        // If there is an enemy piece, remove it
        if (mTargetCell != mCurrentCell)
        {
            mTargetCell.RemovePiece();
        }

        // Clear current
        mCurrentCell.mCurrentPiece = null;

        // Switch cells
        mCurrentCell = mTargetCell;
        mCurrentCell.mCurrentPiece = this;

        // Move on board
        transform.position = mCurrentCell.transform.position;
        mTargetCell = null;

        this.gameObject.name = mCurrentCell.mBoardPosition.x + "+" + mCurrentCell.mBoardPosition.y;
    }
    #endregion

    #region Events
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        // Test for cells
        CheckPathing();

        // Show valid cells
        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        // Follow pointer
        transform.position += (Vector3)eventData.delta;

        // Check for overlapping available squares
        foreach (Cell cell in mHighlightedCells)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition))
            {
                // If the mouse is within a valid cell, get it, and break.
                mTargetCell = cell;
                break;
            }

            // If the mouse is not within any highlighted cell, we don't have a valid move.
            mTargetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // Hide
        ClearCells();

        // Return to original position
        if (!mTargetCell)
        {
            transform.position = mCurrentCell.gameObject.transform.position;
            return;
        }
        Move();

        // Move to new cell


        // End turn
        if (!turnExtra && !this.extraMovement)
        {
            mPieceManager.SwitchSides(mColor);
        }
        if (this.extraMovement) extraMovement = false;
        if (turnExtra) turnExtra = false;

    }
    #endregion
 
    public void ComputerMove(ComputerMove move)
    {
        // Get random cell
        //int i = UnityEngine.Random.Range(0, mHighlightedCells.Count);
        mTargetCell = move.secondPosition;

        // Move to new cell
        Move();

        // End turn

        if (!turnExtra && !this.extraMovement)
        {
            mPieceManager.SwitchSides(mColor);
        }
        if (this.extraMovement) extraMovement = false;
        if (turnExtra) turnExtra = false;
    }

    public void computerOptions()
    {
        // Test for cells
        CheckPathing();

        // Show valid cells
        ShowCells();
    }

 }
