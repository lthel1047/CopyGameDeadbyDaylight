using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieModel : MonoBehaviour {

	public BladeMove[] blade;
    public SkinnedMeshRenderer[] body;
	AudioSource sound;
	Animator ani;

    public bool endflag=false;

	void Start ()
	{
		sound = GetComponent<AudioSource>();
		sound.clip = AudioManager.Self.sound[Sound.Scream];
		ani = GetComponent<Animator>();
        gameObject.SetActive(false);
	}
	
    public void StartEffect()
    {
        StartCoroutine("startBlade");
    }
    

	IEnumerator startBlade()
	{
        for (int i = 0; i < blade.Length; i++)
        {
            blade[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < blade.Length; i++)
		{
			blade[i].StartCoroutine("UpProgress");
		}
        ani.SetBool("Next", true);
		sound.Play();

		while (true)
        {
            if(endflag)
            {
                ani.SetBool("Next", false);
                break;
            }
            yield return null;
        }
        endflag = false;
        float tmp = 0.005f;
        float sum = 1;
        while (true)
        {
            sum -= tmp;

            for(int i=0;i<body.Length;i++)
            {
                 body[i].material.SetFloat("_Progress", sum);
            }

            if (sum <= 0)
            {
               break;
            }
            yield return null;
            
        }
        
        for (int i = 0; i < blade.Length; i++)
        {
            blade[i].StartCoroutine("DownProgress");
        }

        while (true)
        {
            if (endflag)
            {
                break;
            }
            yield return null;
        }
        Debug.Log("startBlade End");

        transform.parent.gameObject.SetActive(false);
    }
}
