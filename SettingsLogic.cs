using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SettingsLogic : MonoBehaviour
{
	const List<string> MODES=new List<string>{"navigate","plan","settings","control"};
	publicvar publicv;

		// Use this for initialization
		void Start ()
		{
			try{
				publicv = GameObject.Find ("publicvar").GetComponent<publicvar> ();
			}catch (Exception ex){
				print("Gameobject named publicvar not found! \n  " +ex.Message);
			}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

	public void changeMode(string mode){
		if (MODES.Contains(mode)) {
			publicv.mode = mode;

				}
	}
}

