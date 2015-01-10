using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct Waypoint{
	public float longitude;
	public float latitude;
	public float altitude;

	public Waypoint(float lon, float lat, float al){
		this.longitude = lon;
		this.latitude = lat;
		this.altitude = al;
	}
}

public class Pathway{
	public List<Waypoint> waypoints;
	int Count{get{
			return waypoints.Count;		
		}
	}

	Pathway(){}

		
}
public class PwManager{
	public string nameCurrentPlane;
	public Dictionary<string,Pathway> name2pw;
	public int Count{get{
			return name2pw.Count;
		}
	}


	PwManager(){
		nameCurrentPlane = "";
		name2pw = new Dictionary<string, Pathway> ();
	}

	
	public void push(string name, Waypoint wp){
		if (name2pw.ContainsKey(name)) {
			name2pw[name].waypoints.Add(wp);
		}
	}

	public void setCurrentPlane(string name){
		this.nameCurrentPlane = name;
	}

	public void newPathway(string name){
		if (name2pw [name] == null) {
						name2pw [name] = new Pathway ();
		}
	}

	public void clearPathway(string name){
		if (name2pw[name] != null){
			name2pw[name] = new Pathway();
		}
	}


	public Pathway getPathwayof(string name){
		if (name2pw [name] != null) {
						return name2pw [name];
				} else
						return null;
	}
}


