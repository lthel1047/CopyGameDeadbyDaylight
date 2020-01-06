using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeMove : MonoBehaviour {

	public Transform Point;
    public DieModel dimodel;
	MeshRenderer mater;

	void Start ()
	{
		mater = GetComponent<MeshRenderer>();
		mater.sharedMaterial.SetFloat("_Progress", 0);
        gameObject.SetActive(false);
    }

	IEnumerator UpProgress()
	{
		float tmp = 0.004f;
		float sum = 0;
		while(true)
		{
			sum += tmp;
			mater.sharedMaterial.SetFloat("_Progress", sum);

			if (sum > 1f)
			{
				StartCoroutine("MoveBlade");
				break;
			}
			yield return null;
		}
	}

    IEnumerator DownProgress()
    {
        float tmp = 0.004f;
        float sum = 1;
        while (true)
        {
            sum -= tmp;
            mater.sharedMaterial.SetFloat("_Progress", sum);

            if (sum < 0)
            {
                dimodel.endflag = true;
                break;
            }
            yield return null;
        }
    }


    IEnumerator MoveBlade()
	{
		Vector3 pos = ( Point.position- transform.position ).normalized;
		int count = 0;
		yield return new WaitForSeconds(0.5f);
		while (true)
		{
			if (count < 130)
			{
				transform.position -= pos / 1000;
			}
			else
			{
				transform.position += pos / 20;


                if (count > 150)
                {
                    dimodel.endflag = true;
                    break;
                }
			}
			count++;

			yield return null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DieModel"))
			Debug.Log("dfdfsd");
	}
}
