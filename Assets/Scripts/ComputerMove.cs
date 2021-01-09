using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerMove 
{
    public Cell firstPosition = null;
    public Cell secondPosition = null;
    public BasePiece pieceMoved = null;
    public BasePiece pieceKilled = null;
    public int score = -100000000;

    public  ComputerMove(Cell cell1, Cell cell2, BasePiece piece)
    {
        this.firstPosition = cell1;
        this.secondPosition = cell2;
        this.pieceMoved = piece;
    }
}
