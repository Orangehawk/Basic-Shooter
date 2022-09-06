using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundOptions : MonoBehaviour
{
    [SerializeField]
    AudioMixer audioMixer;

    [SerializeField]
    Slider masterSlider;
    [SerializeField]
    Slider effectsSlider;
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    Slider footstepsSlider;
    [SerializeField]
    Slider voiceSlider;

    // Start is called before the first frame update
    void Start()
    {
        audioMixer.GetFloat("Master", out float masterVolume);
        audioMixer.GetFloat("Effects", out float effectsVolume);
        audioMixer.GetFloat("Music", out float musicVolume);
        audioMixer.GetFloat("Footsteps", out float footstepsVolume);
        audioMixer.GetFloat("Voice", out float voiceVolume);

        masterSlider.value = VolumeToSliderValue(masterVolume);
        effectsSlider.value = VolumeToSliderValue(effectsVolume);
        musicSlider.value = VolumeToSliderValue(musicVolume);
        footstepsSlider.value = VolumeToSliderValue(footstepsVolume);
        voiceSlider.value = VolumeToSliderValue(voiceVolume);
    }

    float VolumeFromSlider(Slider slider)
	{
        return slider.value - 80;
	}

    float VolumeToSliderValue(float volume)
	{
        return volume + 80;
	}

    // Update is called once per frame
    void Update()
    {
        audioMixer.SetFloat("Master", VolumeFromSlider(masterSlider));
        audioMixer.SetFloat("Effects", VolumeFromSlider(effectsSlider));
        audioMixer.SetFloat("Music", VolumeFromSlider(musicSlider));
        audioMixer.SetFloat("Footsteps", VolumeFromSlider(footstepsSlider));
        audioMixer.SetFloat("Voice", VolumeFromSlider(voiceSlider));
    }
}
