using AssemblyCSharp;
using UnityEngine;
using System.Collections;

public class Test1 : MonoBehaviour
{
	public DataPostureSphere dps;
	public GameObject go;
	// Use this for initialization
	void Start ()
	{
//		StartCoroutine(Test1_1 ());
	}

	// Update is called once per frame
	void Update ()
	{
		go.transform.Rotate (new Vector3 (0, 0, 20 * Time.deltaTime));
		var eulerangle = new Vector3 (0, 0, 20 * Time.deltaTime+go.transform.rotation.eulerAngles.z);
		dps.UpdateData ("test", eulerangle);

		
	}

}

