using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    //config parameters
    [SerializeField] List<GamePiece> spawnablePieces;
    [SerializeField] List<SpawnColumn> spawnableLocations;
    [SerializeField] float spawnPieceCooldown = 0.1f;

    //cached refs
    bool canSpawn = true;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
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
        if (column.transform.childCount < 10)
        {
            GamePiece newPiece = Instantiate(GetRandomPiece(), column.gameObject.transform.position, Quaternion.identity);
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
}
