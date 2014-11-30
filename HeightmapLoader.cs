using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using System.IO;
using System;

public class Terrainunit{
	public TerrainData terraindata;
	public TextureData tdata;

	public Terrainunit(TerrainData terraindata, TextureData tdata){
		this.terraindata = terraindata;
		this.tdata = tdata;
	}
}

public class HeightmapLoader : MonoBehaviour
{
	public Queue terrainstoload = new Queue();
	private int count; // the count of terrianstoload
	private bool isloading = false;
	private	bool isbusy = false;


	// Use this for initialization
	void Start ()
	{
		this.terrainstoload = new Queue ();
		this.count = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.count = terrainstoload.Count;
	}

	public void Startload(){
		if (!isloading) {
			isloading = true;
			StartCoroutine (LoadingHeightmap());
		}
	}

	public void Stopload(){
		isloading = false;
	}

	IEnumerator LoadingHeightmap(){
		if (!isloading) {
			yield break;
		}

		Terrainunit terrainunit ;
		while (true) {
			// operate once a frame
			if(count >0 && !isbusy){
				terrainunit = (Terrainunit)terrainstoload.Dequeue();
				isbusy = true;
				StartCoroutine(loadHeightmap(terrainunit.tdata, terrainunit.terraindata));
				// wait for next frame
				yield return 0;
			}
			else {
				// wait for next frame
				yield return 0;
			}

		}
	}

	public void Enqueue(TerrainData terraindata, TextureData tdata){
		Terrainunit tunit = new Terrainunit (terraindata, tdata);
		terrainstoload.Enqueue (tunit);
		
	}


	IEnumerator loadHeightmap(TextureData tdata, TerrainData terraindata){
		string url = heightmapurl (tdata.i, tdata.j, tdata.zoom);
		string localpath = heightmaplocalpath (tdata.i, tdata.j, tdata.zoom);
		FileInfo finfo = new FileInfo (localpath);
		int res = publicvar.heightmapres;
		float[,] heights = new float[res, res];
		TimeSpan timeOld ;
		TimeSpan timeNow ;
		TimeSpan deltaTime;

		for (int i = 0; i < res; i++) {
			for (int j = 0; j < res; j++) {
				heights[i,j]=0f;
			}
		}

		if (finfo.Exists) { 
			WWW www =new WWW("file://" + finfo.FullName);
			yield return www;
			byte[] buff = www.bytes;

			int index = 0;
			for(int i = res-1; i>=0; i--)  
			{  
				timeOld = new TimeSpan(DateTime.Now.Ticks);
				for(int j = 0; j<res; j++)  
				{  
					try{
						heights[i,j] = System.BitConverter.ToSingle(buff, index);
						if (heights[i,j]>60000) {
							heights[i,j] = 0;
						}
//						Debug.Log(heights[i,j]);
						heights[i,j] /= publicvar.maxHeight;
					}

					catch(Exception ex){
						print(ex.Message + "  \n"+ ex.Data);
						print("i "+ i + "j "+j);
						print("index: " +index);
					}
//					Debug.Log("height AT "+ i+" "+j+"---"+heights[i,j]);
					index += 4;
				}
				// wait for next frame in order not to use too much
				// time ,making the game running unsoomthly
				timeNow = new TimeSpan(DateTime.Now.Ticks);
				deltaTime = timeNow.Subtract(timeOld);
				if (deltaTime.Milliseconds > 100) {
					print(deltaTime.Milliseconds);
					yield return 0;
				}
				terraindata.SetHeights (0, 0, heights);
			}			

			www.Dispose();
			www=null;
		}

		else{
			WWW www = new WWW (url);
			yield return www;
			Debug.Log ("loaded from: " + url);
			//解析出高度数据

			heights = new float[res, res];
			int index = 0;
			byte[] buff = www.bytes;
//			File.WriteAllBytes(localpath, buff);
			FileStream f = File.OpenWrite(localpath);
			f.Write(buff,0,www.bytesDownloaded);
//			byte[] bytes = new byte[4];
			//		从二进制文件中得到数据
			for(int i = res-1; i>=0; i--)  
			{  
				timeOld = new TimeSpan(DateTime.Now.Ticks);
				for(int j = 0; j<res; j++)  
				{  	
//					Debug.Log("ij : "+ i + " " + j);
					heights[i,j] = System.BitConverter.ToSingle(buff, index);
					if (heights[i,j]>60000) {
						heights[i,j] = 0;
					}
//					Debug.Log("height AT "+ i+" "+j+"---"+heights[i,j]);
					heights[i,j] /= publicvar.maxHeight;
					index += 4;           
					// wait for next frame in order not to use too much
					// time ,making the game running unsoomthly
				}
				// wait for next frame in order not to use too much
				// time ,making the game running unsoomthly
				timeNow = new TimeSpan(DateTime.Now.Ticks);
				deltaTime = timeNow.Subtract(timeOld);
				if (deltaTime.Milliseconds > 100) {
					print(deltaTime.Milliseconds);
					yield return 0;
				}
				terraindata.SetHeights (0, 0, heights);
			}
			f.Close();

			www.Dispose ();
			www = null;
		}

		isbusy = false;
	}

		protected string heightmapurl(int i, int j,int zoom){
		return string.Format ("http://{0}/{1}/{2}/{3}/{4}/", publicvar.host, i, j, zoom, (int)(Math.Log(publicvar.heightmapres)/Mathf.Log (2f)));
	}

	protected string heightmaplocalpath(int i, int j, int zoom){
		string derictorypath = Application.dataPath + string.Format ("/Resources/heightmap/{0}", zoom);
		if (!Directory.Exists(derictorypath)) 
				Directory.CreateDirectory(derictorypath);
		return Application.dataPath + string.Format("/Resources/heightmap/{0}/{1}_{2}.bytes",zoom, i,j);
	}




}
