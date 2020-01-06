using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControll : MonoBehaviour {

	public RawImage presskey;
	public RawImage blood;

	void Start ()
	{
		presskey.gameObject.SetActive(false);
		blood.gameObject.SetActive(false);
	}
	
	void Update ()
	{
	}

	public void UIPressKeyToggle()
	{
		presskey.gameObject.SetActive(!presskey.gameObject.activeSelf);
	}

	public void UIBloodOn()
	{
		blood.gameObject.SetActive(true);
		blood.color = new Color(255, 0, 0, 1);
		StartCoroutine("ColorAControll");
	}


	bool IsColorAControll = false;
	IEnumerator ColorAControll()
	{
		IsColorAControll = true;
		float tmp = 0.02f;
		while (true)
		{
			blood.color = new Color(255, 0, 0, 1f - tmp);
			if (blood.color.a <= 0)
			{
				StopCoroutine("ColorAControll");
				blood.gameObject.SetActive(false);
				IsColorAControll = false;
			}
			tmp+=0.02f;
			yield return null;
		}
	}
	private static UIControll _Self;
	public static UIControll Self
	{
		get
		{
			if (!_Self)
			{
				_Self = GameObject.FindObjectOfType(typeof(UIControll)) as UIControll;
			}
			return _Self;
		}
	}
}
