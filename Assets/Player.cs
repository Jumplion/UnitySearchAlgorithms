using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SearchType
{
    BFS = 0,
    DFS = 1,
    UCS = 2,
    Greedy = 3,
    Astar = 4
}

public enum Direction
{
    North = 0,
    South = 1,
    East  = 2,
    West  = 3,
    NorthEast = 4,
    NorthWest = 5,
    SouthEast = 6,
    SouthWest = 7
}

public class Player : MonoBehaviour
{
    public bool allowDiagonal = false;
    private Toggle diagonalToggle;
    private int numDirections = 4;
    private int numSteps = 0;

    public static Player instance;

    [Range(0, 1000)]
    public uint stepSpeed = 100;

    public static Vector3 currentPosition = Vector3.zero;
    public static List<Vector2> checkedPositions;

    public SearchType searchType = SearchType.BFS;
    private Dropdown searchTypeDropdown;

    private void Awake()
    {
        instance = this;
        numDirections = allowDiagonal ? 4 : 8;
        searchTypeDropdown = GameObject.Find("Search Type: Dropdown").GetComponent<Dropdown>();
        diagonalToggle = GameObject.Find("Diagonal: Toggle").GetComponent<Toggle>();
    }

    public void SetSearchType()
    {
        searchType = (SearchType)searchTypeDropdown.value;
    }

    public void SetDiagonalMovement()
    {
        allowDiagonal = diagonalToggle.isOn;
    }

    public void StartSearch()
    {
        StopAllCoroutines();

        numSteps = 0;

        switch (searchType)
        {
            case SearchType.BFS:    StartCoroutine(BFS(Map.StartCrumb, Map.GoalCrumb));         break;
            case SearchType.DFS:    StartCoroutine(DFS(Map.StartCrumb, Map.GoalCrumb));         break;
            case SearchType.UCS:    StartCoroutine(UCS(Map.StartCrumb, Map.GoalCrumb));         break;
            case SearchType.Greedy: StartCoroutine(Greedy(Map.StartCrumb, Map.GoalCrumb));      break;
            case SearchType.Astar:  StartCoroutine(Astar(Map.StartCrumb, Map.GoalCrumb, true)); break;
            default: StartCoroutine(BFS(Map.StartCrumb, Map.GoalCrumb));                        break;
        }
    }

    /// <summary>
    /// Checks if there is a valid node in Direction d of the passed Vector3 v
    /// </summary>
    /// <param name="v">The main vector we are checking</param>
    /// <param name="d">The direction from the root vector v we are checking</param>
    /// <returns>If a valid vector is found, returns that. Otherwise, returns an invalid vector Vector3.down (we don't care about vectors in the Y direction for 2D searching)</returns>
    Breadcrumb CheckNode(Breadcrumb crumb, Direction d)
    {
        Breadcrumb result = null;
        int X = (int)crumb.Coordinates.x;
        int Y = (int)crumb.Coordinates.y;
        
        switch (d)
        {
            case Direction.North:       result = Map.GetCrumb(X, Y + 1);        break;
            case Direction.South:       result = Map.GetCrumb(X, Y - 1);        break;
            case Direction.East:        result = Map.GetCrumb(X + 1, Y);        break;
            case Direction.West:        result = Map.GetCrumb(X - 1, Y);        break;
            case Direction.NorthEast:   result = Map.GetCrumb(X + 1, Y + 1);    break;
            case Direction.NorthWest:   result = Map.GetCrumb(X - 1, Y + 1);    break;
            case Direction.SouthEast:   result = Map.GetCrumb(X + 1, Y - 1);    break;
            case Direction.SouthWest:   result = Map.GetCrumb(X - 1, Y - 1);    break;
        }
        return result;
    }

    IEnumerator BFS(Breadcrumb root, Breadcrumb goal)
    {
        List<Breadcrumb> expanded = new List<Breadcrumb>();
        Queue<Breadcrumb> q = new Queue<Breadcrumb>();

        q.Enqueue(root);

        Breadcrumb current = root;

        // Go through the nodes on the map
        while(q.Count > 0)
        {
            current = q.Dequeue();

            // Each time we visit a node, leave a breadcrub
            transform.position = current.transform.position;
            current.SetColor(Color.blue);
            numSteps++;
            yield return new WaitForSeconds( (float)stepSpeed / 1000);
            
            // If we found the goal we're done
            if (current.ID == goal.ID)
            {
                print("Found Goal");
                break;
            }

            if (!expanded.Contains(current))
            {
                expanded.Add(current);

                // Look at north, south, east, and west nodes. Diagonals are optional
                for (int x = 0; x < numDirections; x++)
                {
                    Breadcrumb neighbor = CheckNode(current, (Direction)x);

                    if (neighbor != null && !expanded.Contains(neighbor) && neighbor != current && !q.Contains(neighbor))
                    {
                        q.Enqueue(neighbor);

                        yield return new WaitForSeconds( (float)stepSpeed / 1000);
                    }
                }
            }      
        }

        yield return null;
    }

    IEnumerator DFS(Breadcrumb root, Breadcrumb goal)
    {
        List<Breadcrumb> expanded = new List<Breadcrumb>();
        Stack<Breadcrumb> s = new Stack<Breadcrumb>();

        s.Push(root);

        Breadcrumb current = root;

        // Go through the nodes on the map
        while (s.Count > 0)
        {
            current = s.Pop();

            // Each time we visit a node, leave a breadcrub
            transform.position = current.transform.position;
            current.SetColor(Color.blue);
            numSteps++;
            yield return new WaitForSeconds((float)stepSpeed / 1000);

            // If we found the goal we're done
            if (current.ID == goal.ID)
            {
                print("Found Goal");
                break;
            }

            if (!expanded.Contains(current))
            {
                expanded.Add(current);

                // Look at north, south, east, and west nodes. Diagonals are optional
                for (int x = 0; x < numDirections; x++)
                {
                    Breadcrumb neighbor = CheckNode(current, (Direction)x);

                    if (neighbor != null && !expanded.Contains(neighbor) && neighbor != current && !s.Contains(neighbor))
                    {
                        s.Push(neighbor);

                        yield return new WaitForSeconds((float)stepSpeed / 1000);
                    }
                }
            }
        }

        yield return null;
    }

    // Annoyingly, C# doesn't have Priority Queues...
    IEnumerator UCS(Breadcrumb root, Breadcrumb goal)
    {
        List<Breadcrumb> expanded = new List<Breadcrumb>();

        // Organizes by Breadcrumb heuristic. The lower the number, the higher the priority
        PriorityQueue<Breadcrumb> pq = new PriorityQueue<Breadcrumb>();

        pq.Enqueue(root);

        Breadcrumb current = root;

        // Go through the nodes on the map
        while (pq.Count() > 0)
        {
            current = pq.Dequeue();
            // Each time we visit a node, leave a breadcrub
            transform.position = current.transform.position;
            current.SetColor(Color.blue);
            numSteps++;
            yield return new WaitForSeconds((float)stepSpeed / 1000);

            // If we found the goal we're done
            if (current == goal)
            {
                print("Found Goal");
                break;
            }

            if (!expanded.Contains(current))
            {
                expanded.Add(current);

                // Look at north, south, east, and west nodes. Diagonals are optional
                for (int x = 0; x < numDirections; x++)
                {
                    Breadcrumb neighbor = CheckNode(current, (Direction)x);

                    if (neighbor != null && !expanded.Contains(neighbor) && neighbor != current && !pq.Contains(neighbor))
                    {
                        pq.Enqueue(neighbor);
   
                        yield return new WaitForSeconds((float)stepSpeed / 1000);
                    }
                }
            }
        }
        
        yield return null;
    }

    IEnumerator Greedy(Breadcrumb root, Breadcrumb goal)
    {
        print("Did not find the Goal");
        return null;
    }

    IEnumerator Astar(Breadcrumb root, Breadcrumb goal, bool consistentHeuristic)
    {
        // The set of nodes already evaluated.
        // Don't used expanded if the heuristic is MONOTONIC/CONSISTENT (see Wikipedia)
        // Basically, if the shortest path between two neighboring nodes is always the direct path between those nodes
        // then the heuristic is consistent. There isn't another path that goes through more nodes but has a smaller heuristic
        List<Breadcrumb> expanded = new List<Breadcrumb>();
        PriorityQueue<Breadcrumb> pq = new PriorityQueue<Breadcrumb>();
        pq.Enqueue(root);

        // For each node, which node it can most efficiently be reached from.
        Dictionary<Breadcrumb, Breadcrumb> cameFrom = new Dictionary<Breadcrumb, Breadcrumb>();

        // For each node, the cost of getting from the start node to that node.
        Dictionary<Breadcrumb, float> fromStartHeuristic = new Dictionary<Breadcrumb, float>();
        fromStartHeuristic[root] = 0;   // The cost of going from start to start is zero.

        // For each node, the total cost of getting from the start node to the goal by passing by that node. That value is partly known, partly heuristic.
        Dictionary<Breadcrumb, float> estimatedHeuristic = new Dictionary<Breadcrumb, float>();

        estimatedHeuristic[root] = root.Heuristic;

        Breadcrumb current = null;

        while(pq.Count() > 0)
        {
            current = pq.Dequeue();

            transform.position = current.transform.position;
            current.SetColor(Color.blue);
            numSteps++;
            yield return new WaitForSeconds((float)stepSpeed / 1000);

            if (current.ID == goal.ID)
            {
                print("Found Goal");
                break;
            }

            if (!expanded.Contains(current))
            {
                expanded.Add(current);

                // Check neighbors
                for (int x = 0; x < numDirections; x++)
                {
                    Breadcrumb neighbor = CheckNode(current, (Direction)x);

                    if(neighbor != null && neighbor != current)
                    {
                        // If it has consistent heuristics, then we have to check if we've already expanded the node
                        if (consistentHeuristic)
                        {
                            if (!expanded.Contains(neighbor))
                            {
                                float temp_gScore = fromStartHeuristic[current] + (current.Heuristic - neighbor.Heuristic);

                                if (!pq.Contains(neighbor)) // Discover a new node
                                    pq.Enqueue(neighbor);
                                else if (temp_gScore >= fromStartHeuristic[neighbor])
                                    continue;       // This is not a better path.

                                // This path is the best until now. Record it!
                                cameFrom[neighbor] = current;
                                fromStartHeuristic[neighbor] = temp_gScore;
                                estimatedHeuristic[neighbor] = fromStartHeuristic[neighbor] + neighbor.Heuristic;
                            }
                        }
                        // Otherwise, we don't need/don't want to check if we've already expanded the node, so let's check it
                        else
                        {
                            float tentative_gScore = fromStartHeuristic[current] + (current.Heuristic - neighbor.Heuristic);

                            if (!pq.Contains(neighbor))	// Discover a new node
                                pq.Enqueue(neighbor);
                            else if (tentative_gScore >= fromStartHeuristic[neighbor])
                                continue;		// This is not a better path.

                            // This path is the best until now. Record it!
                            cameFrom[neighbor] = current;
                            fromStartHeuristic[neighbor] = tentative_gScore;
                            estimatedHeuristic[neighbor] = fromStartHeuristic[neighbor] + neighbor.Heuristic;
                        }

                        yield return new WaitForSeconds((float)stepSpeed / 1000);
                    }
                }
            }
        }

        yield return null;
    }
}

