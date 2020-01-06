using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateUI : MonoBehaviour
{
    public Texture[] textures;
    public Slider slider;
    RawImage img;
    
	// Use this for initialization
	void Start ()
    {
        img = GetComponent<RawImage>();
        img.texture = textures[UIIMG.Health];
		img.SetNativeSize();
        slider.gameObject.SetActive(false);
    }
	
    public void ChangeUI(int _state)
    {
        img.texture = textures[_state];
		img.SetNativeSize();
	}

    public void UpdateSlider(float time,float max)
    {
        slider.value = time / max;
    }
}

static class UIIMG
{
    public const int Health = 0;
    public const int Hurt = 1;
    public const int Down = 2;
    public const int Hook = 3;
    public const int Die = 4;
	public const int Exit = 5;
}
