using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSkill : MonoBehaviour
{
	public GameObject trap;
	public GameObject viewPoint;

	GameObject maincam;

	void Start ()
	{
		maincam = GameObject.Find("Main Camera");
	}
	
	void Update ()
	{
		if(Input.GetMouseButtonDown(1)&& 
			PlayerMovementKiller.Self.ActionState == ActionSTATE.MOVE_STATE)
		{
			// 캐릭터의 움직임과 카메라의 움직임을 막음
			MainCamera.Self.SetCameraMoveState(CameraState.STOP);
			PlayerMovementKiller.Self.MoveControll = false;
			PlayerMovementKiller.Self.ActionState = ActionSTATE.SEPCIAL_STATE;
			PlayerMovementKiller.Self.Animat.SetInteger("ANI_STATE", ANI_STATE.SPECIAL_STATE);
			maincam.transform.SetParent(viewPoint.GetComponent<Transform>());
			maincam.transform.position = viewPoint.transform.position;
			StartCoroutine("SetTrap");

		}
	}

	IEnumerator SetTrap()
	{
		float time = 0;
		Vector3 pos = transform.position + transform.forward;
		pos.y = 0f;
		while (true)
		{
			maincam.transform.LookAt(pos);
			time += Time.deltaTime;
			if (time > 2.5f)
			{
				Instantiate(trap, pos, transform.rotation);
				StopCoroutine("SetTrap");
				StartCoroutine("ReturnCamera");
			}
			yield return null;
		}
	}

	IEnumerator ReturnCamera()
	{
		float time = 0;
		Vector3 pos = transform.position + transform.forward;
		pos.y = 0f;

		while (true)
		{
			maincam.transform.LookAt(pos);
			time += Time.deltaTime;
			if (time > 2.5f)
			{
				MainCamera.Self.SetCameraMoveState(CameraState.START);
				PlayerMovementKiller.Self.ActionState = ActionSTATE.MOVE_STATE;
				PlayerMovementKiller.Self.MoveControll = true;
				PlayerMovementKiller.Self.Animat.SetInteger("ANI_STATE", ANI_STATE.IDLE_STATE);
				maincam.transform.SetParent(null);
				StopCoroutine("ReturnCamera");
			}
			yield return null;
		}
	}
}
