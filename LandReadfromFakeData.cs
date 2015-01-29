using System;
using UnityEngine;
using System.IO;

namespace AssemblyCSharp
{
	public class LandReadfromFakeData:MonoBehaviour
	{
		public string filepath = "fakedata.txt";
		public GameObject go;

		//		private FileInfo finfo;
		private StreamReader file;
		private bool isReadFinished = false;

		void Awake ()
		{
			file = File.OpenText (filepath);
		}

		void Update ()
		{
			string s = " ";
			try {
				s = file.ReadLine ();
			} catch (ObjectDisposedException e) {
				isReadFinished = true;
			}
			if (s == null) {
				file.Close ();
				isReadFinished = true;
			}
			if (isReadFinished == false) {
				s = s.Substring (1, s.Length - 2);
				Debug.Log (s);
				string[] vecstring = s.Split (',');
				Vector3 locPos = new Vector3 (float.Parse (vecstring [0]), float.Parse (vecstring [1]), float.Parse (vecstring [2]));
				go.transform.localPosition = locPos;	
	
			}

		}
	}
}
