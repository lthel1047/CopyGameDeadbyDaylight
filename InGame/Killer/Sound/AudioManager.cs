using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip[] sound;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}




	private static AudioManager _Self;
	public static AudioManager Self
	{
		get
		{
			if (!_Self)
			{
				_Self = GameObject.FindObjectOfType(typeof(AudioManager)) as AudioManager;
			}
			return _Self;
		}
	}
}


static class Sound
{
	public const int Chase = 0;
	public const int GenerExPlo = 7;
	public const int GenerLoop = 8;
	public const int Kick = 9;
	public const int Down = 10;
	public const int Die = 11;
	public const int Scream = 12;
	public const int Walk = 13;
	public const int Result = 14;
}
