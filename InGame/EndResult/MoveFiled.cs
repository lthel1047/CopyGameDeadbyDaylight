using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFiled : MonoBehaviour {

	public Transform pos;

	void Start ()
	{
		
	}
	
	void Update ()
	{
		transform.position += new Vector3(0, 0, 0.02f);
		if (transform.position.z > 40)
			transform.position = pos.position - new Vector3(0, 0, 40f);
	}
}
