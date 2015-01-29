using System;
using System.IO;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public class LandWritaFakeData:MonoBehaviour
	{
		public string filepath = "fakedata.txt";
		public GameObject go;

		private FileInfo finfo;
		private StreamWriter file;

		void Awake ()
		{
			finfo = new FileInfo (filepath);
			if (go == null) {
				go = gameObject;
			}
			if (finfo.Exists) {
				File.Delete (filepath);
			}
			file = File.CreateText (filepath);
		}



		void Update ()
		{
			if (go.GetComponent <LandDroneController> ().isLanded == false) {
				float lx = go.transform.localPosition.x;
				float ly = go.transform.localPosition.y;
				float lz = go.transform.localPosition.z;
				string s = go.transform.localPosition.ToString ("R");
				file.WriteLine (s);
			} else {
				file.Close ();
			}
		}
	}
}

