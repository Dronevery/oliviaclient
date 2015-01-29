using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using AssemblyCSharp;


public class PlanLogic : MonoBehaviour
{
	publicvar publicv;
	DataPathway datapathway;

		// Use this for initialization
		void Start ()
		{
			try{
				publicv = GameObject.Find ("publicvar").GetComponent<publicvar> ();
			}catch (Exception ex){
				print("Gameobject named publicvar not found! \n  " +ex.Message);
			}
		datapathway = new DataPathway ();
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (publicv.mode == "plan") {
						updateUIwith (datapathway.nameCurrentPlane, datapathway.getPathwayof(datapathway.nameCurrentPlane));
				}

		}

	void OnMouseDown(){
		if (publicv.mode == "plan") {
			if (isOnmap()){
				Vector3 pos = getPosfrom(Input.mousePosition);
				Location loc  = maplib.getLonLatfrom(pos); // wp.altitude==0
				datapathway.Push(datapathway.nameCurrentPlane ,loc);
				updateUIwith (datapathway.nameCurrentPlane, datapathway.getPathwayof(datapathway.nameCurrentPlane));
			}
		}
	}

	private void updateUIwith (string name, Pathway pathway){
		// TODO

	}


	private bool isOnmap(){
		return true;
		}

	private Vector3 getPosfrom(Vector3 mousePos){
		return new Vector3(0,0,0);
	}
}

