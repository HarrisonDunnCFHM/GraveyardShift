using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePiece : MonoBehaviour
{
    public enum PieceType { Coffin, Lantern, Grave, Shovel, Dirt, Bone, Flower, Candle, Robber};
    
    //config params
    [SerializeField] float dropSpeed = 10f;
    [SerializeField] Sprite dirtBlock;
    [SerializeField] Sprite boneSprite;
    [SerializeField] public SpriteRenderer myHighlighter;
    [SerializeField] public PieceType myType;
    [SerializeField] TextMeshProUGUI myClusterCounter;
    [SerializeField] float timeBetweenParticleBursts = 0.1f;
    [SerializeField] float waitAtBottomCountdown = 0.2f;



    //cached references
    public Vector2Int nextFallTarget;
    public GameObject myColumn;
    public bool isSelected;
    public bool isDropping;
    public bool isDestroying = false;
    public bool atBottom = false;
    public List<GamePiece> myCluster = new List<GamePiece>();
    public List<GamePiece> myDiagonalCluster = new List<GamePiece>();
    public static bool destroying = false;
    int lastFrameClusterCount = 0;
    public int myColumnBottom;
    float waitAtBottomTimer = 0f;

    SpecialPieceManager dirtManager;
    SpriteRenderer myRenderer;
    MovePicker movePicker;
    LevelManager levelManager;
    
    // Start is called before the first frame update
    void Start()
    {
        myCluster.Add(this);
        nextFallTarget = new Vector2Int(0, -2);
        myRenderer = GetComponent<SpriteRenderer>();
        myHighlighter.gameObject.SetActive(false);
        dirtManager = FindObjectOfType<SpecialPieceManager>();
        movePicker = FindObjectOfType<MovePicker>();
        // myColumn = GetComponentInParent<Transform>().gameObject;
        myColumnBottom = myColumn.GetComponent<SpawnColumn>().columnBottom;
        levelManager = FindObjectOfType<LevelManager>();
    }



    // Update is called once per frame
    void Update()
    {
        IfHovered();
        DisplayHighlight();
        DisplayTypeChange();
        AssembleMyCluster();
        //AssembleDiagonalCluster();
        DropPiece();
        CheckIfBones();
        FallOffBottom();
        DebugClusterCounter();
    }

    private void DebugClusterCounter()
    {
        if (movePicker.clusterDebugger)
        {
            myClusterCounter.enabled = true;
            myClusterCounter.text = myCluster.Count.ToString();
        }
        else
        {
            myClusterCounter.enabled = false;
        }
    }

    private void IfHovered()
    {
        if (levelManager.levelLost) { return; }
        Vector2Int mousePos = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector2Int myPos = Vector2Int.RoundToInt(transform.position);
        if(mousePos == myPos)
        {
            if (myType != PieceType.Bone && myType != PieceType.Dirt && myType != PieceType.Robber)
            {
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
        else if(!isDestroying)
        {
            transform.localScale = Vector3.one;
        }
    }

    private void AssembleMyCluster()
    {
        List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        //check adjacent pieces, if they're the same color, add to cluster
        
        foreach (GamePiece piece in allPieces.ToArray())
        {
            if (piece.transform.position == transform.position + Vector3.up
                || piece.transform.position == transform.position + Vector3.right
                || piece.transform.position == transform.position + Vector3.down
                || piece.transform.position == transform.position + Vector3.left)
            {
                if (piece.myType == myType && !myCluster.Contains(piece))
                {
                    myCluster.Add(piece);
                }
            }
        }
        //repeat for each piece of the cluster
        foreach (GamePiece clusterPiece in myCluster.ToArray())
        {
            foreach (GamePiece piece in allPieces.ToArray())
            {
                if (piece.transform.position == clusterPiece.transform.position + Vector3.up
                    || piece.transform.position == clusterPiece.transform.position + Vector3.right
                    || piece.transform.position == clusterPiece.transform.position + Vector3.down
                    || piece.transform.position == clusterPiece.transform.position + Vector3.left)
                {
                    if (piece.myType == myType && !myCluster.Contains(piece))
                    {
                        myCluster.Add(piece);
                    }
                }
            }
        }
        //remove pieces that are no longer adjacent to another piece in the cluster
        foreach(GamePiece piece in myCluster.ToArray())
        {
            //if(piece == null) { myCluster.Remove(piece); }
            bool isStillAdjacent = false;
            foreach(GamePiece otherPiece in myCluster)
            {
                if(otherPiece == piece) { isStillAdjacent = true; }
                if (piece.transform.position == otherPiece.transform.position + Vector3.up
                                    || piece.transform.position == otherPiece.transform.position + Vector3.right
                                    || piece.transform.position == otherPiece.transform.position + Vector3.down
                                    || piece.transform.position == otherPiece.transform.position + Vector3.left)
                {
                    isStillAdjacent = true;
                    break;
                }
            }
            if (!isStillAdjacent) { myCluster.Remove(piece); }
        }

    }

    private void AssembleDiagonalCluster()
    {
        List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        //check diagonal pieces, if they're the same color, add to cluster
        foreach(GamePiece piece in myCluster)
        {
            Vector2Int upperRightPos = Vector2Int.RoundToInt(piece.transform.position + Vector3.right + Vector3.up);
            Vector2Int lowerRightPos = Vector2Int.RoundToInt(piece.transform.position + Vector3.right + Vector3.down);
            Vector2Int upperLeftPos = Vector2Int.RoundToInt(piece.transform.position + Vector3.left + Vector3.up);
            Vector2Int lowerLeftPos = Vector2Int.RoundToInt(piece.transform.position + Vector3.left + Vector3.down);
            foreach(GamePiece otherPiece in allPieces)
            {
                Vector2Int otherPos = Vector2Int.RoundToInt(otherPiece.transform.position);
                if (otherPos == upperRightPos || otherPos == lowerRightPos || otherPos == upperLeftPos || otherPos == lowerLeftPos)
                {
                    if(!myDiagonalCluster.Contains(otherPiece))
                    {
                        myDiagonalCluster.Add(otherPiece);
                    }
                }
            }
        }

    }

    private void DisplayHighlight()
    {
        if(isSelected)
        {
            myHighlighter.gameObject.SetActive(true);
        }
        else
        {
            myHighlighter.gameObject.SetActive(false);
        }
    }

    private void DisplayTypeChange()
    {
        if (myType == PieceType.Dirt)
        {
            myRenderer.sprite = dirtBlock;
            myRenderer.color = Color.white;
            myHighlighter.gameObject.SetActive(false);
            if(myCluster.Count > 0 && !isDestroying)
            {
                foreach(GamePiece piece in myCluster)
                {
                    if(piece.myType != myType)
                    {
                        piece.myCluster = new List<GamePiece>();
                        piece.myCluster.Add(piece);
                        piece.myDiagonalCluster = new List<GamePiece>();
                        piece.myDiagonalCluster.Add(piece); 
                        break;
                    }
                }
            }
        }
        if (myType == PieceType.Bone)
        {
            myRenderer.sprite = boneSprite;
            myRenderer.color = Color.white;
            myHighlighter.gameObject.SetActive(false); 
            if (myCluster.Count > 0)
            {
                foreach (GamePiece piece in myCluster)
                {
                    if (piece.myType != myType)
                    {
                        piece.myCluster = new List<GamePiece>();
                        piece.myCluster.Add(piece);
                        piece.myDiagonalCluster = new List<GamePiece>();
                        piece.myDiagonalCluster.Add(piece); 
                        break;
                    }
                }
            }
        }
    }

    private void DropPiece()
    {
        isDropping = true;
        FindTarget();
        float step = dropSpeed * Time.deltaTime;
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, nextFallTarget, step);
        if (transform.localPosition == new Vector3(nextFallTarget.x, nextFallTarget.y, 0)) 
        { isDropping = false; }
        else
        {
            myCluster = new List<GamePiece>();
            myCluster.Add(this);
        }
    }

    private void FindTarget()
    {
        Vector2Int belowMe = nextFallTarget + Vector2Int.down;
        foreach(Transform child in myColumn.transform)
        {
            if(Vector2Int.RoundToInt(child.transform.localPosition) == belowMe)
            {
                return;
            }
        }
        if(belowMe.y < myColumnBottom)
        {
            return;
        }
        nextFallTarget = belowMe;
    }

    private void CheckIfBones()
    {
        if(myType == PieceType.Dirt) { return; }
        List<GamePiece> myClustersAdjacents = new List<GamePiece>();
        List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        foreach (GamePiece clusterPiece in myCluster.ToArray())
        {
            //check each orthagonal piece. if it's not in mycluster, add it to adj cluster
            foreach (GamePiece piece in allPieces.ToArray())
            {
                if (piece.transform.position == clusterPiece.transform.position + Vector3.up
                    || piece.transform.position == clusterPiece.transform.position + Vector3.right
                    || piece.transform.position == clusterPiece.transform.position + Vector3.down
                    || piece.transform.position == clusterPiece.transform.position + Vector3.left)
                {
                    if (!myCluster.Contains(piece))
                    {
                        myClustersAdjacents.Add(piece);
                    }
                }
            }
        }
        if(myClustersAdjacents.Count <= 2) { return; }
        bool isBone = true;
        foreach (GamePiece adjacentPiece in myClustersAdjacents)
        {
            if (adjacentPiece.myType != PieceType.Dirt && adjacentPiece.myType != PieceType.Bone)
            {
                isBone = false;
            }
        }
        if (isBone && myClustersAdjacents.Count > 0 && !isDropping) { myType = PieceType.Bone; }
    }

    private void FallOffBottom()
    {
        if (isDropping) { return; }
        if (transform.localPosition.y == myColumnBottom && myType == PieceType.Robber)
        {
            levelManager.LevelLost();
            transform.localScale = new Vector2(2, 2);
            GetComponent<SpriteRenderer>().sortingOrder = 11;
        }

            if (transform.localPosition.y == myColumnBottom && myType == PieceType.Dirt)
        {
            foreach(GamePiece piece in myCluster)
            {
                piece.isDestroying = true;
                piece.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
        }
        if(atBottom == false)
        {
            atBottom = true;
            myCluster = new List<GamePiece>();
            myCluster.Add(this);
            lastFrameClusterCount = 0;
        }
        if(lastFrameClusterCount != myCluster.Count) { lastFrameClusterCount = myCluster.Count; return; }
        if (destroying) { return; }
        if(waitAtBottomTimer < waitAtBottomCountdown)
        {
            waitAtBottomTimer += Time.deltaTime;
            return;
        }
        else
        {
            waitAtBottomTimer = 0f;
        }
        if (transform.localPosition.y == myColumnBottom)
        {
            if (myType == PieceType.Dirt || myType == PieceType.Bone)
            {
                destroying = true;
                dirtManager.RunPieceDestruction(myCluster.ToArray());
            }
        }
    }

    

    private void OnDestroy()
    {
        if (myType == PieceType.Bone) { dirtManager.boneCount++; }
        List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        foreach(GamePiece piece in myCluster.ToArray())
        {
            piece.myCluster.Remove(this);
        }
        foreach(GamePiece piece in allPieces)
        {
            piece.myCluster = new List<GamePiece>();
            piece.myCluster.Add(piece);
            piece.isDestroying = false;
        }
    }
}
