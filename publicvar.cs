using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class publicvar : MonoBehaviour {
	public static Vector3 landPosition = new Vector3 (-1000, -1000, -1000);
	public string mode = "navigate";
	public GameObject originairplane1; // the model of the plane
	public GameObject originairplane2;
	public static int basezoom = 13; // represent the size of one tile (Cut the whole into 2^zoom x 2^zoom tiles)

	public static string ip_addr = "127.0.0.1";
	public static string host ="map.dronevery.com:8000";// "127.0.0.1:8000";
	public static string username = "admin"; // use for authorizationo
	public static string passwd = "qiaochu";

	public static int basei = 7213; // the i of the tile in the Point (0,0,0) // not in use
	public static int basej = 3066;  // not in use



	public static float lengthmesh_17 = 2296.26f;
	public static float lengthmesh = lengthmesh_17;//*Mathf.Pow(2,17-zoom);// the size of one tile in Unity world

	public static int loadwidth = 5;
	public static int maxTileX = loadwidth; // basei-maxTileX < i < basei+maxTileX will be loaded
	public static int maxTileY = loadwidth;

	public static int heightmapres = 33; // the resolution of heightmap (must be 2^n+1)
	public static float maxHeight = 5000; // the height of the terrain box



		public static bool isloadfromLonLat = false; // not in use
		public static float longitude = 137f; 		// not in use
		//117.2688041888f;
		public static float latitude = 41.1472260440f;// not in use
		//31.8371046436f;

	public Dictionary<string, Terrain> terrains ;
	public Dictionary<string, airobj> airs;


	void Start(){
		airs= new Dictionary<string, airobj >();
		terrains = new  Dictionary<string, Terrain> ();
	}
}