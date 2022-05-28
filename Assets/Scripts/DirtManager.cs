using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DirtManager : MonoBehaviour
{
    //config paramters
    [SerializeField] float timeBetweenParticleBursts = 0.1f;
    [SerializeField] float disappearDelay = 0.5f;
    [SerializeField] ParticleSystem myParticles;
    [SerializeField] TextMeshProUGUI dirtCounter;
    [SerializeField] TextMeshProUGUI boneCounter;
    [SerializeField] List<AudioClip> dirtSounds;


    //cached references
    List<GamePiece> allPieces = new List<GamePiece>();
    public List<GamePiece> bottomPieces = new List<GamePiece>();
    int dirtCount;
    int boneCount;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dirtCounter.text = "Dirt: " + dirtCount.ToString();
        boneCounter.text = "Bones: " + boneCount.ToString();
    }

    public void RunPieceDestruction(GamePiece[] cluster)
    {
        for (int i = 0; i < cluster.Length; ++i)
        {
            StartCoroutine(DestroyPiece(cluster, i));
        }
    }

    private IEnumerator DestroyPiece(GamePiece[] cluster, int index)
    {
        float particleTimer = index * timeBetweenParticleBursts;
        yield return new WaitForSeconds(particleTimer);
        if (cluster[index] != null)
        {
            GameObject particleBurst = Instantiate(myParticles.gameObject, cluster[index].gameObject.transform.position, Quaternion.identity);
            particleBurst.GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForSeconds(disappearDelay);
        cluster[index].GetComponent<SpriteRenderer>().enabled = false;
        PlayDirtSound();
        if (cluster[index].myType == GamePiece.PieceType.Bone) { boneCount++; }
        if (cluster[index].myType == GamePiece.PieceType.Dirt) { dirtCount++; }
        yield return new WaitForSeconds(((cluster.Length + 2) * timeBetweenParticleBursts) - particleTimer + disappearDelay);
        if (cluster[index] != null)
        {
            Destroy(cluster[index].gameObject);
        }
        GamePiece.destroying = false;
    }
    private void PlayDirtSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, dirtSounds.Count);
        AudioSource.PlayClipAtPoint(dirtSounds[randomIndex], Camera.main.transform.position, 0.4f);
    }
}
