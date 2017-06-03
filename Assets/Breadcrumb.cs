using UnityEngine;
using System;

public enum CrumbType
{
    None = 0,
    Block = 1
}

public class Breadcrumb : MonoBehaviour, IComparable<Breadcrumb>
{
    Renderer rend;

    public static int numCrumbs = 0;
    public readonly int ID;

    public Vector2 Coordinates { get { return coordinates; } }
    private Vector2 coordinates = Vector2.zero;
    public float Heuristic { get { return heuristic; } set { heuristic = value; } }
    private float heuristic = Mathf.Infinity;

    public Color initialColor;

    public CrumbType type = CrumbType.None;

    Breadcrumb()
    {
        numCrumbs++;
        ID = numCrumbs;
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start ()
    {
        SetColor(Color.black);
        transform.parent = Map.mapObj.transform;
	}

    public void SetColor(Color col)
    {
        rend.material.color = col;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetCoordinates(Vector2 v)
    {
        coordinates = v;
    }

    // Highlight the breadcrumb (when not running)
    public void OnMouseEnter()
    {
        SetColor(Color.red);
    }

    public void OnMouseExit()
    {
        if (this == Map.StartCrumb)
            SetColor(Color.blue);
        else if (this == Map.GoalCrumb)
            SetColor(Color.green);
        else
            SetColor(Color.black);
    }

    public void OnMouseUp()
    {
        if (Map.wantSetStart)
            Map.StartCrumb = this;

        else if (Map.wantSetGoal)
            Map.GoalCrumb = this;

        Map.wantSetStart = false;
        Map.wantSetGoal = false;
    }

    public int CompareTo(Breadcrumb other)
    {
        return heuristic.CompareTo(other.heuristic);
    }
}
