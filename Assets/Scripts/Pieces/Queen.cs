using UnityEngine;
using UnityEngine.UI;

public class Queen : BasePiece
{
    public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        // Base setup
        base.Setup(newTeamColor, newSpriteColor, newPieceManager);

        // Queen stuff
        mMovement = new Vector3Int(12, 12, 12);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("shipPiece");

        
        this.typePiece = BasePiece.pieceType.QUEEN;

    }
    public override void Kill()
    {
        base.Kill();

        turnExtra = true;
    }
}
