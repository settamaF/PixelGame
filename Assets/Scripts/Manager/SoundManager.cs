//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;

//******************************************************************************

public enum eSoundEvent
{
	Event1,
	Event2,
	Event3
}

[System.Serializable]
public class SoundEvent
{
	public AudioClip Audio;
	public eSoundEvent Event;
}


public class SoundManager : MonoBehaviour 
{
	#region Script Parameters
	public List<SoundEvent> SoundEvents;
	#endregion

	#region Static
	public static SoundManager Get { get; private set; }
	#endregion

	#region Properties

	#endregion

	#region Fields
		// Const -------------------------------------------------------------------
	
		// Static ------------------------------------------------------------------

		// Private -----------------------------------------------------------------

	#endregion

	#region Unity Methods
	void Awake()
	{
		if (Get != null && Get != this)
		{
			Destroy(gameObject);
			return;
		}
		if (Get == null)
			Get = this;
		if (transform.parent == null)
			DontDestroyOnLoad(gameObject);
	}
	#endregion

	#region Methods
	public AudioClip GetSoundEvent(eSoundEvent soundEvent)
    {
		foreach (var sound in SoundEvents)
		{
			if (sound.Event == soundEvent)
				return sound.Audio;
		}
		return null;
	}

	public void PlaySound(AudioSource source, eSoundEvent soundEvent)
    {
		if (source == null)
			return;
		AudioClip clip = GetSoundEvent(soundEvent);
		if (clip == null)
		{
			Debug.LogError("No sound found for event: " + soundEvent.ToString());
			return;
		}
		source.clip = clip;
		source.Play();
	}

	#endregion

	#region Implementation

	#endregion
}
