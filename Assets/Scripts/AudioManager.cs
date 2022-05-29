using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] public float masterVolume;
    [SerializeField] Slider masterVol;
    [SerializeField] float musicMultiplier = 0.6f;

    //cached references
    AudioSource myMusic;

    private void Awake()
    {
        int numberOfManagers = FindObjectsOfType<AudioManager>().Length;
        if (numberOfManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSliders();
        myMusic.volume = masterVolume * musicMultiplier;
    }

    public void ResetSliders()
    {
        var sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.name == "Master Volume") { masterVol = slider; }
        }
        masterVol.value = masterVolume;
    }

    private void UpdateSliders()
    {
        masterVolume = masterVol.value;
    }
}
