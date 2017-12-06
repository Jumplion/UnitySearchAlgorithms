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
    public static Map instance;
    public uint xSize, ySize;
    private static uint prevX, prevY;

    public GameObject crumb;
    private Renderer rend;
    public Gradient heuristicGradient;
    public Color expandedColor, checkedColor, goalColor;

    public static GameObject mapObj;
    public static Breadcrumb[,] breadcrumbs;

    public HeuristicType heuristicType = HeuristicType.None;
    private Dropdown heuristicTypeDropdown;

    public static Button startCrumbButton, goalCrumbButton;
    private static Breadcrumb startCrumb , goalCrumb;
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
        instance = this;
        mapObj = gameObject;

        rend = GameObject.Find("Map").GetComponent<Renderer>();
        heuristicTypeDropdown = GameObject.Find("Heuristic Type: Dropdown").GetComponent<Dropdown>();
        startCrumbButton = GameObject.Find("Set Goal Button").GetComponent<Button>();
        goalCrumbButton = GameObject.Find("Set Start Button").GetComponent<Button>();

        startCrumb = goalCrumb = GetCrumb(1, 1);

        InitializeMap();
        InitializeHeuristics(heuristicType);
    }

    public void SetX(string xParse)
    {
        if (xParse == "" || xParse == null)
            return;

        int x = int.Parse(xParse);
        x = Mathf.Clamp(x, 2, 99);
        xSize = (uint)x;
    }
    public void SetY(string yParse)
    {
        if (yParse == "" || yParse == null)
            return;

        int y = int.Parse(yParse);
        y = Mathf.Clamp(y, 2, 99);
        ySize = (uint)y;
    }

    // FixedUpdate is called once per frame
    void FixedUpdate ()
    {
        if (prevX == xSize && prevY == ySize)
            return;

        InitializeMap();
        InitializeHeuristics(heuristicType);      
    }

    void InitializeMap()
    {
        StopAllCoroutines();
        prevX = xSize;
        prevY = ySize;

        rend.material.mainTextureScale = new Vector2(xSize, ySize);
        transform.localScale = new Vector3((float)xSize / 10, 1, (float)ySize / 10);

        if (breadcrumbs != null)
            foreach (Breadcrumb b in breadcrumbs)
                if (b != null)
                    b.DestroySelf();

        breadcrumbs = new Breadcrumb[xSize, ySize];

        Breadcrumb.numCrumbs = 0;

        for (int x = 1; x < xSize; x++)
        {
            for (int z = 1; z < ySize; z++)
            {
                breadcrumbs[x, z] = Instantiate(crumb, new Vector3(x, 0, z), Quaternion.identity).GetComponent<Breadcrumb>();
                breadcrumbs[x, z].SetCoordinates(new Vector2(x, z));
            }
        }

        if(startCrumb == null)
            startCrumb = GetCrumb(1, 1);
        if (goalCrumb == null)
            goalCrumb = GetCrumb(1, 1);
    }

    public void InitializeHeuristics(HeuristicType type)
    {
        StopAllCoroutines();

        if (breadcrumbs == null)
            return;

        foreach(Breadcrumb b in breadcrumbs)
        {
            if (b == null)
                continue;

            if (b == goalCrumb)
            {
                b.heuristic = 0;
                b.SetColor(Color.green);
                continue;
            }

            switch (type)
            {
                case HeuristicType.None:
                        b.heuristic = 0;
                        b.initialColor = heuristicGradient.Evaluate(0);
                        b.SetColor(heuristicGradient.Evaluate(0));
                    break;
                case HeuristicType.DistanceFromGoal:
                        float startGoalDistance = Vector2.Distance(startCrumb.coordinates, goalCrumb.coordinates);
                        float distance = Vector2.Distance(b.coordinates, goalCrumb.coordinates);
                        b.heuristic = distance;
                        b.initialColor = heuristicGradient.Evaluate(distance / startGoalDistance);
                        b.SetColor(heuristicGradient.Evaluate(distance / startGoalDistance));
                    break;
                case HeuristicType.ID:
                        b.heuristic = b.ID;
                        b.initialColor = heuristicGradient.Evaluate((float)b.ID / Breadcrumb.numCrumbs);
                        b.SetColor(heuristicGradient.Evaluate((float)b.ID / Breadcrumb.numCrumbs));
                    break;
                case HeuristicType.Random:
                        float randomRange = Random.Range(0, Mathf.Max(xSize, ySize));
                        b.heuristic = randomRange;
                        b.initialColor = heuristicGradient.Evaluate(randomRange / Mathf.Max(xSize, ySize));
                        b.SetColor(heuristicGradient.Evaluate(randomRange/Mathf.Max(xSize,ySize)));
                    break;
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

    public void SetStartCrumb(bool startSetting)
    {
        StopAllCoroutines();

        wantSetStart = startCrumbButton.interactable = startSetting;
        wantSetGoal = false;
        goalCrumbButton.interactable = true;

        InitializeHeuristics(heuristicType);
    }

    public void SetGoalCrumb(bool startSetting)
    {
        StopAllCoroutines();

        wantSetGoal = goalCrumbButton.interactable = startSetting;
        wantSetStart = false;
        startCrumbButton.interactable = true;

        InitializeHeuristics(heuristicType);
    }

    public void SetHeuristicType()
    {
        heuristicType = (HeuristicType)heuristicTypeDropdown.value;
        InitializeHeuristics(heuristicType);
    }
}
