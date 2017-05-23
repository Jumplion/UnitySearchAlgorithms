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

public enum Direction
{
    North,
    South,
    East,
    West
}

public class Player : MonoBehaviour
{
    // Each *FRAME*
    // 1000 = 1 second
    [Range(0, 1000)]
    public uint stepSpeed = 100;

    public Vector3 currentPosition = Vector3.zero;
    public List<Vector2> checkedPositions;


	// Use this for initialization
	void Start ()
    {
		
	}

    void StartSearch(SearchType sType)
    {

    }

    Vector3 CheckNode(Vector3 v, Direction d)
    {
        Vector3 result = Vector3.down;

        switch (d)
        {
            case Direction.North:
                if (Map.map[(int)v.x, (int)v.z + 1] != null)
                    result = new Vector3(v.x, 0, v.z + 1);
                break;
            case Direction.South:
                if (Map.map[(int)v.x, (int)v.z - 1] != null)
                    result = new Vector3(v.x, 0, v.z - 1);
                break;
            case Direction.East:
                if (Map.map[(int)v.x+1, (int)v.z] != null)
                    result = new Vector3(v.x + 1, 0, v.z);
                break;
            case Direction.West:
                if (Map.map[(int)v.x-1, (int)v.z] != null)
                    result = new Vector3(v.x - 1, 0, v.z);
                break;
        }


        return result;
    }

    IEnumerator BFS(Vector3 root, Vector3 goal)
    {
        List<Vector3> expanded = new List<Vector3>();
        Queue<Vector3> q = new Queue<Vector3>();

        expanded.Add(root);
        q.Enqueue(root);

        Vector3 current = root;

        // Go through the nodes on the map
        while(q.Count > 0)
        {
            current = q.Dequeue();
            expanded.Add(current);

            // Each time we visit a node, leave a breadcrub
            Map.breadcrumbs[(int)current.x, (int)current.z].SetCrumb(true, Color.blue);
            yield return new WaitForSeconds(stepSpeed / 1000);

            // If we found the goal we're done
            if (current == goal)
                StopCoroutine("BFS");

            // Look at north, south, east, and west nodes
            // Adds the child node if it returns a valid vector
            // and if it is not already expanded

            // Every time we visit a child, leave a breadcrumb
            for (int x = 0; x < 4; x++)
            {
                Vector3 child = CheckNode(current, (Direction)x);

                if (child != Vector3.down && !expanded.Contains(child))
                {
                    q.Enqueue(child);

                    // Leave breadcrumb
                    Map.breadcrumbs[(int)current.x, (int)current.z].SetCrumb(true, Color.red);
                    yield return new WaitForSeconds(stepSpeed / 1000);
                }
            }      
        }

        yield return null;
    }

    IEnumerator DFS()
    {

        return null;
    }

    IEnumerator UCS()
    {

        return null;
    }

    IEnumerator Greedy()
    {

        return null;
    }

    IEnumerator Astar()
    {

        return null;
    }

}
