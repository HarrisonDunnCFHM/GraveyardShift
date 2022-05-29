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
    [SerializeField] String boneCountText;


    //cached references
    List<GamePiece> allPieces = new List<GamePiece>();
    public List<GamePiece> bottomPieces = new List<GamePiece>();
    int dirtCount;
    public int boneCount;
    bool destroying = false;
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        dirtCounter.text = "Dirt: " + dirtCount.ToString();
        boneCounter.text = boneCountText + " " + boneCount.ToString() + " of " + levelManager.requiredBonesToWin.ToString();
    }

    public void RunPieceDestruction(GamePiece[] cluster)
    {
        if (destroying) { return; }
        destroying = true;
        for (int i = 0; i < cluster.Length; ++i)
        {
            StartCoroutine(DestroyPiece(cluster, i));
        }
    }

    private IEnumerator DestroyPiece(GamePiece[] cluster, int index)
    {
        float particleTimer = index * timeBetweenParticleBursts;
        if (cluster[index] != null)
        {
            yield return new WaitForSeconds(particleTimer);
                myParticles.gameObject.transform.position = cluster[index].gameObject.transform.position;
                myParticles.Play();

                //GameObject particleBurst = Instantiate(myParticles.gameObject, cluster[index].gameObject.transform.position, Quaternion.identity);
                //particleBurst.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(disappearDelay);
            cluster[index].GetComponent<SpriteRenderer>().enabled = false;
            PlayDirtSound();
            yield return new WaitForSeconds(((cluster.Length + 2) * timeBetweenParticleBursts) - particleTimer + disappearDelay);
            if (cluster[index] != null)
            {
                Destroy(cluster[index].gameObject);
            }
            GamePiece.destroying = false;
            destroying = false;
        }
    }
    private void PlayDirtSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, dirtSounds.Count);
        AudioSource.PlayClipAtPoint(dirtSounds[randomIndex], Camera.main.transform.position, 0.4f);
    }
}
