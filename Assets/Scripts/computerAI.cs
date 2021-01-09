using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class computerAI : MonoBehaviour
{
    List<BasePiece> mWhitePieces;
    List<BasePiece> mBlackPieces;
    Stack<ComputerMove> moveStack = new Stack<ComputerMove>();
    Weights weight = new Weights();
    ComputerMove bestMove;

    private void Start()
    {
        mWhitePieces = GameObject.FindObjectOfType<PieceManager>().mWhitePieces;
        mBlackPieces = GameObject.FindObjectOfType<PieceManager>().mBlackPieces;
    }

    

    public int evaluation()
    {
        int scoreWhite = 0, scoreBlack = 0;

        foreach (BasePiece item in mWhitePieces)
        {
            scoreWhite += weight.GetBoardWeight(item.typePiece, item.mCurrentCell, Color.white ); ;
        }
        foreach (BasePiece item in mBlackPieces)
        {
            scoreBlack += weight.GetBoardWeight(item.typePiece, item.mCurrentCell, Color.black);
        }
        if (scoreWhite > scoreBlack)
        {
            return scoreWhite - scoreBlack;
        }
        else
        {
            return scoreBlack - scoreWhite;
        }
    }

    public ComputerMove getMove()
    {
        Board board = GameObject.FindObjectOfType<Board>();
        bestMove = new ComputerMove(board.mAllCells[11,11], board.mAllCells[11, 11], board.mAllCells[11, 11].mCurrentPiece);
        minMax(4, -100000000, 1000000000, true);
        return bestMove;
    }

    int minMax(int depth, int option1, int option2, bool max)
    {

        if (depth == 0)
        {
            return evaluation();
        }
        if (max)
        {
            int score = -10000000;
            List<ComputerMove> allMoves = getComputerMoves(new Color32(25, 25, 25, 255));
            foreach (ComputerMove move in allMoves)
            {
                moveStack.Push(move);

                fakeMove(move.firstPosition, move.secondPosition);

                score = minMax(depth - 1, option1, option2, false);

                undoFakeMove(move);

                if (score > option1)
                {
                    move.score = score;
                    if (move.score > bestMove.score && depth == 4)
                    {
                        bestMove = move;
                    }
                    option1 = score;
                }
                if (score >= option2)
                {
                    break;
                }
            }
            return option1;
        }
        else
        {
            int score = 10000000;
            List<ComputerMove> allMoves = getComputerMoves(new Color32(247, 242, 220, 255));
            foreach (ComputerMove move in allMoves)
            {
                moveStack.Push(move);

                fakeMove(move.firstPosition, move.secondPosition);

                score = minMax(depth - 1, option1, option2, true);

                undoFakeMove(move);

                if (score < option2)
                {
                    move.score = score;
                    option2 = score;
                }
                if (score <= option1)
                {
                    break;
                }
            }
            return option2;
        }
        
    }
    List<ComputerMove> getComputerMoves(Color32 color)
    {
        List<ComputerMove> posibleMoves = new List<ComputerMove>();
        List<BasePiece> pieces = new List<BasePiece>();
        if (color.Equals(new Color32(25, 25, 25, 255)))
        {
            pieces = mBlackPieces;
        }
        else
        {
            pieces = mWhitePieces;
        }

        foreach (BasePiece piece in pieces)
        {
            piece.computerOptions();
            foreach (Cell tile in piece.mHighlightedCells)
            {
                posibleMoves.Add(new ComputerMove(piece.mCurrentCell, tile, piece));
            }
        }

        return posibleMoves;
    }

    void fakeMove(Cell currentTile, Cell targetTile)
    {
        targetTile.mCurrentPiece = currentTile.mCurrentPiece;
        currentTile.mCurrentPiece = null;
    }

    void undoFakeMove(ComputerMove tempMove)
    {      
        Cell movedTo = tempMove.secondPosition;
        Cell movedFrom = tempMove.firstPosition;
        BasePiece pieceKilled = tempMove.pieceKilled;
        BasePiece pieceMoved = tempMove.pieceMoved;

        movedFrom.mCurrentPiece = movedTo.mCurrentPiece;

        if (pieceKilled != null)
        {
            movedTo.mCurrentPiece = pieceKilled;
        }
        else
        {
            movedTo.mCurrentPiece = null;
        }
    }

}
