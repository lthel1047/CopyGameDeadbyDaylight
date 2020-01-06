using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Round : MonoBehaviour
{
	public Transform player;
	public Transform point;
	public GameObject[] Survivor;
	MainCamera maincam;
	
	void Start ()
	{
		player.GetComponent<PlayerMovementKiller>().MoveControll = false;
		maincam = GetComponent<MainCamera>();
		for (int i=0;i<Survivor.Length;i++)
		{
			Survivor[i].GetComponent<NavMeshAgent>().speed = 0.1f;
		}
		maincam.enabled = false;

		StartCoroutine("RoundCamera");
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	IEnumerator RoundCamera()
	{
		float count = 0;
		while(true)
		{
			transform.LookAt(point);
			Vector3 nor = Quaternion.Euler(0, count, 0) * -player.right;
			transform.position = player.position + nor * 2f + new Vector3(0,2.3f,0);
			count += 0.5f;

			if (count > 270)
				break;

			yield return new WaitForSeconds(0.01f);
		}
		count = 0;

		Vector3 pos = player.transform.position - transform.position;
		pos = pos / 100;
		while (true)
		{
			transform.position += pos;
			count++;
			if (count > 50)
				break;
			yield return null;
		}

		for (int i = 0; i < Survivor.Length; i++)
		{
			Survivor[i].GetComponent<NavMeshAgent>().speed = 3.0f;
		}
		maincam.enabled = true;
		player.GetComponent<PlayerMovementKiller>().MoveControll = true;

	}
}
