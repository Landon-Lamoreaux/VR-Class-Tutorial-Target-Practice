using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehavior : MonoBehaviour
{
    [SerializeField]
    private bool useMass = false;
    [SerializeField]
    private bool holdForce = false;
    [SerializeField]
    private KeyCode onKey = KeyCode.None;
    [SerializeField]
    private float force = 700f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(onKey))
        {
            FireCannon();
        }
    }

    private ForceMode getForceMode()
    {
        if (useMass)
        {
            if (holdForce)
                return ForceMode.Force;
            else
                return ForceMode.Impulse;
        }
        else
        {
            if (holdForce)
                return ForceMode.Acceleration;
            else
        return ForceMode.VelocityChange;
        }
    }
    private void FireCannon()
    {
        Debug.Log("Fire " + getForceMode());

        // Load a new ball.
        GameObject temp = Resources.Load<GameObject>("Cannon Ball");

        // Height is cannon height plus have the ball size.
        float height = gameObject.transform.lossyScale.y + temp.transform.localScale.x / 2;
        // Place the ball.
        Vector3 up = gameObject.transform.up;
        Vector3 startingPos = height * up + gameObject.transform.position;
        GameObject ball = Instantiate(temp, startingPos, Quaternion.identity);
        
        // Apply force.
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        Vector3 force = this.force * up;
        rb.AddForce(force, getForceMode());
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Controller"))
        {
            FireCannon();
        }
    }

}
