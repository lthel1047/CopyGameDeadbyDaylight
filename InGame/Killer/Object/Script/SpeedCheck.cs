using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedCheck : MonoBehaviour {

	Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		text.text = "Speed : " + PlayerMovementKiller.Self.RunSPD;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void UpdateSPDText()
	{
		text.text = "Speed : " + PlayerMovementKiller.Self.RunSPD;
	}
}
