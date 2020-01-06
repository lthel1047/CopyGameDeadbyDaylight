using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorInfo : MonoBehaviour
{
	public float Timer = 60f;
	public float Count = 0f;
	public bool Work = true;
	public bool KillerKick = false;
	public bool MatchSurvivor;
	// Use this for initialization
	void Start ()
	{
		MatchSurvivor = false;
	}
	
	

	public bool GetWork()
	{
		return Work;
	}

	public void SetMatchSurvivor(bool match)
	{
		MatchSurvivor = match;
	}

	bool IsWorkStart = false;
	IEnumerator WorkStart()
	{
		if(Count>=Timer)
			StopCoroutine("WorkStart");

		IsWorkStart = true;
		KillerKick = true;
		while (true)
		{
			Count++;
			if(Count>Timer)
			{
				Work = false;
				StopCoroutine("WorkStart");
				IsWorkStart = false;
				Work = false;
				TextCount.Self.count--;
			}

			yield return new WaitForSeconds(1);
		}
	}

	public void AttackGenerator()
	{
		Count -= 10f;
		if (Count < 0f) Timer = 0;
		KillerKick = false;
	}

	 void OnTriggerEnter(Collider other)
	 {
		if (other.gameObject.CompareTag("Survivor") &&
			other.GetComponent<AIControl>().GetState() == AISTATE.WORK_GENER)
		{
			if (!IsWorkStart&&Work)
				StartCoroutine("WorkStart");
		}
	 }

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.CompareTag("Survivor")&&
			other.GetComponent<AIControl>().GetState()== AISTATE.WORK_GENER)
		{
			if(!IsWorkStart && Work)
			StartCoroutine("WorkStart");
		}
	}

	private void OnTriggerExit(Collider other)
	{

		if (other.gameObject.CompareTag("Survivor") &&
			(other.GetComponent<AIControl>().GetState() == AISTATE.RUN||
			other.GetComponent<AIControl>().GetState() == AISTATE.HOOK_HELP))
		{
			StopCoroutine("WorkStart");
			IsWorkStart = false;
			MatchSurvivor = false;
		}
	}
}
