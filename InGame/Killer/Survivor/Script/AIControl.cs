using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
	public GameObject Killer;
	public GameObject[] Generator;
	public GameObject runpoint;

	public float Hookdie = 60f;
	public float HookTimer = 0f;
	NavMeshAgent nav;
	int AI_STATE = AISTATE.FIND_GENER;
	AIMove aimove;

	int generIndex;
	Transform hookhelp=null;
	
	Queue<Transform> RPQueue= new Queue<Transform>();

	void Start ()
	{
		generIndex = 0;
		nav = GetComponent<NavMeshAgent>();
		aimove = GetComponent<AIMove>();
	}
	
	void Update ()
	{
		
		switch(AI_STATE)
		{
			case AISTATE.IDLE:

				break;

			case AISTATE.FIND_GENER:
				nav.SetDestination(FindGenerator());
				aimove.SetAnimation(AIANI.RUN);
				AI_STATE = AISTATE.MOVE_GENER;
				break;

			case AISTATE.MOVE_GENER:
				if(!IsMoveFindKiller)
				StartCoroutine("MoveFindKiller");
				break;

			case AISTATE.WORK_GENER:
				if(!IsWorkFindKiller)
				StartCoroutine("WorkFindKiller");
				break;

			case AISTATE.RUN:
				if (!IsRunawayKiller) 
				StartCoroutine("RunawayKiller");
				break;

			case AISTATE.HIDE:
				aimove.SetAnimation(AIANI.HIDE);
				if(!IsHide)
				StartCoroutine("Hide");
				break;

			case AISTATE.DOWN:
				StopAllCoroutines();
				aimove.SetAnimation(AIANI.DOWN);
				gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
				nav.SetDestination(transform.position);
				Generator[generIndex].SendMessage("SetMatchSurvivor", false);
				break;

			case AISTATE.HOOK:
				aimove.SetAnimation(AIANI.HOOK);
				transform.GetChild(0).transform.localPosition = new Vector3(0, 0.7f, 0);
				if (!IsHookCount)
					StartCoroutine("HookCount");
				break;

			case AISTATE.HOOK_HELP:
				HookHidePos();
				AI_STATE = AISTATE.HOOK_HELP_HIDE;
				break;

			case AISTATE.HOOK_HELP_HIDE:
				if (!IsHelpHide)
					StartCoroutine("HelpHide");
				break;

			case AISTATE.HOOK_HELP_CHECK:
				if (!IsHelpCheck)
					StartCoroutine("HelpCheck");
				break;

			case AISTATE.HOOK_HELP_RUN:
				if (!IsHelpRun)
					StartCoroutine("HelpRun");
				break;
		}

	}



	private void OnTriggerEnter(Collider other)
	{
		switch(AI_STATE)
		{
			case AISTATE.MOVE_GENER:
				if(other.gameObject.CompareTag("Generator")&&
					other.gameObject.name == Generator[generIndex].name)
				{
					StopCoroutine("MoveFindKiller");
					IsMoveFindKiller = false;

					AI_STATE = AISTATE.WORK_GENER;
					aimove.SetAnimation(AIANI.WORK);
				}
				break;
				
		}
	}


	//코루틴 종료는 ontrigger enter에
	bool IsMoveFindKiller = false;
	IEnumerator MoveFindKiller()
	{
		IsMoveFindKiller = true;
		while (true)
		{

			if(KillerDis()<10f)
			{
				StopCoroutine("MoveFindKiller");
				IsMoveFindKiller = false;

				nav.SetDestination(runpoint.transform.position);
				Generator[generIndex].SendMessage("SetMatchSurvivor", false);
				AI_STATE = AISTATE.RUN;
				aimove.SetAnimation(AIANI.RUN);
			}
			yield return null;
		}
	}

	bool IsWorkFindKiller = false;
	IEnumerator WorkFindKiller()
	{
		IsWorkFindKiller = true;
		while (true)
		{
			if(!Generator[generIndex].GetComponent<GeneratorInfo>().Work)
			{
				IsWorkFindKiller = false;
				StopAllCoroutines();
				AI_STATE = AISTATE.FIND_GENER;
			}

			if (KillerDis() < 5f)
			{
				IsWorkFindKiller = false;
				StopCoroutine("WorkFindKiller");

				nav.SetDestination(runpoint.transform.position);
				Generator[generIndex].SendMessage("SetMatchSurvivor", false);
				AI_STATE = AISTATE.RUN;
				aimove.SetAnimation(AIANI.RUN);
			}
			yield return new WaitForSeconds(1);
		}
	}

	bool IsRunawayKiller = false;
	IEnumerator RunawayKiller()
	{
		IsRunawayKiller = true;
		Transform tmp = FindRunPoint();
		while (true)
		{
			if (KillerDis() < 20f)
			{
				if(nav.remainingDistance<=1f)
				{
					tmp = FindRunPoint();
				}
				nav.SetDestination(tmp.position);
				AI_STATE = AISTATE.RUN;
				aimove.SetAnimation(AIANI.RUN);
			}
			else
			{
				IsRunawayKiller = false;
				StopCoroutine("RunawayKiller");
				RPQueue.Clear();
				nav.SetDestination(transform.position);
				AI_STATE = AISTATE.HIDE;
			}
			yield return null;
		}
	}

	bool IsHide = false;
	IEnumerator Hide()
	{
		IsHide = true;
		while (true)
		{
			if (KillerDis() > 20f)
			{
				IsHide = false;
				StopCoroutine("Hide");
				AI_STATE = AISTATE.FIND_GENER;
			}

			if (KillerDis() < 2f)
			{
				IsHide = false;
				StopCoroutine("Hide");
				nav.SetDestination(runpoint.transform.position);
				AI_STATE = AISTATE.RUN;
				aimove.SetAnimation(AIANI.RUN);
			}
			yield return new WaitForSeconds(1);
		}
	}

	bool IsHookCount = false;
	IEnumerator HookCount()
	{
		IsHookCount = true;
		while (true)
		{
			HookTimer += Time.deltaTime;
			if(HookTimer > Hookdie)
			{
				AI_STATE = AISTATE.DIE;
				StopCoroutine("HookCount");
				IsHookCount = true;
				gameObject.SetActive(false);
			}

			if(AI_STATE == AISTATE.RUN)
			{
				IsRunawayKiller = false;
				transform.parent.GetComponent<HookInfo>().Enable = true;
				transform.SetParent(null);
				transform.GetComponent<AIInfo>().Hook = false;
				HookTimer = 0f;
				transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
				StopCoroutine("HookCount");
				IsHookCount = true;
			}

			yield return null;
		}
	}

	float KillerDis()
	{
		return Vector3.Distance(Killer.transform.position, transform.position);
	}

	Vector3 FindGenerator()
	{
		generIndex = Random.Range(0, Generator.Length);
		Vector3 pos;
		while(true)
		{
			if (Generator[generIndex].GetComponent<GeneratorInfo>().Work)
			{
				break;
			}
			else
			{
				generIndex = Random.Range(0, Generator.Length);
			}
		}

		pos = Generator[generIndex].transform.position;

		for(int i= generIndex; i<Generator.Length;i++)
		{
			if (Vector3.Distance(transform.position, pos) >
				Vector3.Distance(transform.position, Generator[i].transform.position)&&
				Generator[i].GetComponent<GeneratorInfo>().GetWork()&&
				!Generator[i].GetComponent<GeneratorInfo>().MatchSurvivor
				)
			{
				generIndex = i;
				pos = Generator[i].transform.position;
			}
		}
		
		Generator[generIndex].GetComponent<GeneratorInfo>().MatchSurvivor = true;
		return pos;
	}


	public void SetState(int State)
	{
		AI_STATE = State;
	}

	public int GetState()
	{
		return AI_STATE;
	}

	public Transform FindRunPoint()
	{
		int length = runpoint.transform.childCount;
		Transform min = runpoint.transform.GetChild(100);

		for (int i=0;i< length;i++)
		{
			Transform child = runpoint.transform.GetChild(i);

			if(Vector3.Distance(transform.position,min.position)>
				Vector3.Distance(transform.position,child.position)&&
				min!=child)
			{
				if (RPQueue.Contains(child)) continue;
				else
				{
					min = child;
				}
			}
		}
		
		if(RPQueue.Count>10)
		{
			RPQueue.Dequeue();
			RPQueue.Enqueue(min);
		}
		else
			RPQueue.Enqueue(min);

		return min;
	}

	public void SetHookHelp(Transform data)
	{
		hookhelp = data;
	}

	public void HookHidePos()
	{
		int length = runpoint.transform.childCount;
		Transform min = runpoint.transform.GetChild(0);

		for (int i = 0; i < length; i++)
		{
			Transform child = runpoint.transform.GetChild(i);

			if (Vector3.Distance(hookhelp.position, child.position)>15f &&
				Vector3.Distance(hookhelp.position, child.position) < 20f&&
				Vector3.Distance(transform.position, child.position) <
				Vector3.Distance(transform.position, min.position) )
			{
				min = child;
			}
		}

		aimove.SetAnimation(AIANI.RUN);
		nav.SetDestination(min.position);
	}

	bool IsHelpHide = false;
	IEnumerator HelpHide()
	{
		IsHelpHide = true;
		while (true)
		{
			yield return new WaitForSeconds(1);
			if (KillerDis()<5f)
			{
				IsHelpHide = false;
				StopCoroutine("HelpHide");
				AI_STATE = AISTATE.RUN;
			}

			if (nav.remainingDistance < 1.0f)
			{

				AI_STATE = AISTATE.HOOK_HELP_CHECK;
				aimove.SetAnimation(AIANI.HIDE);
				IsHelpHide = false;
				StopCoroutine("HelpHide");
			}

			
		}
	}

	bool IsHelpCheck = false;
	IEnumerator HelpCheck()
	{
		IsHelpCheck = true;
		while (true)
		{
			yield return new WaitForSeconds(1);
			if(KillerDis()>10f &&
				Vector3.Distance(hookhelp.position,Killer.transform.position)>10f)
			{
				nav.SetDestination(hookhelp.position);
				aimove.SetAnimation(AIANI.RUN);
				IsHelpCheck = false;
				StopCoroutine("HelpCheck");
				AI_STATE = AISTATE.HOOK_HELP_RUN;
			}
		}
	}

	bool IsHelpRun = false;
	IEnumerator HelpRun()
	{
		IsHelpRun = true;
		while(true)
		{
			yield return new WaitForSeconds(1);
			if (nav.remainingDistance < 1.0f)
			{
				AI_STATE = AISTATE.RUN;
				hookhelp.GetComponent<AIControl>().SetState(AISTATE.RUN);

				IsHelpRun = false;
				StopCoroutine("HelpRun");
			}

		}
	}
}

static class AISTATE
{
	public const int IDLE = 0;
	public const int MOVE_GENER = 1;
	public const int RUN = 2;
	public const int FIND_GENER = 3;
	public const int WORK_GENER = 4;
	public const int HIDE = 5;
	public const int DOWN = 6;
	public const int HOOK = 7;
	public const int HOOK_HELP = 8;
	public const int HEAL = 9;
	public const int HEAL_WAIT = 10;
	public const int HOOK_HELP_HIDE = 11;
	public const int HOOK_HELP_CHECK = 12;
	public const int HOOK_HELP_RUN = 13;

	public const int DIE = 100;
}