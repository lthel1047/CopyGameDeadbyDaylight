using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookingCheck : MonoBehaviour {

	public ControlAI[] Survivor;
	int EndCount = 0;
	int DieCount = 0;
	int ExitCount = 0;
	int GeneratorCount = 0;
	object Lock = new object();

	public GameObject MainCam;
	public GameObject ResultObj;

	public GameObject MainCanvas;
	public GameObject ResultCanvas;


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
			ResultStart();
	}

	public void FindHelper(Transform help)
	{
		int count = 0;
		for (int i = 0; i < Survivor.Length; i++)
		{
			if (Survivor[i].transform == help)
				continue;
			if (!Survivor[i].Hook && !Survivor[i].Down && !Survivor[i].Helper && !Survivor[i].Healer &&
				Survivor[i].GetState() != "RunAway")
			{
				Survivor[i].SetHelpTarget(help);
				Survivor[i].Helper = true;
				count++;
				if (count == 2) break;
			}
		}

	}

	public bool FindHealer(Transform help)
	{

		for (int i = 0; i < Survivor.Length; i++)
		{
			if (Survivor[i].transform == help)
				continue;
			if (!Survivor[i].Hook && !Survivor[i].Down && !Survivor[i].Helper && !Survivor[i].Healer &&
				Survivor[i].GetState() != "RunAway")
			{
				if (Survivor[i].Hurt && Survivor[i].GetHealer() == help)
				{
					continue;
				}
				return true;
			}
		}
		return false;
	}

	public bool SetHealer(Transform help)
	{

		for (int i = 0; i < Survivor.Length; i++)
		{
			if (Survivor[i].transform == help)
				continue;
			if (!Survivor[i].Hook && !Survivor[i].Down && !Survivor[i].Helper && !Survivor[i].Healer &&
				Survivor[i].GetState() != "RunAway")
			{
				if (Survivor[i].Hurt && Survivor[i].GetHealer() == help)
				{
					continue;
				}
				Survivor[i].SetHealTarget(help);
				Survivor[i].Healer = true;
				help.GetComponent<ControlAI>().SetHealer(Survivor[i].transform);
				return true;
			}
		}
		return false;
	}

	public void GCount(int _data)
	{
		GeneratorCount = _data;

		if(GeneratorCount == 0)
		{

			for(int i=0;i<Survivor.Length;i++)
			{
				Survivor[i].Exit = true;
			}
		}
	}
	
	public void EndCountUp(string end)
	{
		if (end == "Die")
			DieCount++;
		else if (end == "Exit")
			ExitCount++;

		EndCount++;
		if(EndCount==4)
		{
			ResultStart();
			Debug.Log("Game End");
		}
	}

	private static HookingCheck _Self;
	public static HookingCheck Self
	{
		get
		{
			if (!_Self)
			{
				_Self = GameObject.FindObjectOfType(typeof(HookingCheck)) as HookingCheck;
			}
			return _Self;
		}
	}

	void ResultStart()
	{
		MainCam.GetComponent<OptionKey>().bgm.Stop();
		MainCam.GetComponent<OptionKey>().bgm.clip = AudioManager.Self.sound[Sound.Result];
		MainCam.GetComponent<OptionKey>().bgm.loop = false;
		MainCam.GetComponent<OptionKey>().bgm.playOnAwake = false;
		MainCam.GetComponent<OptionKey>().bgm.Play();
		MainCam.GetComponent<MainCamera>().SetCameraMoveState(CameraState.STOP);
		ResultObj.SetActive(true);
		ResultCanvas.SetActive(true);
		MainCanvas.SetActive(false);
		MainCam.GetComponent<Camera>().enabled = false;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}
