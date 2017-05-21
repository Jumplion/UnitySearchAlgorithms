using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    public int size = 10;
    public Renderer rend;
    public Vector3 center;

    public static Vector3[,] map;

	// Use this for initialization
	void Start ()
    {
        map = new Vector3[size,size];

        for (int x = 1; x <= size; x++)
            for (int z = 1; z <= size; z++)
                map[x,z] = new Vector3(x, 0, z);
	}
	
	// FixedUpdate is called once per frame
	void FixedUpdate ()
    {
        rend.material.mainTextureScale = new Vector2(size, size);
        transform.localScale = new Vector3((float)size/10, 1, (float)size/10);

        center = new Vector3((float)size / 2, 0, (float)size / 2);
    }


}
