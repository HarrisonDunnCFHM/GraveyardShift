using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovePicker : MonoBehaviour
{
    //config params
    [SerializeField] int minPiecesToClear = 3;
    [SerializeField] float selectionRadious = 0.5f;
    [SerializeField] float distanceToDiagonal = 1.5f;
    
    //cached references
    public GamePiece.PieceType moveColor;
    public List<GamePiece> pickablePieces = new List<GamePiece>();
    public List<GamePiece> selectedPieces = new List<GamePiece>();
    GamePiece lastPickedPiece;
    LineRenderer lineRenderer;
    List<Transform> linePoints = new List<Transform>();
    public bool clusterDebugger;
    
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ResetDebug();
        PickPieces();
        ProcessMove();
        DrawMoveLine();
    }

    private void ResetDebug()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            GamePiece.destroying = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void DrawMoveLine()
    {
        lineRenderer.positionCount = selectedPieces.Count + 1;
        if(selectedPieces.Count == 0) { return; }
        Color lineColor = selectedPieces[0].myHighlighter.color;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        for (int i = 0; i <= selectedPieces.Count; ++i)
        {
            Vector3 drawPos;
            if (i >= selectedPieces.Count)
            {
                drawPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector3.Distance(selectedPieces[i - 1].transform.position, drawPos) < 0.5f)
                {
                    drawPos = selectedPieces[i - 1].transform.position;
                }
            }
            else
            {
                drawPos = selectedPieces[i].transform.position;
            }
            lineRenderer.SetPosition(i, drawPos);
        }
    }

    private void PickPieces()
    {
        List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        pickablePieces = new List<GamePiece>();
        foreach (GamePiece piece in allPieces)
        {
            if (piece.myType != GamePiece.PieceType.Dirt
                && piece.myType != GamePiece.PieceType.Bone)
            {
                pickablePieces.Add(piece);
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 clickedPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach(GamePiece piece in pickablePieces)
            {
                if (Vector3Int.RoundToInt(clickedPoint) == piece.transform.position)
                {
                    moveColor = piece.myType;
                    piece.isSelected = true;
                    selectedPieces.Add(piece);
                    lastPickedPiece = piece;
                }
            }
        }
        if(Input.GetMouseButton(0))
        {
            Vector2 currentMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (GamePiece piece in pickablePieces)
            {
                if (Vector2.Distance(currentMousePoint,piece.transform.position) <= selectionRadious
                    && piece.myType == moveColor
                    && Vector2.Distance(piece.transform.position,lastPickedPiece.transform.position) < distanceToDiagonal
                    && !selectedPieces.Contains(piece))
                {
                    //moveColor = piece.myType;
                    piece.isSelected = true;
                    selectedPieces.Add(piece);
                    lastPickedPiece = piece;
                }
                else if(Vector2.Distance(currentMousePoint, piece.transform.position) <= selectionRadious
                    && selectedPieces.Count > 1)
                {
                    if (piece == selectedPieces[^2])
                    {
                        lastPickedPiece.isSelected = false;
                        selectedPieces.RemoveAt(selectedPieces.Count - 1);
                        lastPickedPiece = piece;
                    }
                }
            }
        }
    }
    private void ProcessMove()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (selectedPieces.Count >= minPiecesToClear)
            {
                foreach (GamePiece piece in selectedPieces)
                {
                    piece.isSelected = false;
                    piece.myType = GamePiece.PieceType.Dirt;
                }
            }
            else
            {
                foreach (GamePiece piece in selectedPieces)
                {
                    piece.isSelected = false;
                }
            }
            selectedPieces = new List<GamePiece>();
        }
    }
}
