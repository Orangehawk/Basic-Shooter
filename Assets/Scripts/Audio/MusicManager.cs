using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [SerializeField]
    AudioSource audioSource;

	bool paused;
    GameManager gameManager;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("Duplicate MusicManager!");
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
		if (gameManager.GetState() == GameManager.State.Playing)
		{
			if (paused)
			{
				audioSource.UnPause();
				paused = false;
			}
			else if (audioSource.isPlaying == false && !paused)
			{
				Debug.Log("Music ended, restarting music");
				audioSource.Play();
				paused = false;
			}
		}
		else
		{
			audioSource.Pause();
			paused = true;
		}
    }
}
