using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machete : MonoBehaviour
{
	public MeshCollider col;
	AudioSource hitsound;
	void Start ()
	{
		hitsound = GetComponent<AudioSource>();
		col = GetComponent<MeshCollider>();
		col.enabled = false;
	}
	

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Survivor"))
		{
            ControlAI info = other.GetComponent<ControlAI>();
			hitsound.Play();

			col.enabled = false;
			UIControll.Self.UIBloodOn();
			Debug.Log("Hit");
            
            info.HitCol();
            
		}
	}



	private static Machete _Self;
	public static Machete Self
	{
		get
		{
			if (!_Self)
			{
				_Self = GameObject.FindObjectOfType(typeof(Machete)) as Machete;
			}
			return _Self;
		}
	}
}
