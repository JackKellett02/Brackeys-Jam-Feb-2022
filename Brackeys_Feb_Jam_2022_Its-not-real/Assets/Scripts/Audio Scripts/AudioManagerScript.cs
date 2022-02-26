using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerScript : MonoBehaviour {
	#region Variables to assign via the unity inspector (SerializeFields).
	[SerializeField]
	[Range(1, 100)]
	private int audioSourcePoolSize = 20;

	[SerializeField]
	private GameObject audioSourcePrefab = null;

	[SerializeField]
	private List<AudioFile> audioFiles = new List<AudioFile>();
	#endregion

	#region Private Variable Declarations.

	private static GameObject musicAudioSource = null;
	private static List<AudioFile> staticSoundEffectFiles;
	private static List<AudioFile> staticMusicFiles;
	private static ObjectPool audioSourcePool;
	#endregion

	#region Private Functions.
	// Start is called before the first frame update
	void Start() {
		//Create the audio source pool.
		audioSourcePool = new ObjectPool(this.gameObject.transform, audioSourcePrefab, audioSourcePoolSize);

		//Create an audio source specifically for music.
		musicAudioSource = Instantiate(audioSourcePrefab, this.gameObject.transform);

		//Create the static audio file list and copy the serializeField values.
		staticSoundEffectFiles = new List<AudioFile>();
		staticMusicFiles = new List<AudioFile>();
		for (int i = 0; i < audioFiles.Count; i++)
		{
			if (audioFiles[i].bMusicClip)
			{
				staticMusicFiles.Add(audioFiles[i]);
			}
			else
			{
				staticSoundEffectFiles.Add(audioFiles[i]);
			}

		}
	}

	// Update is called once per frame
	void Update() {
		//Move the music audio source infront of the main camera.
		Vector3 cameraPos = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position;
		musicAudioSource.transform.position = new Vector3(cameraPos.x, cameraPos.y, 0.0f);
	}
	#endregion

	#region Public Access Functions (Getters and Setters).
	/// <summary>
	/// Player an audio file that has been given the passed in name.
	/// Also can give the audio source a position to play the sound from.
	/// </summary>
	/// <param name="name"></param>
	public static IEnumerator PlaySoundEffect(string name, Vector3 a_position)
	{
		//Find the audio clip with the specified name.
		AudioClip audioClip = null;
		float volume = 0.0f;
		for (int i = 0; i < staticSoundEffectFiles.Count; i++)
		{
			//If the name matches and it's not a music clip.
			if (staticSoundEffectFiles[i].audioClip.name == name)
			{
				audioClip = staticSoundEffectFiles[i].audioClip;
				volume = staticSoundEffectFiles[i].volume;
				break;
			}
		}

		//Get an audio source object.
		GameObject audioSoureGameObject = audioSourcePool.SpawnObject();
		AudioSource audioSource = audioSoureGameObject.GetComponent<AudioSource>();

		//Change the volume to the correctly volume for the clip.
		audioSource.volume = volume;

		//Move it to the specified location.
		audioSoureGameObject.transform.position = a_position;

		//Get the length of the audio clip.
		float clipLength = 0.5f;
		if (audioClip != null)
		{
			//Pass audio clip values to the audio source and play the clip.
			clipLength = audioClip.length;
			audioSource.clip = audioClip;
			audioSource.loop = false;
			audioSource.Play();
		}
		else
		{
			Debug.LogError("Sound effect does not exist in the audio managers sound effect list.");
		}

		//Pause execution for length of the audio.
		yield return new WaitForSeconds(clipLength);

		//Deactivate the audio Source.
		audioSource.Stop();
		audioSoureGameObject.SetActive(false);
	}

	public static void PlayMusicClip(string a_name, bool a_loop)
	{
		//Get the music audio source object.
		AudioSource audioSource = musicAudioSource.GetComponent<AudioSource>();

		//Stop any previous music from playing.
		audioSource.Stop();

		//Find the audio clip with the specified name.
		AudioClip audioClip = null;
		float volume = 0.0f;
		for (int i = 0; i < staticMusicFiles.Count; i++) {
			//If the name matches and it's not a music clip.
			if (staticMusicFiles[i].audioClip.name == a_name) {
				audioClip = staticMusicFiles[i].audioClip;
				volume = staticMusicFiles[i].volume;
				break;
			}
		}

		//Set the music volume to the correct value.
		audioSource.volume = volume;

		//Get the length of the audio clip.
		float clipLength = 0.5f;
		if (audioClip != null) {
			//Pass audio clip values to the audio source and play the clip.
			clipLength = audioClip.length;
			audioSource.clip = audioClip;
			audioSource.loop = a_loop;
			audioSource.Play();
		}
		else
		{
			Debug.LogError("Music clip does not exist in music list in the audio manager.");
		}
	}
	#endregion
}
