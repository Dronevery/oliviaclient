using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class AirManager : MonoBehaviour
{
	public Dictionary<string, airobj> airs;
	public GameObject originair;

	public airobj CreateAir (string name, GameObject originair, JsonData jd)
	{
		airobj air = new airobj (name, originair, jd);
		Console.print ("created a new airobj --- " + name);
		airs [name] = air;
		Console.print ("stored airobj into dict --- " + name);
		return air;
	}

	public int UpdateAir (string name, JsonData jd)
	{
		foreach (DictionaryEntry entry in jd) {
			airs [name].update (jd);
			return 0;
		}
		return -1;
	}

	public int UpdateOrCreate (JsonData jd)
	{
		foreach (DictionaryEntry entry  in jd) {
			string name = (string)entry.Key;
			JsonData data = (JsonData)entry.Value;
			try {
				if (airs.ContainsKey (name)) {
					UpdateAir (name, data);
				} else {
					CreateAir (name, originair, data);
				}
			} catch (System.Exception ex) {
				print (ex.Data);
				print (ex.Message);
			}
		
			return 0;
		}
		return -1;
	}

	public void DestroyAir (string name)
	{
		airs [name].destroy ();
		airs [name] = null;
	}

	public int numAir ()
	{
		return airs.Count;
	}

}

