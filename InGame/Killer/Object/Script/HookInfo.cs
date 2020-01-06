using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookInfo : MonoBehaviour {

	public bool Enable = true;
    public GameObject diemodel;
    public GameObject blade;
	public GameObject point;
	public GameObject Circle;
	public int CircleTimer = 5;
	public int DieHookCount = 3;
    GameObject Survivor;


	private void Start()
	{

		Circle.SetActive(false);
	}


	public Transform GetPointTrans()
	{
		return point.GetComponent<Transform>();
	}

    public void SurvivorDie()
    {
        diemodel.SetActive(true);
        blade.SetActive(true);
        diemodel.GetComponent<DieModel>().StartEffect();
    }

    public void SetSurvivor(GameObject sur)
    {
        Survivor = sur;
        StartCoroutine("CheckState");
    }


    IEnumerator CheckState()
    {
        ControlAI info = Survivor.GetComponent<ControlAI>();
        while (true)
        {
            if(info.HookCount== DieHookCount)
            {
                SurvivorDie();
                break;
            }
            if(info.Die)
            {
                SurvivorDie();
                break;
            }

			//걸려있다 도망치면
			if(!info.Hook)
			{
				StartCoroutine("CircleCount");
				Survivor = null;
				Enable = true;
				break;
			}
            yield return null;
        }
    }


	IEnumerator CircleCount()
	{
		Circle.SetActive(true);
		float timer = 0f;
		while(true)
		{
			if(timer>CircleTimer)
			{
				Circle.SetActive(false);
				break;
			}

			timer += Time.deltaTime;
			yield return null;
		}
	}

}
