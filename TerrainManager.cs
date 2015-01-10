using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class TerrainManager : MonoBehaviour
{
	// a manager only manage the tiles around one aircraft

	public Dictionary<string, Terrain> terrains;
	public HeightmapLoader heightsloader;
	public int maxTileX ; // tiles farther than this will be destroyed
	public int maxTileY ; // tiles farther than this will be destroyed
	public int basei ;
	public int basej ;
	public GameObject plane;
	private bool isUpdating = false;


//---------------------------------------------------------------------------------------

	public void Start(){
		terrains = GameObject.Find("publicvar").GetComponent<publicvar> ().terrains;
		basei = publicvar.basei;
		basej = publicvar.basej;
		maxTileX = publicvar.maxTileX;
		maxTileY = publicvar.maxTileY;
		heightsloader = GameObject.Find("HeightmapLoader").GetComponent<HeightmapLoader>();
	}

	public Terrain NewTerrainData(TerrainMetaData mdata, TextureData tdata, HeightmapMetaData hdata){
		Terrain terrain;
		if (terrains.ContainsKey(str(tdata.i,tdata.j, tdata.zoom))) {
			terrain = terrains[str(tdata.i,tdata.j,tdata.zoom)];
			terrain.gameObject.SetActive(true);
			return terrain;
				}
		TerrainData terraindata = new TerrainData ();
		// set some paras of the terrainTile
		_InitTerrain(terraindata, mdata, hdata);

		// start a coroutine to load the texture
		StartCoroutine(loadTexture(tdata, terraindata));

		heightsloader.Enqueue (terraindata, tdata);

		terrain = setTerrPos (terraindata, tdata);

		// register to the manager
			terrains.Add(str(tdata.i,tdata.j, tdata.zoom), terrain);

		return terrain;
	}

	public void DestroyTerrain(int i ,int j, int zoom){
		Terrain terrain = terrains[str (i, j, zoom)];
		GameObject.Destroy (terrain);
		terrains[str (i,j, zoom)] = null;
	}
	

	public void StartUpdate(){
		this.isUpdating = true;
		StartCoroutine (Updatemap ());
	}

	public void StopUpdate(){
		this.isUpdating = false;
	}

	public Terrain getTerrain(int i, int j, int zoom){
		return terrains [str (i,j, zoom)];
	}

//	public Terrain getActiveTerrain(int zoom){
//		int[] center = maplib.getCurrentTile (plane ,zoom);
//		return terrains [str (center[0],center[1])];
//	}

	public int numTerrain(){
		return terrains.Count;
	}

	public TerrainManager(GameObject plane){
		this.plane = plane;
	}
//----------------------------------------------------------------------------------------

	// Once started ,the function will go through all tiles responsible to be loaded.
	// and then load those which hasn't been loaded, clean whose which are too far.
	IEnumerator Updatemap(){
		int[] center;
		while (true) {
			//wait for a second
			yield return new WaitForSeconds(3);
			Debug.Log ("count"+terrains.Count);
			// clear remote tiles
			center = maplib.getCurrentTile(plane, publicvar.basezoom);
			Debug.Log ("current tile--- i: " + center[0] + "  j: "+center[1]);
			StartCoroutine(ClearRemoteTiles(center));
			
			//wait for a second 
			yield return new WaitForSeconds(1);
			// flush new tiles
			center = maplib.getCurrentTile(plane, publicvar.basezoom);
			StartCoroutine(FlushNewTiles(center));
			if (!isUpdating) break;
		}
	}

	IEnumerator ClearRemoteTiles(int[] center){
		while (terrains == null) {
			Debug.Log("terrain null");
			yield return 0;
		}

		ArrayList terrainstodelete = new ArrayList();
		foreach (KeyValuePair<string, Terrain> entry in terrains) {
			int i = int.Parse(entry.Key.Split(',')[0]);
			int j = int.Parse(entry.Key.Split(',')[1]);

			if (Math.Abs(i-center[0]) > maxTileX || Math.Abs (j-center[1]) > maxTileY){
//				DestroyTerrain(entry.Key);
				terrainstodelete.Add(new int[2]{i, j});
			}
		}
		foreach(int[] ij in terrainstodelete){
//			DestroyTerrain(ij[0], ij[1], publicvar.basezoom);
		}
		yield return 0;

	}

	IEnumerator FlushNewTiles(int[] center){
		while (terrains == null) {
			Debug.Log("terrain null");
			yield return 0;
		}
		for (int i =center[0] -maxTileX; i <= center[0]+maxTileX; i++) 
			for (int j = center[1]-maxTileY; j <= center[1]+maxTileY; j++)
			if (!terrains.ContainsKey(str (i,j, publicvar.basezoom))) {
				TerrainMetaData mdata = new TerrainMetaData();
				TextureData tdata = new TextureData(i, j);
				HeightmapMetaData hdata = new HeightmapMetaData();
				yield return NewTerrainData(mdata, tdata, hdata);
			}

		yield return 0;
	}


	private void _InitTerrain(TerrainData tdata, TerrainMetaData mdata, HeightmapMetaData hdata){
		tdata.heightmapResolution = publicvar.heightmapres;
	}
	

	IEnumerator loadTexture(TextureData tdata, TerrainData terraindata){
		int i = tdata.i;
		int j = tdata.j;
		string path = mappath(i,j, tdata.zoom);
		FileInfo fi1 = new FileInfo(path);   
		WWW www;
		// file exists. Load from local cache.
		if (fi1.Exists)
		{
			www = new WWW("file://"+path);
			yield return www;
			if (www.error != null )
			{
				print("Error:"+ www.error);
			}
			else
			{
				var splats = new SplatPrototype[1];
				splats[0] = new SplatPrototype();
				splats[0].texture = www.texture;
				splats[0].tileSize = new Vector2(tdata.lengthmesh, tdata.lengthmesh);
				
				terraindata.splatPrototypes = splats;
				www.Dispose();
				www =null;
			}
		}


		// file not exits. load from Internet
		else{
			string url = remotemapurl(i,j,publicvar.basezoom);
			www=new WWW(url);
			yield return www;
			if (www.error != null ){
				print("Error:"+ www.error);
				yield return www;
			}
			else{
				var splats = new SplatPrototype[1];
				splats[0] = new SplatPrototype();
				splats[0].texture = www.texture;
				splats[0].tileSize = new Vector2(tdata.lengthmesh, tdata.lengthmesh);

				terraindata.splatPrototypes = splats;
				byte[] jpgData = www.texture.EncodeToJPG();
				File.WriteAllBytes(path, jpgData);

				www.Dispose();
				www =null;
			}
		}
	}

	protected string remotemapurl(int i, int j, int zoom){
		return string.Format ("https://api.tiles.mapbox.com/v3/examples.map-qfyrx5r8/{0}/{1}/{2}.jpg",zoom, i, j);
	}

	protected string mappath(int i,int j, int zoom){
		string derictorypath = Application.dataPath + string.Format ("/Resources/map/{0}", zoom);
		if (!Directory.Exists(derictorypath)) 
			Directory.CreateDirectory(derictorypath);
		return string.Format(Application.dataPath + "/Resources/map/{0}/{1}_{2}.jpg", zoom, i, j);
	}

	public string str(int i, int j, int zoom){
		return String.Format ("{0},{1},{2}", i, j, zoom);
	}

	public Terrain setTerrPos (TerrainData terraindata, TextureData tdata){
		float[] xz = maplib.getPos (tdata);
		GameObject terrainobject = Terrain.CreateTerrainGameObject (terraindata);
		terrainobject.name = string.Format ("{0},{1},{2}", tdata.i, tdata.j, tdata.zoom);
		Terrain terrain = terrainobject.GetComponent<Terrain> ();
		terraindata.size = new Vector3 (tdata.lengthmesh, publicvar.maxHeight, tdata.lengthmesh);
		terrain.transform.position = new Vector3 (xz[0],0,xz[1]);
		return terrain;
	}

	
}



