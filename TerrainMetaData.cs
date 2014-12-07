using UnityEngine;
using System.Collections;

public class TerrainMetaData
{
	public float width = publicvar.lengthmesh;
	public float length = publicvar.lengthmesh;
	public float[] position = {0f, 0f};
	public float latmin = 31f;
	public float latmax = 32f;
	public float lonmin = 117f;
	public float lonmax = 118f;
	public int i;
	public int j;

	public TerrainMetaData(float width, float length){
		this.width = width;
		this.length = length;
	}
	public TerrainMetaData(){
	}

}

