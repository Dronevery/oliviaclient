using UnityEngine;
using System.Collections;
using LitJson;

// used to interact with the UI layer
// simple encapsulation of some operations such as connected to a new drone

public class UIManager : MonoBehaviour
{
	public GameObject uavsPanel;
	public GameObject missionPanel;
	public GameObject settingsPanel;
	public GameObject logPanel;

	public RectTransform uavEntryPrefab;


	private GameObject _uavList;
	private connect _conn;


	// Use this for initialization
	void Start ()
	{
		// get some private objects;
		//this._uavList = GameObject.FindGameObjectWithTag ("uavlist");
		//this._conn = GameObject.Find ("Connecter").GetComponent <connect> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void DoArm (GameObject go)
	{
		_Arm (go.GetComponent <UnityEngine.UI.Text> ().text);
	}

	public void DoDisArm (GameObject go)
	{
		_DisArm (go.GetComponent <UnityEngine.UI.Text> ().text);
	}

	public  void  AddUAVEntry (string nam)
	{
		RectTransform newUAVEntry = Instantiate (uavEntryPrefab) as RectTransform;
		newUAVEntry.FindChild ("name").GetComponent <UnityEngine.UI.Text> ().text = nam;
		newUAVEntry.name = nam;
	}

	private void _Arm (string name)
	{
		_conn.send ("{'event':'cmd','data':{'name':'" + name + "','action':'arm'}}");
	}

	private void _DisArm (string name)
	{
		_conn.send ("{'event':'cmd','data':{'name':'" + name + "','action':'disarm'}}");
	}



}
