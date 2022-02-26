using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A custom class that stores an audio clip and information about it to be
/// used by the audio manager script in playing and sorting audio.
/// </summary>
[System.Serializable]
public class AudioFile {
	#region Class Variables
	public AudioClip audioClip = null;
	public bool bMusicClip = false;

	[Range(0.0f, 1.0f)]
	public float volume = 1.0f;
	#endregion

	#region Class Public Functions.
	//Constructor.
	public AudioFile() {

	}
	#endregion

	#region Class Private Functions.

	#endregion
}
