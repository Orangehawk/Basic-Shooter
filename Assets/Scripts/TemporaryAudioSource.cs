using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TemporaryAudioSource : MonoBehaviour
{
	[SerializeField]
	AudioSource audioSource;

	public AudioClip clip;
	public AudioMixerGroup group;

	public void Play()
	{
		audioSource.outputAudioMixerGroup = group;
		audioSource.PlayOneShot(clip);
	}

	void Update()
	{
		if(!audioSource.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
