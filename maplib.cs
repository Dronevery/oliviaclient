using UnityEngine;
using System;
using System.Collections;

public class maplib //: MonoBehaviour
{
	public static int[] lnglatToXY(float longitude, float latitude, int zoom){
		float lat_rad;
		float n;
		int xtile;
		int ytile;
		lat_rad = (float) latitude * Mathf.PI / 180.0f;
		n = Mathf.Pow(2, zoom);
		xtile =(int) Mathf.Floor((longitude + 180.0f) / 360.0f * n);
		ytile =(int) Mathf.Floor((1.0f - Mathf.Log(Mathf.Tan(lat_rad) + (1.0f / Mathf.Cos(lat_rad))) / Mathf.PI) / 2.0f * n);
		int[] answer = new int[2];
		answer [0] = xtile; answer [1] = ytile;
		return answer; 
	}

	public static float[] XYToLonLat(int xtile, int ytile, int zoom){
		float lon_deg;
		float lat_deg;
		float lat_rad;
		float n;
		n = Mathf.Pow (2.0f, zoom);
		lon_deg = (xtile * 360.0f) / n - 180.0f;
		lat_rad = Mathf.Atan (Convert.ToSingle( System.Math.Sinh(Mathf.PI * (1.0f - 2.0f * ytile / n))));
		lat_deg = Mathf.Rad2Deg * lat_rad;
		return new float[] {lon_deg, lat_deg};	
	}


	// given 
	public static float[] getUnityPosfromLatlng(float lon, float lat, int zoom){
		// get lon, lat length
		float lonlength = Mathf.Abs(XYToLonLat(publicvar.basei,publicvar.basej,publicvar.basezoom)[0]-XYToLonLat(publicvar.basei+1,publicvar.basej,publicvar.basezoom)[0]);
		float latlength = Mathf.Abs(XYToLonLat(publicvar.basei,publicvar.basej,publicvar.basezoom)[1]-XYToLonLat(publicvar.basei,publicvar.basej+1,publicvar.basezoom)[1]);
		
		// given lon lat
		// get tileNum
		int[] tileNum = lnglatToXY (lon, lat, zoom);
		
		// get tilelon tilelat of tile top-left
		float[] LonlatTopleft = XYToLonLat (tileNum [0], tileNum [1], zoom);
		
		// get delta of the lon, lat
		float[] deltaLonlat = new float[] {lon- LonlatTopleft[0],lat-LonlatTopleft[1]};
		
		// get x,z of topleft
		float[] posTopleft = new float[]{((tileNum [0] - publicvar.basei) - 0.5f) * publicvar.lengthmesh,
			(-(tileNum [1] - publicvar.basej) + 0.5f) * publicvar.lengthmesh,
		};
		
		// get x,z of the given lon,lat
		float[] position = new float[]{
			posTopleft[0]+publicvar.lengthmesh*deltaLonlat[0]/lonlength,
			posTopleft[1]+publicvar.lengthmesh*deltaLonlat[1]/latlength
		};
		return new float[]{
			position[0], 0, position[1]
		};		
	}



	public static float[] getLonLatfrom(GameObject target){
		float x = target.transform.position.x;
		float z = target.transform.position.z;
		float lonlength = Mathf.Abs(XYToLonLat(publicvar.basei,publicvar.basej,publicvar.basezoom)[0]-XYToLonLat(publicvar.basei+1,publicvar.basej,publicvar.basezoom)[0]);
		float latlength = Mathf.Abs(XYToLonLat(publicvar.basei,publicvar.basej,publicvar.basezoom)[1]-XYToLonLat(publicvar.basei,publicvar.basej+1,publicvar.basezoom)[1]);
		float longitude = XYToLonLat (publicvar.basei, publicvar.basej, publicvar.basezoom) [0] + (x + 0.5f*publicvar.lengthmesh) * lonlength / publicvar.lengthmesh;
		float latitude = XYToLonLat (publicvar.basei, publicvar.basej, publicvar.basezoom) [1] + (z - 0.5f*publicvar.lengthmesh) * latlength / publicvar.lengthmesh;
		return new float[2]{longitude, latitude};
	}

	public static Waypoint getLonLatfrom(Vector3 pos){
		float x = pos.x;
		float z = pos.z;
		float lonlength = Mathf.Abs(XYToLonLat(publicvar.basei,publicvar.basej,publicvar.basezoom)[0]-XYToLonLat(publicvar.basei+1,publicvar.basej,publicvar.basezoom)[0]);
		float latlength = Mathf.Abs(XYToLonLat(publicvar.basei,publicvar.basej,publicvar.basezoom)[1]-XYToLonLat(publicvar.basei,publicvar.basej+1,publicvar.basezoom)[1]);
		float longitude = XYToLonLat (publicvar.basei, publicvar.basej, publicvar.basezoom) [0] + (x + 0.5f*publicvar.lengthmesh) * lonlength / publicvar.lengthmesh;
		float latitude = XYToLonLat (publicvar.basei, publicvar.basej, publicvar.basezoom) [1] + (z - 0.5f*publicvar.lengthmesh) * latlength / publicvar.lengthmesh;
		return new Waypoint(longitude,latitude,0);
	}





	public static float[] getPos(TextureData tdata){

		float[] lonlat = XYToLonLat (publicvar.basei, publicvar.basej, publicvar.basezoom);
		int[] newbase = lnglatToXY (lonlat [0], lonlat [1],tdata.zoom);
		float[] xy = new float[2];
//		float k = Mathf.Pow (2, tdata.zoom - publicvar.basezoom-1);
//		float newbasei = 2*k * publicvar.basei + k;
//		float newbasej = 2*k * publicvar.basej + k;

		xy [0] = (tdata.i - newbase[0] + 0.5f) * tdata.lengthmesh;
		xy [1] = -(tdata.j - newbase[1] + 0.5f) * tdata.lengthmesh;
		return xy;
	}

	public static int[] getij(int zoom, Vector2 xz){
		float lengthmesh = publicvar.lengthmesh / Mathf.Pow (2, zoom - publicvar.basezoom);
		float[] lonlat = XYToLonLat (publicvar.basei, publicvar.basej, publicvar.basezoom);
		int[] newbase = lnglatToXY (lonlat [0], lonlat [1],zoom);

		int[] result = new int[2];
		result [0] = (int)(xz [0] / lengthmesh - 0.5f + newbase[0]);
		result [1] = (int)(-xz [1] / lengthmesh - 0.5f + newbase[1]);
		return result;
	}

	public static int[] getCurrentTile(GameObject aircraft, int zoom){
		int[] ij = new int[2];
		float x = aircraft.transform.position.x ;
		float z = aircraft.transform.position.z ;
		ij [0] = (int)((x + publicvar.lengthmesh / 2) / publicvar.lengthmesh);
		ij [1] = (int)((z + publicvar.lengthmesh / 2) / publicvar.lengthmesh);
		ij [0] += publicvar.basei;
		ij [1] = publicvar.basej - ij [1];
		return ij;
	}
	
}

