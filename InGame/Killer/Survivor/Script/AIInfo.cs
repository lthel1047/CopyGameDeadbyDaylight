using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInfo : MonoBehaviour
{
	bool Hurt = false;
	bool Down = false;
	public bool Hook = false;
	int HookCount = 0;
	AIMove aimove;
	AIControl aiControl;
	
	void Start ()
	{
		aimove = gameObject.GetComponent<AIMove>();
		aiControl = GetComponent<AIControl>();
		Down = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void Hitattack()
	{
		if(Hurt)
		{
			if (Down) return;
			else
			{
				Down = true;
				aiControl.SetState(AISTATE.DOWN);
			}
		}
		else
		{
			Hurt = true;
			aimove.SetAnimation(Hurt);
		}
	}

	public void HookCountUp()
	{
		HookCount++;
	}

	public int GetHookCount()
	{
		return HookCount;
	}

	public bool GetDown()
	{
		return Down;
	}

	public void SetHook()
	{
		Down = false;
		Hook = true;
		HookCountUp();
	}
}
