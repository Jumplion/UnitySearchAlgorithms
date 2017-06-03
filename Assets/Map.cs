using UnityEngine;
using UnityEngine.UI;

public enum HeuristicType
{
    None = 0,
    DistanceFromGoal = 1,
    ID = 2,
    Random = 3
}

public class Map : MonoBehaviour
{
    public GameObject player, crumb;
    public Gradient heuristicGradient;
    public Color expandedColor, checkedColor, goalColor;
    private Renderer rend;
    private Vector3 center;

    public uint xSize, ySize;
    private static uint prevX, prevY;

    public static GameObject mapObj;
    public static Breadcrumb[,] breadcrumbs;
    public HeuristicType heuristicType = HeuristicType.None;
    private Dropdown heuristicTypeDropdown;

    private static Breadcrumb startCrumb , goalCrumb;
    public static Button startCrumbButton, goalCrumbButton;

    public static Breadcrumb StartCrumb
    {
        get { return startCrumb; }
        set
        {
            if(startCrumb != null)
                startCrumb.SetColor(Color.black);
            startCrumb = value;
            startCrumb.SetColor(Color.blue);

            Player.instance.transform.position = startCrumb.transform.position;
        }
    }
    public static Breadcrumb GoalCrumb
    {
        get { return goalCrumb; }
        set
        {
            if(goalCrumb != null)
                goalCrumb.SetColor(Color.black);
            goalCrumb = value;
            goalCrumb.SetColor(Color.green);
        }
    }
    public static bool wantSetStart, wantSetGoal;

    private void Awake()
    {
        rend = GameObject.Find("Map").GetComponent<Renderer>();
        heuristicTypeDropdown = GameObject.Find("Heuristic Type: Dropdown").GetComponent<Dropdown>();
    }

    // Use this for initialization
    void Start ()
    {
        startCrumbButton = GameObject.Find("Set Goal Button").GetComponent<Button>();
        goalCrumbButton = GameObject.Find("Set Start Button").GetComponent<Button>();
        mapObj = gameObject;

        prevX = xSize;
        prevY = ySize;

        ResizeMap();
        InitializeMap();
        InitializeHeuristics(heuristicType);
    }
	
	// FixedUpdate is called once per frame
	void FixedUpdate ()
    {
        if (prevX == xSize && prevY == ySize)
            return;

        ResizeMap();
        ResetMap();
        InitializeMap();
        InitializeHeuristics(heuristicType);      
    }

    void ResizeMap()
    {
        prevX = xSize;
        prevY = ySize;

        rend.material.mainTextureScale = new Vector2((float)xSize, (float)ySize);
        transform.localScale = new Vector3((float)xSize / 10, 1, (float)ySize / 10);
    }

    void ResetMap()
    {
        if (breadcrumbs == null)
            return;

        foreach (Breadcrumb b in breadcrumbs)
            if(b != null)
                b.DestroySelf();        
    }

    void InitializeMap()
    {
        breadcrumbs = new Breadcrumb[xSize, ySize];

        for (int x = 1; x < xSize; x++)
        {
            for (int z = 1; z < ySize; z++)
            {
                breadcrumbs[x, z] = Instantiate(crumb, new Vector3(x, 0, z), Quaternion.identity).GetComponent<Breadcrumb>();
                breadcrumbs[x, z].SetCoordinates(new Vector2(x, z));
            }
        }
    }

    public static Breadcrumb GetCrumb(int X, int Y)
    {
        if (X >= prevX || X < 1 || Y >= prevY || Y < 1 || breadcrumbs[X, Y].type == CrumbType.Block)
            return null;
        else
            return breadcrumbs[X, Y];
    }

    public void InitializeHeuristics(HeuristicType type)
    {
        foreach(Breadcrumb b in breadcrumbs)
        {
            if(startCrumb == null || goalCrumb == null || b == null)
                continue;

            if (b == goalCrumb)
            {
                b.Heuristic = 0;
                b.SetColor(heuristicGradient.Evaluate(0));
                continue;
            }

            switch (type)
            {
                case HeuristicType.None:
                        b.Heuristic = 0;
                        b.SetColor(heuristicGradient.Evaluate(0));
                    break;
                case HeuristicType.DistanceFromGoal:
                        float startGoalDistance = Vector2.Distance(startCrumb.Coordinates, goalCrumb.Coordinates);
                        float distance = Vector2.Distance(b.Coordinates, goalCrumb.Coordinates);
                        b.Heuristic = distance;
                        b.SetColor(heuristicGradient.Evaluate(distance / startGoalDistance));
                    break;
                case HeuristicType.ID:
                        b.Heuristic = b.ID;
                        b.SetColor(heuristicGradient.Evaluate(b.ID / Breadcrumb.numCrumbs));
                    break;
                case HeuristicType.Random:
                        float randomRange = Random.Range(0, Mathf.Max(xSize, ySize));
                        b.Heuristic = randomRange;
                        b.SetColor(heuristicGradient.Evaluate(randomRange/Mathf.Max(xSize,ySize)));
                    break;
            }
        }
    }

    public void SetStartCrumb(bool startSetting)
    {
        wantSetStart = startCrumbButton.interactable = startSetting;
        wantSetGoal = false;
        goalCrumbButton.interactable = true;

    }

    public void SetGoalCrumb(bool startSetting)
    {
        wantSetGoal = goalCrumbButton.interactable = startSetting;
        wantSetStart = false;
        startCrumbButton.interactable = true;
    }

    public void SetHeuristicType()
    {
        heuristicType = (HeuristicType)heuristicTypeDropdown.value;
    }
}
