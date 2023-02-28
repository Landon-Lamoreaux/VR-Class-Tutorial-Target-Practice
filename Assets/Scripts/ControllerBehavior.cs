using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ControllerBehavior : MonoBehaviour
{
    private List<InputDevice> inputDevices = new List<InputDevice>();

    private bool first = false; //flag for the first time we see a device

    private bool isPressed;
    private bool isGripped;

    private Arc[] arcScripts;
    //private Grab[] grabScripts;

    [SerializeField]
    InputDeviceCharacteristics hand = InputDeviceCharacteristics.Right;

    // Start is called before the first frame update
    void Start()
    {
        arcScripts = GameObject.FindObjectsOfType<Arc>();
        //grabScripts = GameObject.FindObjectsOfType<Grab>();
    }

    void Update()
    {
        if (!first)
        {
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand, inputDevices);
        }

        foreach (InputDevice device in inputDevices)
        {
            if (inputDevices.Count == 1 || (device.characteristics & hand) > 0)
            {
                // Find features for this device.
                List<InputFeatureUsage> supportedFeatrures = new List<InputFeatureUsage>();
                device.TryGetFeatureUsages(supportedFeatrures);

                // For each feature, identify the type of it (bool, float, or Vector2), and print its state.

                bool state;
                device.TryGetFeatureValue(CommonUsages.triggerButton, out state);

                if (state)  // Is the button down on this frame? 
                {
                    if (!isPressed) // Was the button up on the last frame?
                    {
                        foreach (Arc a in arcScripts)
                        {
                            a.BeginPlace();
                        }
                    }
                }
                else // Is the button up on this frame? 
                {
                    if (isPressed)  // Is the button down on last frame?
                    {
                        foreach (Arc a in arcScripts)
                        {
                            a.EndPlace();
                        }
                    }
                }

                isPressed = state; // Update its state.

                device.TryGetFeatureValue(CommonUsages.gripButton, out state);

                if (state) // Was the button down on this frame?
                {
                    if (!isGripped)  // Is the button up on last frame?
                    {
                        // TODO grab
                    }
                }
                else // Was the button up on this frame?
                {

                    if (isGripped)  // Is the button up on this frame?
                    {
                        // TODO drop
                    }
                }

                isGripped = state; // Update its state.
            }
        }
    }
}
