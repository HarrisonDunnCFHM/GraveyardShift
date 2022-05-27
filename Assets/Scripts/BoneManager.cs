using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneManager : MonoBehaviour
{
    //cached references
    List<GamePiece> allPieces = new List<GamePiece>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ConvertToBone();
    }

    private void ConvertToBone()
    {
        allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        foreach(GamePiece piece in allPieces)
        {
            if(CheckForEnclosed(piece, Vector3.up)
                && CheckForEnclosed(piece, Vector3.right)
                && CheckForEnclosed(piece, Vector3.down)
                && CheckForEnclosed(piece, Vector3.left))
            {
                piece.myType = GamePiece.PieceType.Bone;
            }
        }
    }

    private bool CheckForEnclosed(GamePiece pieceToCheck, Vector3 direction)
    {
        foreach(GamePiece piece in allPieces)
        {
            if(pieceToCheck.transform.position == piece.transform.position + direction)
            {
                if (piece.myType == GamePiece.PieceType.Dirt
                || piece.myType == GamePiece.PieceType.Bone
                || piece.myType == pieceToCheck.myType)
                {
                    return true;
                }
            }
        }
        if(pieceToCheck.transform.position.x == -4 && direction == Vector3.left) { return true; }
        if(pieceToCheck.transform.position.x == 5 && direction == Vector3.right) { return true; }
        if(pieceToCheck.transform.localPosition.y == -11 && direction == Vector3.down) {
            Debug.Log(pieceToCheck.name + " is at the bottom of a column");
            return true; }
        return false;
    }
}
