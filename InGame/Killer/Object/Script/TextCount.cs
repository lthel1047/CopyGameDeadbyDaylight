using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextCount : MonoBehaviour
{
	public int count = 5;
	Text text;
	void Start ()
	{
		text = GetComponent<Text>();
	}
	
    public void CountMinus()
    {
        count--;
		if (count < 0)
			count = 0;

		HookingCheck.Self.GCount(count);

		text.text = count.ToString();
    }
	


	private static TextCount _Self;
	public static TextCount Self
	{
		get
		{
			if (!_Self)
			{
				_Self = GameObject.FindObjectOfType(typeof(TextCount)) as TextCount;
			}
			return _Self;
		}
	}
}
