using UnityEngine;
using System.Collections;
using System;

public class TextureData
{

	public int zoom = publicvar.basezoom;
	public int i = 26969;
	public int j = 12419;
	public float lengthmesh = publicvar.lengthmesh;

	public TextureData(int zoom, int i, int j){
		this.zoom = zoom;
		this.i = i;
		this.j = j;
		this.lengthmesh = publicvar.lengthmesh/Mathf.Pow(2,zoom-publicvar.basezoom);
	}
	public TextureData(int i, int j){
		this.i = i;
		this.j = j;
	}
}



