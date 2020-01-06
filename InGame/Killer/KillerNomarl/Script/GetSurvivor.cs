using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSurvivor : MonoBehaviour {

	Transform survivor;
	public GameObject viewPoint;
	public GameObject getPoint;
	
	GameObject maincam;
	public bool getSurvivor = false;

	void Start ()
	{
		maincam = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Hook")&&
			getSurvivor)
		{
			if (!UIControll.Self.presskey.gameObject.activeSelf)
				UIControll.Self.UIPressKeyToggle();
		}

        if (other.gameObject.CompareTag("Survivor") &&
            other.gameObject.GetComponent<ControlAI>().Down&&
			!other.gameObject.GetComponent<ControlAI>().Hook)
		{
			if (!UIControll.Self.presskey.gameObject.activeSelf)
				UIControll.Self.UIPressKeyToggle();
        }
    }

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Hook") &&
			getSurvivor&&
			other.GetComponent<HookInfo>().Enable&&
			Input.GetKeyDown(KeyCode.Space))
        {

            Transform point = other.GetComponent<HookInfo>().GetPointTrans();
			survivor.SetParent(point.parent);
            survivor.position = point.position;
			survivor.rotation = point.rotation;
            survivor.GetComponent<ControlAI>().Hook = true;
            other.GetComponent<HookInfo>().Enable = false;
            other.GetComponent<HookInfo>().SetSurvivor(survivor.gameObject);
            getSurvivor = false;
			survivor = null;
			PlayerMovementKiller.Self.ActionState = ActionSTATE.MOVE_STATE;
            UIControll.Self.UIPressKeyToggle();

        }


        if (other.gameObject.CompareTag("Survivor") &&
            other.gameObject.GetComponent<ControlAI>().Down&&
            !other.gameObject.GetComponent<ControlAI>().Hook)
        {
            if (Input.GetKeyDown(KeyCode.Space) &&
                PlayerMovementKiller.Self.ActionState == ActionSTATE.MOVE_STATE)
            {
                UIControll.Self.UIPressKeyToggle();

                survivor = other.gameObject.GetComponent<Transform>();
                MainCamera.Self.SetCameraMoveState(CameraState.STOP);
                PlayerMovementKiller.Self.MoveControll = false;
                PlayerMovementKiller.Self.Animat.SetInteger("ANI_STATE", ANI_STATE.SPECIAL_STATE);
                PlayerMovementKiller.Self.ActionState = ActionSTATE.GETS_STATE;
                maincam.transform.SetParent(viewPoint.GetComponent<Transform>());
                maincam.transform.position = viewPoint.transform.position;
                maincam.transform.LookAt(survivor);

                other.gameObject.GetComponent<ControlAI>().Getting = true;

                getSurvivor = true;
				if(!IsGet)
					StartCoroutine("Get");
            }
        }
    }

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Hook") &&
			getSurvivor)
		{
			if(UIControll.Self.presskey.gameObject.activeSelf)
				UIControll.Self.UIPressKeyToggle();
		}

        if (other.gameObject.CompareTag("Survivor") &&
            other.gameObject.GetComponent<ControlAI>().Down)
		{
			if (UIControll.Self.presskey.gameObject.activeSelf)
				UIControll.Self.UIPressKeyToggle();
        }
    }



	bool IsGet = false;
	IEnumerator Get()
	{
		IsGet = true;
		float time = 0;
		while (true)
		{
			time += Time.deltaTime;
			if (time > 2.5f)
			{
				survivor.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
				survivor.transform.SetParent(getPoint.transform.parent);
				survivor.localPosition = getPoint.transform.localPosition;
				survivor.localRotation = getPoint.transform.localRotation;
				StartCoroutine("ReturnCamera");
				break;
			}
			yield return null;
		}

		IsGet = false;
	}

	IEnumerator ReturnCamera()
	{
		float time = 0;

		while (true)
		{
			time += Time.deltaTime;
			if (time > 2.5f)
			{
				MainCamera.Self.SetCameraMoveState(CameraState.START);
				maincam.transform.SetParent(null);
				PlayerMovementKiller.Self.MoveControll = true;
				PlayerMovementKiller.Self.Animat.SetInteger("ANI_STATE", ANI_STATE.IDLE_STATE);
				StopCoroutine("ReturnCamera");
			}
			yield return null;
		}
	}
}
