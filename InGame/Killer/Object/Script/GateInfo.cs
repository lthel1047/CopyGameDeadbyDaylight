using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateInfo : MonoBehaviour
{
	public GameObject door;
	public Transform RunPoint;
	public bool Match = false;
	public bool OpenDoorCheck = false;
	public int OpenCount = 10;
	public float WorkTime = 0f;
	public GameObject Circle;
	public int CircleTimer = 3;
	public GameObject []GateLight;
	public AudioSource SirenSound;
	AudioSource doorsound;
	// Use this for initialization
	void Start ()
	{
		doorsound = GetComponent<AudioSource>();
		for (int i=0;i<GateLight.Length;i++)
		{
			GateLight[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.L))
			StartCoroutine("OpenDoor");
	}

	IEnumerator StartExitWork()
	{
		while(true)
		{
			if(WorkTime>OpenCount)
			{
				SirenSound.Play();
				doorsound.Play();
				StartCoroutine("OpenDoor");
				StartCoroutine("StartTimer");
				for (int i = 0; i < GateLight.Length; i++)
				{
					GateLight[i].SetActive(true);
					GateLight[i].GetComponent<Light>().enabled = true;
				}
				OpenDoorCheck = true;
				break;
			}

			WorkTime += Time.deltaTime;
			yield return null;
		}
	}


	IEnumerator OpenDoor()
	{
		Transform tr = door.GetComponent<Transform>();
		float max = 3;
		float up = 0.05f;
		float sum = 0f;

		while(true)
		{
			if (sum > max)
				break;
			tr.position -= new Vector3(up, 0, 0);
			sum += up;
			yield return null;
		}
	}

	bool IsStartTimer = false;
	IEnumerator StartTimer()
	{
		IsStartTimer = true;
		float timer = 0f;
		Circle.SetActive(true);
		while (true)
		{
			if (timer > CircleTimer)
			{
				Circle.SetActive(false);
				break;
			}
			timer += Time.deltaTime;
			yield return null;
		}

		IsStartTimer = false;
	}
}
