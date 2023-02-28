using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject closestObject;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grabbable")
        {
            Debug.Log("Closest object is: " + other.gameObject.name);
            closestObject = other.gameObject;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (closestObject != null && other != closestObject)
        {
            Debug.Log("No object in reach");
            closestObject = null;
        }
    }

    private Transform originalParent;
    private GameObject objectInHand;
    public void GrabItem(bool isLeft)
    {
        //wrong hand stop
        if (isLeft && !name.Contains("Left") || !isLeft && !name.Contains("Right"))
            return;
        if (closestObject != null)
        {
            //move object to be in hand
            objectInHand = closestObject;
            //parent to the hand, but save the original to place it back
            originalParent = objectInHand.transform.parent;
            objectInHand.transform.parent = this.transform;
            objectInHand.transform.position = transform.position;
            //turn off physics
            Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void DropItem(bool isLeft)
    {
        // Wrong hand stop.
        if (isLeft && !name.Contains("Left") || !isLeft && !name.Contains("Right"))
            return;
        if (objectInHand)
        {
            // Place object back into original parent.
            objectInHand.transform.parent = originalParent;
            // Turn on physics.
            Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            // Apply estimated physics.
            // Flip x and y due to vive controller orientation.
            objectInHand.GetComponent<Rigidbody>().velocity = new Vector3(velocityEstimate.x, velocityEstimate.y, velocityEstimate.z) / Time.fixedDeltaTime;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = aVelocityEstimate / Time.fixedDeltaTime;
        }
        objectInHand = null;
    }

    [SerializeField]
    private float updateRate = 0.8f;

    private Vector3 velocityEstimate;
    private Vector3 lastPos;
    private Vector3 lastRotation;
    private Vector3 aVelocityEstimate;
    private void FixedUpdate()
    {
        //skip calculation if not neccesary
        if (objectInHand != null)
        {
            velocityEstimate = (1 - updateRate) * velocityEstimate + updateRate * (transform.position - lastPos);
            lastPos = transform.position;
            aVelocityEstimate = (1 - updateRate) * aVelocityEstimate + updateRate * (transform.eulerAngles - lastRotation);
            lastRotation = transform.eulerAngles;
            Debug.Log(velocityEstimate / Time.fixedDeltaTime + " " + aVelocityEstimate / Time.fixedDeltaTime);
        }
    }

}
