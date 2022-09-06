using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TemporaryAudioManager : MonoBehaviour
{
	public static TemporaryAudioManager instance;

	[SerializeField]
	GameObject audioSourcePrefab;
	[SerializeField]
	AudioMixerGroup defaultGroup;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning($"Duplicate TemporaryAudioManager! Deleting {name}");
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	public void PlayClipAtPoint(AudioClip clip, Vector3 position, AudioMixerGroup group = null)
	{
		TemporaryAudioSource t = Instantiate(audioSourcePrefab, position, Quaternion.identity).GetComponent<TemporaryAudioSource>();
		t.clip = clip;
		t.group = group ?? defaultGroup;
		t.Play();
	}
}