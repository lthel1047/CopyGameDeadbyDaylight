using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineControl : MonoBehaviour {

	public Transform Killer;
	public Material Fill;
	public Material Mask;
	public MeshRenderer[] mesh;

	void Start ()
	{
		StartCoroutine("CheckDistance");
	}

	IEnumerator CheckDistance()
	{
		bool flag = false;
		while(true)
		{
			float dis = KillerDis();
			Debug.Log(dis);
			if(dis<10f)
			{
				Debug.Log("sdfsdfsdfsdfsdfdsfsfsdfsfsdf");
				flag = true;
				for (int i=0;i<mesh.Length;i++)
				{
					Material[] mater = new Material[3];
					mater = mesh[i].materials;
					mater[1] = null;
					mater[2] = null;
				}
			}
			else
			{
				if (flag)
					CreateOutlien();
			}

			yield return null;
		}
	}

	void CreateOutlien()
	{
		for (int i = 0; i < mesh.Length; i++)
		{
			Material[] mater = mesh[i].sharedMaterials;
			mater[1] = Fill;
			mater[2] = Mask;
		}
	}
	

	float KillerDis()
	{
		float tmp;
		tmp = Vector3.Distance(transform.position, Killer.position);

		return tmp;
	}
}
