using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class PlanLogic : MonoBehaviour
{
	publicvar publicv;
	PwManager pwmanager;

		// Use this for initialization
		void Start ()
		{
			try{
				publicv = GameObject.Find ("publicvar").GetComponent<publicvar> ();
			}catch (Exception ex){
				print("Gameobject named publicvar not found! \n  " +ex.Message);
			}
		pwmanager = new PwManager ();
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (publicv.mode == "plan") {
						updateUIwith (pwmanager.nameCurrentPlane, pwmanager.getPathwayof(pwmanager.nameCurrentPlane));
				}

		}

	void OnMouseDown(){
		if (publicv.mode == "plan") {
			if (isOnmap()){
				Vector3 pos = getPosfrom(Input.mousePosition);
				Waypoint wp = maplib.getLonLatfrom(pos); // wp.altitude==0
				pwmanager.push(pwmanager.nameCurrentPlane,wp);
				updateUIwith (pwmanager.nameCurrentPlane, pwmanager.getPathwayof(pwmanager.nameCurrentPlane));
			}
		}
	}

	private void updateUIwith (string name, Pathway pathway){
		// TODO

	}


	private bool isOnmap(){
		}

	private Vector3 getPosfrom(Vector3 mousePos){}
}

