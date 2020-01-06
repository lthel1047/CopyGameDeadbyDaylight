using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour
{
	public float MoveSpeed = 1.0f;

	Animator animat;

	void Start () {
		animat = transform.Find("meg_rig").GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void SetAnimation(int state)
	{
		animat.SetInteger("ANI_STATE", state);
	}

	public void SetAnimation(bool state)
	{
		animat.SetBool("Hurt", state);
	}
}

static class AIANI
{
	public const int IDLE = 0;
	public const int WALK = 1;
	public const int WORK = 2;
	public const int RUN = 3;
	public const int HIDE = 4;
	public const int DOWN = 5;
	public const int HOOK = 6;
}
