using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LookAtTarget : MonoBehaviour
{
    //This Script is attached to the fixation target game object and validates where the user is looking during a test.
    //The variables below store necessary game objects and are used to store required variables
    [SerializeField]
    public FoveInterfaceBase FoveInterface;//Stores instance of the fove plugin class
    public GameObject averageCursor;
    private Collider this_targetCollider;
    private RaycastHit rayCollisionLeft, rayCollisionRight;
    private Vector3 cursorPointStore;
    private bool targetCollision;
    public bool TargetCollision { get { return targetCollision; } }
    public Canvas canvasClinician;
    private masterController masterControllerScript;
    private int whichEye; //Variable stores an integer which represents an eye to validate
    private bool correctEye; //True if user is looking with the correct eye
    public bool CorrectEye { get { return correctEye; } } //For use in the static and kinetic test scripts
    private float currentUnBlinkTime = 0; //Stores all of the blink durations
    private float currentBlinkTime = 0;
    private float currentBlinkTime2 = 0;
    private float blinkThreshold = 0.5f;
    void Start() // Use this for initialization
    {
        //get an instance of the master controller script and the target's collider
        this_targetCollider = GetComponent<Collider>();
        masterControllerScript = canvasClinician.GetComponent<masterController>();
    }
    void Update() // Update is called once per frame
    {
        //Cursor Position. Used to check a user's gaze direction. Store structure of eye gaze rays from the headset
        FoveInterfaceBase.EyeRays eyeRays = FoveInterface.GetGazeRays();
        Physics.Raycast(eyeRays.left, out rayCollisionLeft, Mathf.Infinity);
        Physics.Raycast(eyeRays.right, out rayCollisionRight, Mathf.Infinity);
        whichEye = masterControllerScript.WhichEye; //determine which eye should be used for validation. Gets value from the master controller script
        correctEye = true; //Set equal to true every frame. To be changed to false if conditions aren't met
        switch (whichEye) //Determine what to do depending on which eye is selected. 0=both, 1=left, 2=right
        {
            case 0: //Determine which eyes are open and set the cursor and eye ray collision position using the open eyes
                if ((FoveInterface.CheckEyesClosed() == Fove.Managed.EFVR_Eye.Neither) && (rayCollisionLeft.point != Vector3.zero && rayCollisionRight.point != Vector3.zero))
                {
                    cursorPointStore = rayCollisionLeft.point + (rayCollisionRight.point - rayCollisionLeft.point) / 2; //Set the cursor position variable
                    averageCursor.transform.position = cursorPointStore; // Set the actual position of the cursor
                }
                else if ((FoveInterface.CheckEyesClosed() == Fove.Managed.EFVR_Eye.Left) && (rayCollisionRight.point != Vector3.zero))
                {
                    cursorPointStore = rayCollisionRight.point;
                    averageCursor.transform.position = cursorPointStore;
                }
                else if ((FoveInterface.CheckEyesClosed() == Fove.Managed.EFVR_Eye.Right) && (rayCollisionLeft.point != Vector3.zero))
                {
                    cursorPointStore = rayCollisionLeft.point;
                    averageCursor.transform.position = cursorPointStore;
                }
                else
                {
                    averageCursor.transform.position = cursorPointStore; //Do nothing. Set cursor position to its current position from the last frame
                }
                break;
            case 1: // Do the same but only move the cursor using the left eye
                if ((FoveInterface.CheckEyesClosed() == Fove.Managed.EFVR_Eye.Right) && (rayCollisionLeft.point != Vector3.zero))
                {
                    currentBlinkTime2 = 0; //Reset the blink timer. This is used so the user can blink, but the eye state is still valid
                    cursorPointStore = rayCollisionLeft.point;
                    averageCursor.transform.position = cursorPointStore;
                }
                else
                {
                    averageCursor.transform.position = cursorPointStore;
                    currentBlinkTime2 += Time.deltaTime; //Increase the timer when the eyes are closed by the frame time delta
                    if (currentBlinkTime2 > blinkThreshold) //Determine if the eye has been closed for over half a second
                    {
                        correctEye = false; //Correct eye isn't being used because because the eyes have been closed for longer than the threshold time
                    }
                }
                break;
            case 2: // Do the same but only move the cursor using the right eye
                if ((FoveInterface.CheckEyesClosed() == Fove.Managed.EFVR_Eye.Left) && (rayCollisionRight.point != Vector3.zero))
                {
                    currentBlinkTime2 = 0;
                    cursorPointStore = rayCollisionRight.point;
                    averageCursor.transform.position = cursorPointStore;
                }
                else
                {
                    averageCursor.transform.position = cursorPointStore;
                    currentBlinkTime2 += Time.deltaTime;
                    if (currentBlinkTime2 > blinkThreshold)
                    {
                        correctEye = false;
                    }
                }
                break;
        }
        if (whichEye == 1 || whichEye == 2) //Determine if an eye test is being conducted, as a particular eye has been selected
        {
            //The gazecast method is inaccurate for two eyes (Limmitation of the current plugin), but works for a single eye (in this case)
            if (FoveInterface.Gazecast(this_targetCollider)) //Check if the gaze direction collides with this target object.
            {
                currentBlinkTime = 0; //Reset another blink timer, the ray cast from the eye has collided with this target and the colour of this target should be set to its default white colour
                targetCollision = true;
                transform.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

                currentUnBlinkTime += Time.deltaTime; //Increase the timer when the eyes are open by the frame time delta
                if (currentUnBlinkTime > blinkThreshold) // Check if eyes have been open for at least half a second
                {
                    averageCursor.GetComponent<MeshRenderer>().enabled = false; // Disable/hide the eye cursor
                }
            }
            else
            {
                currentUnBlinkTime = 0;// Opposite to the above statement. Reset the unblink timer
                currentBlinkTime += Time.deltaTime; //Increase the timer when the eyes are closed by the frame time delta
                if (currentBlinkTime > blinkThreshold) // Check if eyes have been closed for at least half a second
                {
                    targetCollision = false; //The ray cast from the eye has not collided with this target
                    transform.GetComponent<Renderer>().material.SetColor("_Color", Color.red); //Set colour of this target object to red
                    averageCursor.GetComponent<MeshRenderer>().enabled = true; // Enable/show the eye cursor
                }
            }
        } else
        {
            //Reset all of the timers as an eye test isn't currently being done
            currentBlinkTime = 0;
            currentBlinkTime2 = 0;
            currentUnBlinkTime = 0;
            averageCursor.GetComponent<MeshRenderer>().enabled = false;// Disable/hide the eye cursor
        }
    }
}