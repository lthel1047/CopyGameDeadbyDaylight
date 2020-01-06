using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ControlAI : MonoBehaviour
{

	public GameObject Killer;
	public GameObject []Generator;
	public GameObject resetpoint;
    public StateUI ui;
	public HookingCheck findhelper;
	public GateInfo[] Gate;
    public int DieTime = 10;
    public int HookCount = 0;
	public int HealTime = 5;
	public int DownTime = 10;
	int GeneratorIndex;
	NavMeshAgent nav;
	Animator Ani;

	State state;
	object Lock = new object();

	public bool Exit = false;
    public bool Hurt = false;
    public bool Down = false;
    public bool Hook = false;
    public bool Getting = false;
    public bool Die = false;
	public bool Helper = false;
	public bool Healer = false;
	Transform helpTarget = null;
	Transform healTarget = null;
	Transform healer = null;

	AudioSource sound;
	void Start ()
	{
		sound = GetComponent<AudioSource>();
		nav = GetComponent<NavMeshAgent>();
		Ani = transform.GetChild(0).GetComponent<Animator>();
		//첫 시작은 발전기부터 찾는다.
		state = State.FindGenerator;
		StartCoroutine("FSM");
	}
	


	//상태가 돌아가는 주 코루틴
	IEnumerator FSM()
	{
		while(true)
		{
			yield return StartCoroutine(state.ToString());
            if(Die)
            {
                break;
            }
		}

        gameObject.SetActive(false);
        Debug.Log(gameObject.name + ": FMS END");
	}
	

	IEnumerator FindGenerator()
	{
		Debug.Log(gameObject.name + ": FindGenerator In");
		while (true)
		{
			if(Exit)
			{
				state = State.ExitState;
				break;
			}

			if (Hurt)
			{
				if (findhelper.FindHealer(transform))
				{
					state = State.HealerWait;
					break;
				}
			}
			//발전기를 찾으면 상태를 바꾸고 빠져나간다.
			if (FindGener())
			{
				state = State.MoveGenerator;
				break;
			}

			yield return null;
		}

		Debug.Log(gameObject.name + ": FindGenerator Out");
	}

	//발전기에 이동 상태
	bool moveflag = false;
	IEnumerator MoveGenerator()
	{
		GeneratInfo info = Generator[GeneratorIndex].GetComponent<GeneratInfo>();
		Debug.Log(gameObject.name + ": MoveGenerator In");
		
		while (true)
		{
			if (Exit)
			{
				info.MatchSurvivor = null;
				state = State.ExitState;
				break;
			}

			//작업 이동중 hook상태인 ai 도와주러가기
			if (Helper)
			{
				info.MatchSurvivor = null;
				state = State.HelpTargetMove;
				break;
			}

			if (Healer)
			{
				info.MatchSurvivor = null;
				state = State.HealTargetMove;
				break;
			}
			

			//이동중에 킬러를 만나면 발전기 초기화 해줘야됨
			if (KillerDis(20f))
			{
				info.MatchSurvivor = null;
				state = State.RunAway;
				break;
			}

			//발전기에 다가가면 발전기 일을 한다
			if (moveflag)
			{
				moveflag = false;
				SetAniState(STATE.Work);
				state = State.WorkGenerator;
				break;
			}
			yield return null;
		}
		Debug.Log(gameObject.name + ": MoveGenerator Out");
	}

	//발전기를 돌리는 상태
	IEnumerator WorkGenerator()
	{
        Debug.Log(gameObject.name + ": WorkGenerator In");
        GeneratInfo info = Generator[GeneratorIndex].GetComponent<GeneratInfo>();
        
        while (true)
		{
			if (Exit)
			{
				info.MatchSurvivor = null;
				info.StopCoroutine("WorkStart");
				state = State.ExitState;
				break;
			}
			//킬러가 가까이 오면 도망 간다.
			//발전기 매치서바이버 초기화,발전기 코루틴 끄기
			if (KillerDis(5f))
            {
                info.MatchSurvivor = null;
                info.StopCoroutine("WorkStart");
                SetAniState(STATE.Run);
				state = State.RunAway;
				break;
			}

			//작업중 hook상태인 ai 도와주러가기
			if(Helper)
			{
				info.MatchSurvivor = null;
				info.StopCoroutine("WorkStart");
				state = State.HelpTargetMove;
				break;
			}
			//작업중 힐러
			if(Healer)
			{
				info.MatchSurvivor = null;
				info.StopCoroutine("WorkStart");
				state = State.HealTargetMove;
				break;
			}

            if (!info.Work)
            {
                state = State.FindGenerator;
                break;
            }

			yield return null;
		}

        Debug.Log(gameObject.name + ": WorkGenerator Out");
    }

	//도망가는 상태
	IEnumerator RunAway()
	{
        Debug.Log(gameObject.name + ": RunAway In");
		FindRunPoint();
		while (true)
        {
			SetAniState(STATE.Run);
            yield return null;
			if(nav.remainingDistance < 1.0f)
			{
				FindRunPoint();
			}

            if(Down)
            {
                state = State.HitDown;
                break;
            }

			//도와주는 사람이었다면 다시 도와주러 가야된다.
			if(!KillerDis(30f))
            {
				yield return new WaitForSeconds(0.1f);

				if (Helper||Healer)
				{
					if(Helper)
						state = State.HelpTargetMove;
					else if(Healer)
						state = State.HealTargetMove;
				}
				else
				{
					if (Exit)
					{
						state = State.ExitState;
					}
					else
					{
						state = State.FindGenerator;
					}
				}

				break;
			}
		}
        Debug.Log(gameObject.name + ": RunAway Out");
    }

    //다운됬을때
    IEnumerator HitDown()
    {
		sound.clip = AudioManager.Self.sound[Sound.Down];
		sound.volume = 1f;
		sound.Play();
		Debug.Log(gameObject.name + ": Down In");
		float timer = DownTime;
        nav.speed = 0.3f;
        FindRunPoint();

		ui.slider.gameObject.SetActive(true);
		ui.UpdateSlider(timer, DownTime);
		while (true)
        {
			if(timer<=0)
			{
				Down = false;
				ui.ChangeUI(UIIMG.Hurt);
				ui.slider.gameObject.SetActive(false);
				nav.speed = 3.0f;
				SetAniState(STATE.Idle);

				if (Exit)
				{
					state = State.ExitState;
				}
				else
				{
					state = State.FindGenerator;
				}
				break;
			}

            if (nav.remainingDistance < 1.0f)
            {
                FindRunPoint();
            }

            if(Getting)
			{
				nav.destination = transform.position;
				nav.baseOffset = 1.3f;
				nav.radius = 0f;
				nav.speed = 3f;
				nav.isStopped = true;
                transform.GetComponent<CapsuleCollider>().enabled = false;
                state = State.KillerGetting;
				ui.slider.gameObject.SetActive(false);
				break;
			}

			timer -= Time.deltaTime;
			ui.UpdateSlider(timer, DownTime);

			yield return null;
        }

        Debug.Log(gameObject.name + ": Down Out");
    }

	//킬러가 자신을 엎었을때 상태
    IEnumerator KillerGetting()
    {
        Debug.Log(gameObject.name + ": KillerGetting In");
        while(true)
        {
            if (Hook)
			{
				nav.baseOffset = 0f;
				transform.GetChild(0).localPosition = new Vector3(0, 0.5f, 0);
                transform.GetComponent<CapsuleCollider>().enabled = true;
                SetAniState(STATE.Hook);
                state = State.Hooking;
                HookCount++;
                break;
            }

            yield return null;
        }
        Debug.Log(gameObject.name + ": KillerGetting Out");
    }

	//갈고리에 걸렸을때 상태
    IEnumerator Hooking()
	{
		sound.clip = AudioManager.Self.sound[Sound.Scream];
		sound.volume = 1f;
		sound.Play();

		float timer = DieTime;
        Debug.Log(gameObject.name + ": Hooking");
        ui.ChangeUI(UIIMG.Hook);
        ui.slider.gameObject.SetActive(true);
        ui.UpdateSlider(DieTime,DieTime);

		//살려줄 상태 찾아달라고 요청하기
		if(HookCount < 3)
			findhelper.FindHelper(transform);

		while (true)
        {
            if(timer<=0f||HookCount==3)
			{
				if (!sound.isPlaying)
				{
					sound.clip = AudioManager.Self.sound[Sound.Scream];
					sound.volume = 1f;
					sound.Play();
				}
				HookingCheck.Self.EndCountUp("Die");
				Die = true;
                ui.ChangeUI(UIIMG.Die);
                ui.slider.gameObject.SetActive(false);
                state = State.DieParticle;
                break;
            }
            
            //후크상태에서 벗어 나면
            if(!Hook)
            {
                transform.GetChild(0).localPosition = new Vector3(0,0, 0);
                nav.radius = 0.05f;
                nav.isStopped = false;

                Down = false;
                Getting = false;

				transform.SetParent(null);

				ui.ChangeUI(UIIMG.Hurt);
				SetAniState(STATE.Run);
				ui.slider.gameObject.SetActive(false);
				if (Exit)
				{
					state = State.ExitState;
				}
				else
				{
					state = State.FindGenerator;
				}
				break;
            }
            timer -= Time.deltaTime;
            ui.UpdateSlider(timer,DieTime);
            yield return null;
        }
        Debug.Log(gameObject.name + ": Hooking");
    }

	IEnumerator HelpTargetMove()
	{
		ControlAI info = helpTarget.GetComponent<ControlAI>();
		Debug.Log(gameObject.name + ": HelpTargetMove In");
		SetAniState(STATE.Run);
		nav.SetDestination(helpTarget.position);

		yield return new WaitForSeconds(0.5f);

		while (true)
		{
			//구했을때 초기화 
			if(nav.remainingDistance<0.3f)
			{
				info.Hook = false;
				if (Exit)
				{
					state = State.ExitState;
				}
				else
				{
					state = State.FindGenerator;
				}
				Helper = false;
				helpTarget = null;
				break;
			}
			//살려주려고 타겟에게 접근중 킬러가 가까이오면 도망간다
			if (KillerDis(10f))
			{
				state = State.RunAway;
				break;
			}
			//도와주려는 타겟을 다른 사람이 구했을때, 죽었을때
			//초기화
			if (!info.Hook || info.Die)
			{
				Helper = false;
				helpTarget = null;
				if (Exit)
				{
					state = State.ExitState;
				}
				else
				{
					state = State.FindGenerator;
				}
				break;
			}
			yield return null;
		}

		Debug.Log(gameObject.name + ": HelpTargetMove Out");
	}

	IEnumerator HealerWait()
	{
		Debug.Log(gameObject.name + ": HealerWait In");
		

		FindRunPoint();
		yield return new WaitForSeconds(0.5f);

		while (true)
		{
			if(nav.remainingDistance <0.1f)
			{
				nav.destination = transform.position;
				SetAniState(STATE.Work);
				break;
			}
			yield return null;
		}

		if (findhelper.SetHealer(transform))
		{
			while (true)
			{
				if (healer == null&& !findhelper.SetHealer(transform))
				{
					SetAniState(STATE.Run);
					if (Exit)
					{
						state = State.ExitState;
					}
					else
					{
						state = State.FindGenerator;
					}
					break;
				}
				
				if (KillerDis(5f))
				{
					healer.GetComponent<ControlAI>().Healer = false;
					SetAniState(STATE.Run);
					state = State.RunAway;
					break;
				}

				if (!Hurt)
				{
					ui.ChangeUI(UIIMG.Health);
					if (Exit)
					{
						state = State.ExitState;
					}
					else
					{
						state = State.FindGenerator;
					}
					Ani.SetBool("Hurt", Hurt);
					break;
				}
				yield return new WaitForSeconds(0.1f);
			}
		}
		else
		{
			if (Exit)
			{
				state = State.ExitState;
			}
			else
			{
				state = State.FindGenerator;
			}
		}
		Debug.Log(gameObject.name + ": HealerWait Out");
	}


	IEnumerator HealTargetMove()
	{
		Debug.Log(gameObject.name + ": HealTargetMove In");
		ControlAI info = healTarget.GetComponent<ControlAI>();
		nav.SetDestination(healTarget.position);
		SetAniState(STATE.Run);
		yield return new WaitForSeconds(0.5f);

		bool move = true;

		while(true)
		{
			if(KillerDis(10f))
			{
				info.healer = null;
				state = State.RunAway;
				Healer = false;
				move = false;
				break;
			}

			if(!Healer)
			{
				move = false;
				if (Exit)
				{
					state = State.ExitState;
				}
				else
				{
					state = State.FindGenerator;
				}
				break;
			}

			if (nav.remainingDistance < 0.5f)
			{
				nav.destination = transform.position;
				SetAniState(STATE.Heal);
				break;
			}
			yield return null;
		}

		float Timer = 0;
		while(move)
		{
			if (!Healer)
			{
				move = false;
				state = State.RunAway;
				break;
			}

			if (Timer>HealTime)
			{
				Healer = false;
				info.Hurt = false;
				if (Exit)
				{
					state = State.ExitState;
				}
				else
				{
					state = State.FindGenerator;
				}
				break;
			}
			Timer += Time.deltaTime;
			yield return null;
		}


		Debug.Log(gameObject.name + ": HealTargetMove Out");
	}
	/// <summary>
	/// //////////////////EXIT STATE
	/// </summary>
	public bool Exit_Work = false;
	public int GateIndex;

	IEnumerator ExitState()
	{
		Debug.Log(gameObject.name + ": ExitState In");
		nav.speed = 3.5f;
		yield return new WaitForSeconds(0.1f);

		

		while(true)
		{
			lock (Lock)
			{
				Exit_Work = FindGate();
			}

			yield return null;
			if (Exit_Work)
			{
				state = State.ExitWorkMove;
				break;
			}
			else
			{
				if(FindOpenGate())
				{
					state = State.ExitRun;
				}
				else
				{
					state = State.Angler;
				}
				break;
			}
		}

		Debug.Log(gameObject.name + ": ExitState Out");
	}

	IEnumerator Angler()
	{
		Debug.Log(gameObject.name + ": Angler In");
		bool flag = true;
		nav.SetDestination(Killer.transform.position);
		SetAniState(STATE.Run);
		yield return new WaitForSeconds(0.2f);

		while(true)
		{
			if (FindOpenGate())
			{
				SetAniState(STATE.Run);
				state = State.ExitRun;
				break;
			}

			if (nav.remainingDistance < 8f && flag)
			{
				flag = false;
				nav.SetDestination(transform.position);
				SetAniState(STATE.Angler);
				yield return new WaitForSeconds(0.2f);
			}

			if(!flag && KillerDis(3f))
			{
				SetAniState(STATE.Run);
				state = State.RunAway;
				break;
			}

			if(!flag && !KillerDis(20f))
			{
				SetAniState(STATE.Run);
				state = State.ExitState;
				break;
			}

			yield return null; ;
		}


		Debug.Log(gameObject.name + ": Angler Out");
	}

	IEnumerator ExitWorkMove()
	{
		Debug.Log(gameObject.name + ": ExitWorkMove In");
		moveflag = false;
		SetAniState(STATE.Run);
		nav.SetDestination(Gate[GateIndex].GetComponent<Transform>().position);
		while (true)
		{
			if(KillerDis(5f))
			{
				Exit_Work = false;
				Gate[GateIndex].Match = false;
				state = State.RunAway;
				break;
			}

			if(moveflag)
			{
				state = State.ExitWork;
				break;
			}

			yield return null;
		}


		Debug.Log(gameObject.name + "ExitWorkMove Out");
	}

	IEnumerator ExitWork()
	{
		Debug.Log(gameObject.name + "ExitWork In");

		SetAniState(STATE.Work);
		while (true)
		{
			if (KillerDis(5f))
			{
				Exit_Work = false;
				Gate[GateIndex].Match = false;
				Gate[GateIndex].StopCoroutine("StartExitWork");
				state = State.RunAway;
				break;
			}

			if(Gate[GateIndex].OpenDoorCheck)
			{
				state = State.ExitRun;
				break;
			}
			yield return null;
		}


		Debug.Log(gameObject.name + "ExitWork Out");
	}

	IEnumerator ExitRun()
	{
		Debug.Log("ExitRun In");
		SetAniState(STATE.Run);
		nav.SetDestination(Gate[GateIndex].RunPoint.position);

		while(true)
		{
			yield return null;
		}
		
		Debug.Log("ExitRun Out");
	}
	/// <summary>
	/// /////////////////////
	/// </summary>
	/// 

	private void OnTriggerEnter(Collider other)
	{
		
		switch(state)
		{
			case State.MoveGenerator:
				if (other.CompareTag("Generator")&&
					Generator[GeneratorIndex]==other.gameObject)
				{
					other.GetComponent<GeneratInfo>().StartCoroutine("WorkStart");
					moveflag = true;
					nav.SetDestination(transform.position);
				}
				break;

			case State.ExitWorkMove:
				if(other.CompareTag("Gate"))
				{
					other.GetComponent<GateInfo>().StartCoroutine("StartExitWork");
					moveflag = true;
					nav.SetDestination(transform.position);
				}
				break;

			case State.ExitRun:
				if (other.CompareTag("ExitPoint"))
				{
					HookingCheck.Self.EndCountUp("Exit");
					ui.ChangeUI(UIIMG.Exit);
					gameObject.SetActive(false);
				}

				break;
		}
	}

	/////////////////////////


	void FindRunPoint()
	{
		Vector3 pos = transform.position + new Vector3(0, 1f, 0);
		Queue<Vector3> queue = new Queue<Vector3>();
		int wallCount = 0;

		for (int i = 0; i < 360; i += 10)
		{
			RaycastHit hit;
			Vector3 nor = Quaternion.Euler(0, i, 0) * transform.forward;
			Debug.DrawRay(pos, nor * 5, Color.green);
			if(Physics.Raycast(pos,nor,out hit,5f))
			{
				if (hit.transform.CompareTag("OutWall"))
					wallCount++;
				continue;
			}
			else
			{
				queue.Enqueue(transform.position + nor * 5f);
			}
		}

		Vector3 max = queue.Dequeue();
		//if (wallCount > 7)
		//{
		//    Vector3 reset = resetpoint.transform.position - transform.position;
		//    Debug.DrawRay(pos, reset.normalized * 5f, Color.red);
		//    max = transform.position + reset.normalized * 5f;
		//}
		while (queue.Count != 0)
		{
			Vector3 tmp = queue.Dequeue();
			if (Vector3.Distance(Killer.transform.position, max) <
				Vector3.Distance(Killer.transform.position, tmp))
			{
				max = tmp;
			}
		}

		nav.SetDestination(max);
	}

	//킬러 거리 계산 넣은 거리 보다 가까우면 트루 리턴
	bool KillerDis(float _dis)
	{
		if(Vector3.Distance(Killer.transform.position,transform.position)
			<_dis)
		{
			return true;
		}
		return false;
	}

	//킬러 거리 계산 넣은 거리 보다 가까우면 트루 리턴
	bool KillerDis(Vector3 pos, float _dis)
	{
		if (Vector3.Distance(Killer.transform.position, pos)
			< _dis)
		{
			return true;
		}
		return false;
	}

	//애니메이션 셋팅
	void SetAniState(int _state)
	{
		Ani.SetInteger("ANI_STATE", _state);
	}

    public string GetState()
    {
        return state.ToString();
    }
	

	bool FindGener()
	{
		int count = 0;
		lock (Lock)
		{
			while (true)
			{
				int i = Random.Range(0, Generator.Length);
				GeneratInfo info = Generator[i].GetComponent<GeneratInfo>();
				if (info.Work && info.MatchSurvivor == null)
				{
					SetAniState(STATE.Run);
					GeneratorIndex = i;
					nav.destination = info.GetComponent<Transform>().position;
					info.MatchSurvivor = gameObject;
					Debug.Log(info.MatchSurvivor.name);
					break;
				}
				//마지막에 발전기가 몇개 안남았을때를 대비한다.
				count++;
				if (count > 10)
					return false;
			}
		}
		return true;
	}

    //맞았을때 실행되는 함수
    public void HitCol()
    {
        //다운 상태가 되기전에 실행되던 코루틴을 종료해줘야 한다.
        if (Hurt)
        {
            Down = true;
            if (!Hook)
            {
                SetAniState(STATE.Down);
                ui.ChangeUI(UIIMG.Down);
            }
        }
        else
        {
            ui.ChangeUI(UIIMG.Hurt);
            Hurt = true;
            Ani.SetBool("Hurt", Hurt);
        }
    }

	public void SetHealer(Transform heal)
	{
		healer = heal;
	}
	public void SetHelpTarget(Transform target)
	{
		helpTarget = target;
	}

	public void SetHealTarget(Transform target)
	{
		healTarget = target;
	}

	public Transform GetHealer()
	{
		return healer;
	}

	
	public bool FindGate()
	{

		for(int i=0;i<Gate.Length;i++)
		{
			if(!Gate[i].Match && !Gate[i].OpenDoorCheck)
			{
				GateIndex = i;
				Gate[i].Match = true;
				return true;
			}
		}

		return false;
	}

	public bool FindOpenGate()
	{
		int count = 0;
		for (int i = 0; i < Gate.Length; i++)
		{
			if (Gate[i].OpenDoorCheck)
			{
				GateIndex = i;
				count++;
			}
		}

		switch(count)
		{
			case 1:
				return true;

			case 2:
				if(Vector3.Distance(transform.position,Gate[0].GetComponent<Transform>().position)<
					Vector3.Distance(transform.position, Gate[1].GetComponent<Transform>().position))
				{
					GateIndex = 0;
				}
				else
				{
					GateIndex = 1;
				}
				return true;
		}

		return false;
	}
}


enum State
{
	FindGenerator,
	MoveGenerator,
	WorkGenerator,
    HitDown,
    KillerGetting,
    Hooking,
    DieParticle,
	HelpTargetMove,
	HealerWait,
	HealTargetMove,
	ExitState,
	ExitWorkMove,
	ExitWork,
	ExitRun,
	Angler,
	RunAway
}


static class STATE
{
	public const int Idle = 0;
	public const int Work = 2;
	public const int Run = 3;
	public const int Hide = 4;
    public const int Down = 5;
    public const int Hook = 6;
	public const int Heal = 7;
	public const int Angler = 8;
}
