using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
	public AIControl[] Survivor;

	int HelpIndex;
	int HookIndex;

	void Start ()
	{
		if (!IsCheckHook)
			StartCoroutine("CheckHook");
	}
	

	bool IsCheckHook = false;
	IEnumerator CheckHook()
	{
		IsCheckHook = true;
		while (true)
		{
			for(int i=0;i<Survivor.Length;i++)
			{
				if(Survivor[i].GetState() == AISTATE.HOOK)
				{
					HookIndex = i;
					IsCheckHook = false;
					StopCoroutine("CheckHook");
					if (!IsFindHelperIndex)
						StartCoroutine("FindHelperIndex");
				}
			}
			yield return new WaitForSeconds(1);
		}
	}

	bool IsCheckHelper = false;
	IEnumerator CheckHelper()
	{
		IsCheckHelper = true;
		while (true)
		{
			//도와주는애가 살인마한태 쫓기면 다시 도와줄애 찾는다
			if(Survivor[HelpIndex].GetState()==AISTATE.RUN)
			{
				StopCoroutine("CheckHelper");
				IsCheckHelper = false;
				//도와줄애 다시 찾는다
				if (!IsFindHelperIndex)
					StartCoroutine("FindHelperIndex");
			}
			//걸린애가 풀리면 다시 걸린애가 있는지 찾는다
			if(Survivor[HookIndex].GetState()==AISTATE.RUN)
			{
				StopCoroutine("CheckHelper");
				IsCheckHelper = false;

				if (!IsCheckHook)
					StartCoroutine("CheckHook");
			}
			yield return null;
		}
	}

	bool IsFindHelperIndex = false;
	IEnumerator FindHelperIndex()
	{
		IsFindHelperIndex = true;
		while (true)
		{
			for (int i = 0; i < Survivor.Length; i++)
			{
				if (i == HookIndex) continue;

				if (Survivor[i].GetState() == AISTATE.WORK_GENER)
				{
					HelpIndex = i;
					Survivor[HelpIndex].SetState(AISTATE.HOOK_HELP);
					Survivor[HelpIndex].SetHookHelp(Survivor[HookIndex].GetComponent<Transform>());
					IsFindHelperIndex = false;
					StopCoroutine("FindHelperIndex");
					if (!IsCheckHelper)
						StartCoroutine("CheckHelper");
					break;
				}
			}

			yield return new WaitForSeconds(1);
		}
	}
	
}
