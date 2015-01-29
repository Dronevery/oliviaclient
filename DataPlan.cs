using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

#region interfaces
public interface IDataPathway{
	int Count{ get; }
	void CreatePathwayof (string name);
	void ClearPathwayof (string name);
	void RemovePathwayof (string name);
	Pathway getPathwayof (string name);
	void Push (string name, Location wp);
}

#endregion


#region classes
public class Pathway{
	public string name;
	public List<Location> waypoints;
	int Count{get{
			return waypoints.Count;		
		}
	}

	public Pathway(){
		waypoints = new List<Location> ();
		this.name = "anonymous";
	}

	public Pathway(string name){
		waypoints = new List<Location> ();
		this.name = name;
	}
}


public class DataPathway: IDataPathway{
	public string nameCurrentPlane;
	public Dictionary<string,Pathway> pathways;
	public int Count{get{
			return pathways.Count;
		}
	}


	public DataPathway(){
		nameCurrentPlane = "";
		pathways = new Dictionary<string, Pathway> ();
	}

	
	public void Push(string name, Location wp){
		if (pathways.ContainsKey(name)) {
			pathways[name].waypoints.Add(wp);
		}
	}

	public void setCurrentPlane(string name){
		this.nameCurrentPlane = name;
	}

	public void CreatePathwayof(string name){
		if (!pathways.ContainsKey(name)) {
			pathways.Add(name, new Pathway(name));
		}
		else{
			Debug.Log("CreatePathway: the pathway of "+name+" already exists.");
			if (pathways[name] == null) {
				pathways[name] = new Pathway(name);
			}
		}
	}

	public void ClearPathwayof(string name){
		if (pathways.ContainsKey(name)){
			pathways[name] = new Pathway(name);
		}
		else {
			CreatePathwayof(name);
				}
	}

	public void RemovePathwayof(string name){
		if (pathways.ContainsKey(name)) {
			pathways.Remove(name);
		}
	}


	public Pathway getPathwayof(string name){
		if (pathways [name] != null) {
						return pathways [name];
				} else
						return null;
	}
}
#endregion


