using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    public int size = 10;
    public Renderer rend;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        rend.material.mainTextureScale = new Vector2(size, size);
        transform.localScale = new Vector3((float)size/10, 1, (float)size/10);      	
	}
}
