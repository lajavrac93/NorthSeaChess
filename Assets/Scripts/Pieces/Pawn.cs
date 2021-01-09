using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Pawn : BasePiece
{
    
    

    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Pawn Stuff
        mMovement = mColor == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
        
        this.typePiece = BasePiece.pieceType.PAWN;

    }

    public override void Move()
    {

        if (this.mCurrentCell != this.mTargetCell)
        {
            base.Move();
            GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
        }
        else
        {
            supplies();
            base.Move();

        }

        CheckForPromotion();

    }

    private bool MatchesState(int targetX, int targetY, CellState targetState)
    {
        CellState cellState = CellState.None;
        cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);
        if (cellState == targetState)
        {
            mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
            return true;
        }

        return false;
    }



    private void CheckForPromotion()
    {
        // Target position
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        // Check if pawn has reached the end of the board
        CellState cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY + mMovement.y, this);

        if (cellState == CellState.OutOfBounds)
        {
            Color spriteColor = GetComponent<Image>().color;
            mPieceManager.PromotePiece(this, mCurrentCell, mColor, spriteColor);
        }
    }

    public override void CheckPathing()
    {
        // Target position
        int currentX = mCurrentCell.mBoardPosition.x;
        int currentY = mCurrentCell.mBoardPosition.y;

        // Top left
        MatchesState(currentX - mMovement.z, currentY + mMovement.z, CellState.Enemy);

        // Forward
        if (MatchesState(currentX, currentY + mMovement.y, CellState.Free))
        {
            // If the first forward cell is free, and first move, check for next
            if (mIsFirstMove)
            {
                MatchesState(currentX, currentY + (mMovement.y * 2), CellState.Free);
            }
        }
        //This
        MatchesState(currentX, currentY, CellState.Friendly);
        // Top right
        MatchesState(currentX + mMovement.z, currentY + mMovement.z, CellState.Enemy);

    }

    

    public void supplies()
    {
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("tent");
        List<BasePiece> pieces = new List<BasePiece>();
        if (this.mPieceManager.isBlackTurn)
        {
            pieces = this.mPieceManager.mBlackPieces;
        }
        else
        {
            pieces = this.mPieceManager.mWhitePieces;
        }

        foreach (BasePiece tile in pieces)
        {
            if (tile.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                try
                {
                    if (this.GetComponent<CircleCollider2D>().bounds.Intersects(tile.gameObject.GetComponent<Collider2D>().bounds))
                    {
                        //Debug.Log("[" + this.tag + "] found a neighbour: " + tile.tag);
                        tile.extraMovement = true;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }

    
}
