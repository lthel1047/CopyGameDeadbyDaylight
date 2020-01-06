using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour {

	GameObject soundtmp;
	AudioSource kicksound;
	// Use this for initialization
	void Start ()
	{
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Generator") &&
			other.gameObject.GetComponent<GeneratInfo>().KillerKick)
		{
			UIControll.Self.UIPressKeyToggle();
		}
	}

	private void OnCollisionStay(Collision other)
	{
		if(other.gameObject.CompareTag("Generator")&&
			other.gameObject.GetComponent<GeneratInfo>().KillerKick&&
			PlayerMovementKiller.Self.ActionState == ActionSTATE.MOVE_STATE&&
			Input.GetKeyDown(KeyCode.Space))
		{
			MainCamera.Self.SetCameraMoveState(CameraState.STOP);
			PlayerMovementKiller.Self.MoveControll = false;

			PlayerMovementKiller.Self.Animat.SetInteger("ANI_STATE", ANI_STATE.KICK_STATE);
			PlayerMovementKiller.Self.ActionState = ActionSTATE.KICK_STATE;
			other.gameObject.GetComponent<GeneratInfo>().AttackGenerator();

			UIControll.Self.UIPressKeyToggle();
			if (!IsCheckKickAction)
				StartCoroutine("CheckKickAction");

			StartCoroutine("KickSound");
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.CompareTag("Generator") &&
			other.gameObject.GetComponent<GeneratInfo>().KillerKick)
		{

			UIControll.Self.UIPressKeyToggle();
		}
	}

	bool IsCheckKickAction = false;
	IEnumerator CheckKickAction()
	{
		IsCheckKickAction = true;
		while (true)
		{
			if(PlayerMovementKiller.Self.Animat.GetCurrentAnimatorStateInfo(0).normalizedTime>0.9f)
			{

				MainCamera.Self.SetCameraMoveState(CameraState.START);
				PlayerMovementKiller.Self.MoveControll = true;
				PlayerMovementKiller.Self.ActionState = ActionSTATE.MOVE_STATE;
				StopCoroutine("CheckKickAction");
				IsCheckKickAction = false;
			}
			yield return null;
		}
	}

	IEnumerator KickSound()
	{
		soundtmp = new GameObject("soundtmp");
		kicksound = soundtmp.AddComponent<AudioSource>();
		kicksound.playOnAwake = false;
		kicksound.volume = 0.5f;
		kicksound.clip = AudioManager.Self.sound[Sound.Kick];
		kicksound.Play();
		while (true)
		{
			if (!kicksound.isPlaying)
			{
				kicksound.Play();
				break;
			}
			yield return null;
		}

		while (true)
		{
			if (!kicksound.isPlaying)
			{
				break;
			}
			yield return null;
		}

		Destroy(soundtmp);
	}
}
