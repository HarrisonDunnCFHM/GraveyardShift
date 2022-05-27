using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePiece : MonoBehaviour
{
    public enum PieceType { Coffin, Lantern, Grave, Shovel, Dirt, Bone};
    
    //config params
    [SerializeField] float dropSpeed = 10f;
    [SerializeField] Sprite dirtBlock;
    [SerializeField] Sprite boneSprite;
    [SerializeField] public SpriteRenderer myHighlighter;
    [SerializeField] public PieceType myType;
    [SerializeField] TextMeshProUGUI myClusterCounter;
    [SerializeField] float timeBetweenParticleBursts = 0.1f;



    //cached references
    public Vector2Int nextFallTarget;
    public GameObject myColumn;
    public bool isSelected;
    public bool isDropping;
    public bool isDestroying = false;
    public List<GamePiece> myCluster = new List<GamePiece>();
    public static bool destroying = false;
    int lastFrameClusterCount = 0;

    DirtManager dirtManager;
    SpriteRenderer myRenderer;
    ParticleSystem myParticles;
    MovePicker movePicker;
    
    // Start is called before the first frame update
    void Start()
    {
        myCluster.Add(this);
        nextFallTarget = new Vector2Int(0, -2);
        myRenderer = GetComponent<SpriteRenderer>();
        myHighlighter.gameObject.SetActive(false);
        myParticles = GetComponentInChildren<ParticleSystem>();
        dirtManager = FindObjectOfType<DirtManager>();
        movePicker = FindObjectOfType<MovePicker>();
    }

    

    // Update is called once per frame
    void Update()
    {
        DropPiece();
        AssembleMyCluster();
        if (movePicker.clusterDebugger) 
        {
            myClusterCounter.enabled = true;
            myClusterCounter.text = myCluster.Count.ToString(); 
        } 
        else 
        { 
            myClusterCounter.enabled = false; 
        }
        DisplayTypeChange();
        DisplayHighlight();
        CheckIfBones();
        FallOffBottom();
    }


    private void AssembleMyCluster()
    {
        if (isDropping 
            /*|| myType == PieceType.Bone 
            || myType == PieceType.Dirt*/) { return; }
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
            transform.localScale = Vector3.one;
            myHighlighter.gameObject.SetActive(false);
            if(myCluster.Count > 0 && !isDestroying)
            {
                foreach(GamePiece piece in myCluster)
                {
                    if(piece.myType != myType)
                    {
                        piece.myCluster = new List<GamePiece>();
                        piece.myCluster.Add(piece);
                        break;
                    }
                }
            }
        }
        if (myType == PieceType.Bone)
        {
            myRenderer.sprite = boneSprite;
            myRenderer.color = Color.white;
            transform.localScale = Vector3.one;
            myHighlighter.gameObject.SetActive(false); 
            if (myCluster.Count > 0)
            {
                foreach (GamePiece piece in myCluster)
                {
                    if (piece.myType != myType)
                    {
                        piece.myCluster = new List<GamePiece>();
                        piece.myCluster.Add(piece);
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
        if (transform.localPosition == new Vector3(nextFallTarget.x, nextFallTarget.y, 0)) { isDropping = false; }
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
        if(belowMe.y <= -12)
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
        bool isBone = true;
        foreach (GamePiece adjacentPiece in myClustersAdjacents)
        {
            if (adjacentPiece.myType != PieceType.Dirt)
            {
                isBone = false;
            }
        }
        if (isBone && myClustersAdjacents.Count > 0 && !isDropping) { myType = PieceType.Bone; }
    }

    private void FallOffBottom()
    {
        if(lastFrameClusterCount != myCluster.Count) { lastFrameClusterCount = myCluster.Count; return; }
        if (destroying) { return; }
        if (transform.localPosition.y == -11)
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
        List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
        foreach(GamePiece piece in myCluster.ToArray())
        {
            piece.myCluster.Remove(this);
        }
        foreach(GamePiece piece in allPieces)
        {
            piece.myCluster = new List<GamePiece>();
            piece.myCluster.Add(piece);
        }
    }
}
