using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SearchType
{
    BFS,
    DFS,
    UCS,
    Greedy,
    Astar
}

public class Player : MonoBehaviour
{
    // Each *FRAME*
    public uint stepSpeed = 100;
    private uint t = 0;

    public GameObject crumb;

    public Vector2 currentPosition;
    public List<Vector2> checkedPositions;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        t++;

        if (t >= stepSpeed)
            t = 0;
        
        	
	}

    void StartSearch(SearchType sType)
    {

    }

    // Make coroutines for each of the search types.

}
