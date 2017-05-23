using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public uint size = 10;
    private uint prevSize;

    public Renderer rend;
    public Vector3 center;

    public static GameObject mapObj;
    public static Vector3[,] map;
    public static Breadcrumb[,] breadcrumbs;
    public GameObject crumb;

	// Use this for initialization
	void Start ()
    {
        mapObj = gameObject;

        prevSize = size;

        InitializeMap();
	}
	
	// FixedUpdate is called once per frame
	void FixedUpdate ()
    {
        if(prevSize != size)
        {
            rend.material.mainTextureScale = new Vector2(size, size);
            transform.localScale = new Vector3((float)size/10, 1, (float)size/10);

            center = new Vector3((float)size / 2, 0, (float)size / 2);

            ResetMap();
            InitializeMap();

            prevSize = size;
        }
    }

    void ResetMap()
    {
        foreach (Breadcrumb b in breadcrumbs)
            if(b != null)
                b.DestroySelf();
    }

    void InitializeMap()
    {
        map = new Vector3[size, size];
        breadcrumbs = new Breadcrumb[size, size];

        for (int x = 1; x < size; x++)
        {
            for (int z = 1; z < size; z++)
            {
                map[x, z] = new Vector3(x, 0, z);
                breadcrumbs[x, z] = Instantiate(crumb, map[x, z], Quaternion.identity).GetComponent<Breadcrumb>();
            }
        }
    }
}
