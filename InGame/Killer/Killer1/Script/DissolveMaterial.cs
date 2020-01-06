using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveMaterial : MonoBehaviour {

    public Material[] mater;

    [Range(0, 1)]
    public float Progress = 1;
    public float Speed = 0.01f;
    
	void Update ()
    {
		if(Input.GetKey(KeyCode.Space))
        {
            if (Progress == 1)
                StartCoroutine("DownProgress");
            else if (Progress == 0 )
                StartCoroutine("UpProgress");
        }
        
    }

    IEnumerator DownProgress()
    {
        while(true)
        {
            Progress -= Speed;
            foreach (Material m in mater)
            {
                m.SetFloat("_Progress", Progress);
            }
            if (Progress < 0)
            {
                Progress = 0;
                StopCoroutine("DownProgress");
            }
            yield return null;
        }
    }

    IEnumerator UpProgress()
    {
        while (true)
        {
            Progress += Speed;
            foreach (Material m in mater)
            {
                m.SetFloat("_Progress", Progress);
            }
            if (Progress > 1)
            {
                Progress = 1f;
                StopCoroutine("UpProgress");
            }
            yield return null;
        }
    }
}
