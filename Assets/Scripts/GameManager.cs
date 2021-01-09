using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public Board mBoard;
    public PieceManager mPieceManager;
    public computerAI computerAI;

    void Start()
    {
        // Create the board
        mBoard.Create();

        // Create pieces
        mPieceManager.Setup(mBoard);
    }

    private void Update()
    {
        if (mPieceManager.isBlackTurn)
        {
            //mPieceManager.MoveRandomPiece();
            mPieceManager.moveComputerPiece(computerAI.getMove());
        }
    }

    void doAIMove()
    {

        mPieceManager.moveComputerPiece(computerAI.getMove());
    }
}
