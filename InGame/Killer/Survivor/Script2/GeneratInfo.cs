using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratInfo : MonoBehaviour
{
	public float Timer = 60f;
	public float Count = 0f;
	public bool Work = true;
	public bool KillerKick = false;
	public GameObject GenerLight;
	public GameObject Circle;
	public int CircleTimer = 5;
	AudioSource source;
	public GameObject loopobj;
	GameObject matchSurvivor;
	public GameObject MatchSurvivor
	{
		get { return matchSurvivor; }
		set { matchSurvivor = value; }
	}
	// Use this for initialization
	void Start()
	{
		source = GetComponent<AudioSource>();
		Circle.SetActive(false);
		MatchSurvivor = null;
	}
	

	public bool GetWork()
	{
		return Work;
	}

	bool IsWorkStart = false;
	IEnumerator WorkStart()
	{
		if (Count >= Timer)
			StopCoroutine("WorkStart");
		loopobj.GetComponent<AudioSource>().Play();
		loopobj.GetComponent<AudioSource>().volume = 0.1f;
		GenerLight.SetActive(true);
		GenerLight.GetComponent<Light>().enabled = true;
		GenerLight.GetComponent<Light>().intensity = 1f;
		IsWorkStart = true;
		KillerKick = true;
		while (true)
		{
			Count++;
			if (Count > Timer)
			{
				loopobj.GetComponent<AudioSource>().volume = 0.5f;

				GenerLight.GetComponent<Light>().intensity = 5f;
				Work = false;
				StopCoroutine("WorkStart");
				IsWorkStart = false;
                KillerKick = false;
                Work = false;
                TextCount.Self.CountMinus();
			}

			int rand = Random.Range(0, 100);
			if(rand<10)
			{
				if (!IsStartTimer)
				{
					Count -= 3;
					if (Count < 0) Count = 0;
					StartCoroutine("StartTimer");
				}
			}
			yield return new WaitForSeconds(1);
		}
	}

	bool IsStartTimer = false;
	IEnumerator StartTimer()
	{
		IsStartTimer = true;
		source.clip = AudioManager.Self.sound[Sound.GenerExPlo];
		source.Play();
		float timer = 0f;
		Circle.SetActive(true);
		while(true)
		{
			if(timer > CircleTimer)
			{
				Circle.SetActive(false);
				break;
			}
			timer += Time.deltaTime;
			yield return null;
		}

		IsStartTimer = false;
	}

	public void AttackGenerator()
	{
		Count -= 10f;
		if (Count < 0f) Timer = 0;
		KillerKick = false;
	}


}