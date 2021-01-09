using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image mOutlineImage;

    [HideInInspector]
    public Vector2Int mBoardPosition = Vector2Int.zero;
    [HideInInspector]
    public Board mBoard = null;
    [HideInInspector]
    public RectTransform mRectTransform = null;

    [HideInInspector]
    public BasePiece mCurrentPiece = null;
    

    public void Setup(Vector2Int newBoardPosition, Board newBoard)
    {
        mBoardPosition = newBoardPosition;
        mBoard = newBoard;
        mOutlineImage.sprite = Resources.Load<Sprite>("T_Outline");
        mOutlineImage.color = new Color32(13, 17, 222, 255);
        mRectTransform = GetComponent<RectTransform>();
       

    }

    public void RemovePiece()
    {
        if (mCurrentPiece != null)
        {
            mCurrentPiece.Kill();
        }
    }
}
