using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    //config parameters
    [SerializeField] List<GamePiece> spawnablePieces;
    [SerializeField] int robberChance = 10;
    [SerializeField] GamePiece robber;
    [SerializeField] public List<SpawnColumn> spawnableLocations;
    [SerializeField] float spawnPieceCooldown = 0.1f;

    //cached refs
    SpecialPieceManager specialManager;
    bool canSpawn = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        specialManager = FindObjectOfType<SpecialPieceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageColumns();
    }

    private void ManageColumns()
    {
        foreach(SpawnColumn column in spawnableLocations)
        {
            if (!column.canSpawn) { return; }
            StartCoroutine(CreateGamePiece(column));
        }
    }

    private IEnumerator CreateGamePiece(SpawnColumn column)
    {
        column.canSpawn = false;
        if (column.transform.childCount == 0 && column.transform.childCount < column.columnCapacity)
        {
            GamePiece newPiece = Instantiate(GetRandomPiece(), column.gameObject.transform.position, Quaternion.identity);
            newPiece.transform.parent = column.gameObject.transform;
            newPiece.myColumn = column.gameObject;
        }
        else if (column.transform.childCount < column.columnCapacity)
        {
            GamePiece newPiece = Instantiate(GetRandomPieceOrRobber(), column.gameObject.transform.position, Quaternion.identity);
            newPiece.transform.parent = column.gameObject.transform;
            newPiece.myColumn = column.gameObject;
        }
        yield return new WaitForSeconds(spawnPieceCooldown);
        column.canSpawn = true;
    }



    private GamePiece GetRandomPiece()
    {
        int indexToSpawn = UnityEngine.Random.Range(0, spawnablePieces.Count);
        return spawnablePieces[indexToSpawn];
    }
    private GamePiece GetRandomPieceOrRobber()
    {
        int randomRobberChance = UnityEngine.Random.Range(1, 101);
        if (randomRobberChance <= robberChance)
        {
            List<GamePiece> allPieces = new List<GamePiece>(FindObjectsOfType<GamePiece>());
            List<GamePiece> robberPieces = new List<GamePiece>();
            foreach (GamePiece piece in allPieces)
            {
                if (piece.myType == GamePiece.PieceType.Robber)
                {
                    robberPieces.Add(piece);
                }
            }
            if (robberPieces.Count < specialManager.robberMax)
            {
                return robber;
            }
        }
        int indexToSpawn = UnityEngine.Random.Range(0, spawnablePieces.Count);
        return spawnablePieces[indexToSpawn];
    }
}
