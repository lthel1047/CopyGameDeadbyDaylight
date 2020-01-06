using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionKey : MonoBehaviour
{
	bool IsStop = false;
	public SpeedCheck spc;
	public AudioSource bgm;

    public GameObject GamePlayCanvas;
    public GameObject ESC_UI;
	// Use this for initialization
	void Start ()
	{
		bgm = GetComponent<AudioSource>();
		bgm.Play();
		Cursor.visible = !Cursor.visible;
		Cursor.lockState = CursorLockMode.Locked;
        GamePlayCanvas.SetActive(true);
        ESC_UI.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
	{
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.visible = !Cursor.visible;
			
			if(IsStop)
			{
				MainCamera.Self.SetCameraMoveState(CameraState.START);
				PlayerMovementKiller.Self.MoveControll = true;
				Cursor.lockState = CursorLockMode.Locked;
                //Time.timeScale = 1;
                GamePlayCanvas.SetActive(true);
                ESC_UI.SetActive(false);
                IsStop = false;
			}
			else
			{
				MainCamera.Self.SetCameraMoveState(CameraState.STOP);
				PlayerMovementKiller.Self.MoveControll = false;
				Cursor.lockState = CursorLockMode.None;
                //Time.timeScale = 0;
                GamePlayCanvas.SetActive(false);
                ESC_UI.SetActive(true);
                IsStop = true;
			}
		}

		if(Input.GetKeyDown(KeyCode.RightBracket))
		{
			PlayerMovementKiller.Self.RunSPD += 0.1f;
			spc.UpdateSPDText();
		}

		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			PlayerMovementKiller.Self.RunSPD -= 0.1f;
			spc.UpdateSPDText();
		}
	}
}
