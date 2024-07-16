using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundFilesAttributes
{
	//attributes or sound files
	public string name;
	//source
	public AudioClip clip;
	public AudioMixerGroup mixer;
	//volume of range
	[Range(0f, 1f)]
	public float volume = 1;
	//pitch range 
	[Range(-3f, 3f)]
	public float pitch = 1;
	//bool for looping sound
	public bool loop = false;
    public bool awake = false;
    [HideInInspector]
	public AudioSource source;

}
