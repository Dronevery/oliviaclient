using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadDetailTerrain : MonoBehaviour
{
	public GameObject third_ca;
	private Camera ca;
	private const float baseheight = 15000f;
	private TerrainManager tmngr=null;
	private bool isbusy = false;
	private int levelnow = 2;
	private Dictionary<int ,int > level_zoom = new Dictionary<int, int>(){
		{0, 17},
		{1, 15},
		{2, 13}
	};
	
		// Use this for initialization
		void Start ()
		{
			if (third_ca != null) {
				ca = third_ca.GetComponent<Camera>();
			}
		levelnow = getLevel (publicvar.basezoom);
			StartCoroutine (findPlane());

		}
	
		// Update is called once per frame
		void Update ()
		{
		if (this.tmngr == null) {
			return;
			}
		if (third_ca != null) {
			float height = third_ca.transform.position.y;
			int level = getLevel(height);
			if (level != levelnow) {
				StartCoroutine(Ontriggered(level));	
			}

		}
	
		}

	IEnumerator findPlane(){
		while (true) {
			yield return new WaitForSeconds(2);
			try {
				GameObject g = GameObject.Find ("qiaochu");
				tmngr = g.GetComponent<TerrainManager> ();
//				tmngr.plane = g;
//				tmngr.StartUpdate ();
			} catch (System.Exception ex) {
				Debug.Log(ex.Message);
			}
				}
	}

	IEnumerator Ontriggered(int l){
		yield return new WaitForSeconds (2);
		if (!isbusy) {
			Debug.Log("i'm called!!");
			isbusy = true;
			levelnow = l;
			int zoom = level_zoom[l];
			HideOldTerrains(zoom);

			HeightmapMetaData hdata = new HeightmapMetaData();
			TerrainMetaData mdata = new TerrainMetaData();
			int[] center = maplib.getij(zoom, new Vector2(third_ca.transform.position.x, third_ca.transform.position.z));
			Debug.Log("center: "+center[0]+" , "+center[1]);
			print("loadwidth: " + publicvar.loadwidth);
			for (int i = center[0]-publicvar.loadwidth; i < center[0]+publicvar.loadwidth; i++) {
				for (int j = center[1]-publicvar.loadwidth; j < center[1]+publicvar.loadwidth; j++) {
					Debug.Log(i+", "+j);
					TextureData tdata = new TextureData(zoom, i, j);
					yield return tmngr.NewTerrainData(mdata,tdata,hdata);
				}
			}

			isbusy = false;
		}
	}

	public void HideOldTerrains(int zoom){
		foreach (KeyValuePair<string, Terrain> entry in tmngr.terrains) {
			int zoooom = int.Parse(entry.Key.Split(',')[2]);
			if (zoooom != zoom) {
				entry.Value.gameObject.SetActive(false);
			}
		}
	}

	public static int getLevel(float height){
		float[] levels = new float[4]{0,3000,7000,100000};
		int result = 0;
		for (int i = 0; i < 3; i++) {
			if (height<levels[i+1] && height>levels[i]) {
				result = i;
				return result;
			}
		}
		return 2;
	}



}

