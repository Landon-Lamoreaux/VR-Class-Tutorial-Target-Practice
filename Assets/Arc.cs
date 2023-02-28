using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Arc : MonoBehaviour
{
    [SerializeField]
    public int initCount = 30;
    [SerializeField]
    public float force = 10;
    [SerializeField]
    public float mass = 1;
    [SerializeField]
    public float lineSize = 0.1f;
    private LineRenderer line;
    private Vector3[] points;
    private GameObject item; // Added this to make item exist.

    // How close are the points, make smaller for smoother curves.
    [SerializeField]
    public float stepSize = 0.1f;

    [SerializeField]
    private float epsilon = 0.005f;
    [SerializeField]
    private GameObject toPlace;
    [SerializeField]
    private bool turnOffCollisionWhenPlacing = true;


    // Start is called before the first frame update.
    void Start() //test git
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = initCount;
        line.startWidth = lineSize;
        line.endWidth = lineSize;
        points = new Vector3[initCount];
        //make the object
        line.enabled = false;
    }

    // Update is called once per frame.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            BeginPlace();
        if (Input.GetKeyUp(KeyCode.Space))
            EndPlace();

        if (item != null)
        {
            UpdateLine();
            PlaceObject();
        }
    }

    private void UpdateLine()
    {
        // Find starting point based on the object this is on.
        Vector3 startPoint = gameObject.transform.forward * gameObject.transform.lossyScale.z / 2;
        Vector3 currentPoint = startPoint + gameObject.transform.position;

        // Calculate initial a and v based on force.
        Vector3 a = Physics.gravity;
        Vector3 v = gameObject.transform.forward * (force / mass);

        // Do Euler steps.
        for (int i = 0; i < initCount; i++)
        {
            points[i] = currentPoint;
            v = v + a * stepSize;
            currentPoint = currentPoint + v * stepSize;
        }

        // Give the line render these positions this frame.
        line.SetPositions(points);
    }

    private float offset = 0.05f;

    private void PlaceObject()
    {
        //turn off item in cause of no hit, and avoid colliding with it
        item.SetActive(false);
        //check each segment for a collision
        for (int i = 1; i < initCount - 1; i++)
        {
            //figure out ray, and check
            Ray r = new Ray(points[i - 1], points[i] - points[i - 1]);
            float distance = (points[i] - points[i - 1]).magnitude;
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, distance))
            {
                // Turn object back on.
                item.SetActive(true);
                //item.transform.position = new Vector3(hit.point.x, hit.point.y + offset, hit.point.z);

                // Place object on top of hit point.
                Vector3 curPos = item.transform.position;
                Vector3 move = new Vector3(hit.point.x - curPos.x, hit.point.y - curPos.y + offset, hit.point.z - curPos.z);
                item.transform.Translate(move);

                break;
            }
        }
    }

    public void BeginPlace()
    {
        item = Instantiate(toPlace);

        // Get bounds for placement.
        offset = item.GetComponent<Collider>().bounds.size.y / 2 + epsilon;
        line.enabled = true;
        if (turnOffCollisionWhenPlacing)
        {
            Collider body = item.GetComponent<Collider>();
            body.enabled = false;
        }
    }

    public void EndPlace()
    {
        Collider body = item.GetComponent<Collider>();
        body.enabled = true;
        item = null;
        line.enabled = false;
    }

}
