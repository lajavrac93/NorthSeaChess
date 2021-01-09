using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool mIsKingAlive = true;
    public bool isBlackTurn = false;
    public GameObject mPiecePrefab;
    public List<BasePiece> mWhitePieces = null;
    public List<BasePiece> mBlackPieces = null;
    private List<BasePiece> mPromotedPieces = new List<BasePiece>();

    private string[] mPieceOrder = new string[26]
    {
        "R", "KN","KN" ,"B", "B", "Q", "K","Q", "B", "B","KN","KN", "R",
        "P", "P", "P", "P", "P", "P", "P", "P", "P","P","P","P","P"
        
    };

    private Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>()
    {
        {"P",  typeof(Pawn)},
        {"R",  typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B",  typeof(Bishop)},
        {"K",  typeof(King)},
        {"Q",  typeof(Queen)}
    };

    public void Setup(Board board)
    {
        // Create white pieces
        mWhitePieces = CreatePieces(Color.white, new Color32(247, 242, 220, 255));

        // Create place pieces
        mBlackPieces = CreatePieces(Color.black, new Color32(25, 25, 25, 255));

        // Place pieces
        PlacePieces(0, 1, mWhitePieces, board);
        PlacePieces(12, 11, mBlackPieces, board);

        // White goes first
        SwitchSides(Color.black);
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor)
    {
        List<BasePiece> newPieces = new List<BasePiece>();

        for (int i = 0; i < mPieceOrder.Length; i++)
        {
            // Get the type
            string key = mPieceOrder[i];
            Type pieceType = mPieceLibrary[key];

            // Create
            BasePiece newPiece = CreatePiece(pieceType);
            newPieces.Add(newPiece);

            // Setup
            newPiece.Setup(teamColor, spriteColor, this);
        }

        return newPieces;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        // Create new object
        GameObject newPieceObject = Instantiate(mPiecePrefab);
        newPieceObject.transform.SetParent(transform);

        // Set scale and position
        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        // Store new piece
        BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);

        return newPiece;
    }

    private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < 13; i++)
        {
            // Place pawns    
            pieces[i].Place(board.mAllCells[i, pawnRow]);

            // Place royalty
            pieces[i + 13].Place(board.mAllCells[i, royaltyRow]);
        }
    }

    private void SetInteractive(List<BasePiece> allPieces, bool value)
    {
        foreach (BasePiece piece in allPieces)
            piece.enabled = value;
    }

    public void SwitchSides(Color color)
    {
        if (!mIsKingAlive)
        {
            // Reset pieces
            ResetPieces();

            // King has risen from the dead
            mIsKingAlive = true;

            // Change color to black, so white can go first again
            color = Color.black;
        }
        if (listExtraMovements().Count>0)
        {
            this.consumeMovement();
        }
        
        
         isBlackTurn = color == Color.white ? true : false;
        
        // Set team interactivity
        SetInteractive(mWhitePieces, !isBlackTurn);

        // Disable this so player can't move pieces
        SetInteractive(mBlackPieces, isBlackTurn);

        // Set promoted interactivity
        foreach (BasePiece piece in mPromotedPieces)
        {
            bool isBlackPiece = piece.mColor != Color.white ? true : false;
            bool isPartOfTeam = isBlackPiece == true ? isBlackTurn : !isBlackTurn;

            piece.enabled = isPartOfTeam;
        }

    }

    public void ResetPieces()
    {
        foreach (BasePiece piece in mPromotedPieces)
        {
            piece.Kill();
            Destroy(piece.gameObject);
        }

        mPromotedPieces.Clear();

        foreach (BasePiece piece in mWhitePieces)
            piece.Reset();

        foreach (BasePiece piece in mBlackPieces)
            piece.Reset();
    }

    public void PromotePiece(Pawn pawn, Cell cell, Color teamColor, Color spriteColor)
    {
        // Kill Pawn
        pawn.Kill();

        // Create
        BasePiece promotedPiece = CreatePiece(typeof(Queen));
        promotedPiece.Setup(teamColor, spriteColor, this);

        // Place piece
        promotedPiece.Place(cell);

        // Add
        mPromotedPieces.Add(promotedPiece);
    }


    public static List<BasePiece> listExtraMovements()
    {
        PieceManager pieceManager = GameObject.FindObjectOfType<PieceManager>();
        List<BasePiece> piecesExtraMovement = new List<BasePiece>();
        if (pieceManager.isBlackTurn)
        {
            foreach (BasePiece piece in pieceManager.mBlackPieces)
            {
                if (piece.extraMovement) piecesExtraMovement.Add(piece);
            }
        }
        else
        {
            foreach (BasePiece piece in pieceManager.mWhitePieces)
            {
                if (piece.extraMovement) piecesExtraMovement.Add(piece);
            }
        }
        return piecesExtraMovement;
    }

    public void consumeMovement()
    {
        foreach (BasePiece piece in listExtraMovements())
        {
            if (haveMovements())
            {
                piece.CheckPathing();
            }
           
        }
    }

    public bool haveMovements()
    {
        PieceManager pieceManager = GameObject.FindObjectOfType<PieceManager>();
        if (pieceManager.isBlackTurn)
        {
            foreach (BasePiece piece in pieceManager.mBlackPieces)
            {
                if (piece.extraMovement) return true;
            }
        }
        else
        {
            foreach (BasePiece piece in pieceManager.mWhitePieces)
            {
                if (piece.extraMovement) return true;
            }
        }
        return false;
    }


    
    public void moveComputerPiece(ComputerMove move)
    {
        
        move.pieceMoved.ComputerMove(move);
    }
    


}
