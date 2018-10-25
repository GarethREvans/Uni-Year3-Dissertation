using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testLightController_Static : MonoBehaviour
{
    //Look At Target Variables
    public GameObject TargetCubePrefab;
    private LookAtTarget lookAtTargetScript;
    private bool TargetCollision = true;
    private bool correctEye = true;
    string[] patientErrorMessages = { "Patient not looking at target.", "Patient not looking with correct eye." };

    //master script Variables
    public Canvas canvasClinician;
    private masterController masterControllerScript;
    private string testedEye;

    //In test objects and variables
    public GameObject RotateToPointParent;
    public Light activeLight; //Linear intensity loss

    private bool staticTestResume;
    private bool thresholdIntensity;
    private int thresholdArrayCount;
    private int currentStaticIntensity;
    private int intensityStep;
    private bool lightMoved;
    private bool waitingForUserResponse;


    private int randomQuarter;
    private int randomPoint;

    //Stores delays
    //light not appearing --> light appearing
    private float currentTimeBetweenLightAppearing;
    [Range(1, 3)]
    public float maxTimeBetweenLightAppearing;
    //light appearing --> light disappearing
    private float currentLightFlashDuration;
    public float maxTimeOfLightFlashDuration;
    //light disappearing --> UserResponse
    private float currentUserResponseDuration;
    [Range(1, 10)]
    public float maxUserResponseDuration;

    // Use this for initialization
    void Start()
    {
        lookAtTargetScript = TargetCubePrefab.GetComponent<LookAtTarget>();
        masterControllerScript = canvasClinician.GetComponent<masterController>();
        initialiseVariables();
    }

    // Update is called once per frame
    void Update()
    {
        // If statements that determine which error messages to display in the error text object, based on boolean values from the look at target script
        if (!TargetCollision && !correctEye)
        {
            masterControllerScript.setErrorText(patientErrorMessages[0], patientErrorMessages[1]);
        }
        else if (!TargetCollision)
        {
            masterControllerScript.setErrorText(patientErrorMessages[0], "");
        }
        else if (!correctEye)
        {
            masterControllerScript.setErrorText("", patientErrorMessages[1]);
        }
        else
        {
            masterControllerScript.setErrorText("", "");
        }

        if (Input.GetKeyDown(KeyCode.Backspace))//Call the cancel test method if the backspace button is pressed
        {
            initialiseVariables();
            cancelTest();
        }

        if (staticTestResume)//Determine if the static test has been resumed/started
        {
            TargetCollision = lookAtTargetScript.TargetCollision;
            correctEye = lookAtTargetScript.CorrectEye;
            if (TargetCollision && correctEye)//Determine if the user is looking at the target with the correct eye
            {
                if (currentTimeBetweenLightAppearing > maxTimeBetweenLightAppearing)//Determine if the threshold delay between lights appearing has been reached.
                {
                    if (thresholdIntensity == false)//Determine if the threshold intensities have been worked out
                    {
                        //The threshold is worked out by increasing and decreasing the stimulus intensity depending on whether the user has seen it. The step in change decreases until the threshold intensity is determined
                        if (determineEyeStatic()[thresholdArrayCount].getPointsLeft().Count == 15)//Determine if the current quarter has had its threshold worked out.
                        {
                            if (currentLightFlashDuration < maxTimeOfLightFlashDuration)//Determine if the threshold duration for the light appearing has been reached.
                            {
                                currentLightFlashDuration += Time.deltaTime;
                                if (lightMoved == false)//Deremine if the light has been presented to the user
                                {
                                    //Set the properties of the static light objects to present the stimulus to the user
                                    RotateToPointParent.transform.Rotate(determineEyeStatic()[thresholdArrayCount].getPointListStore()[7].getRotationValues()[0], determineEyeStatic()[thresholdArrayCount].getPointListStore()[7].getRotationValues()[1], 0);
                                    activeLight.intensity = (4 / Mathf.Pow(10, (currentStaticIntensity / 10)));
                                    lightMoved = true;
                                }
                            }
                            else
                            {
                                //Reset the static stimulus so it can't be seen
                                RotateToPointParent.transform.localEulerAngles = new Vector3(0, 0, 0);
                                activeLight.intensity = 0;
                                currentUserResponseDuration += Time.deltaTime;

                                if (waitingForUserResponse)//Determine which way light intensity is going. Determine if we are detecting user response or user not response -dB
                                {
                                    if (Input.GetKeyDown(KeyCode.Mouse0))//YES - Did see it TRUE
                                    {
                                        resetTimers();
                                        if (intensityStep == 1 || intensityStep == -1) {
                                            storeThresholdResult(thresholdArrayCount, 7, true);//Store the results
                                            waitingForUserResponse = true;
                                        }
                                        else
                                        {
                                            intensityStep = intensityStep / -2;//Inverse and half the intensity step
                                            waitingForUserResponse = !waitingForUserResponse;
                                        }
                                    }
                                    else if (currentUserResponseDuration > maxUserResponseDuration) //NO - Didn't see it TRUE
                                    {
                                        //start changing light intensity
                                        if (currentStaticIntensity > 0)
                                        {
                                            currentStaticIntensity = currentStaticIntensity + intensityStep;
                                        }
                                        else
                                        {
                                            storeThresholdResult(thresholdArrayCount, 7, true);//Store the results
                                            waitingForUserResponse = true;
                                        }
                                        resetTimers();
                                    }
                                }
                                else  //+dB
                                {
                                    if (Input.GetKeyDown(KeyCode.Mouse0))//YES - Did see it FALSE
                                    {
                                        //start changing light intensity
                                        if (currentStaticIntensity < 19)
                                        {
                                            currentStaticIntensity = currentStaticIntensity + intensityStep;
                                        }
                                        else
                                        {
                                            storeThresholdResult(thresholdArrayCount, 7, true); //Store the results
                                            waitingForUserResponse = true;
                                        }

                                        resetTimers();
                                    }
                                    else if (currentUserResponseDuration > maxUserResponseDuration) //NO - Didn't see it FALSE
                                    {
                                        resetTimers();
                                        if (intensityStep == 1 || intensityStep == -1) { storeThresholdResult(thresholdArrayCount, 7, true); waitingForUserResponse = true; }
                                        else
                                        {
                                            intensityStep = intensityStep / -2;//Inverse and half the intensity step
                                            waitingForUserResponse = !waitingForUserResponse;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Reset the light and timers
                            resetLight();
                            resetTimers();
                            thresholdArrayCount++;
                            if (thresholdArrayCount > 3)//Determine if the threshold tests have finished
                            {
                                thresholdIntensity = true;
                            }
                        }
                    }
                    else
                    {
                        //do test on other locations
                        if (currentLightFlashDuration < maxTimeOfLightFlashDuration)
                        {
                            currentLightFlashDuration += Time.deltaTime;
                            if (lightMoved == false)
                            {
                                //Select a random quarter and point to test and set the stimulus properties accordingly.
                                randomQuarter = determineEyeStaticLeftToUse()[UnityEngine.Random.Range(0, determineEyeStaticLeftToUse().Count)];
                                randomPoint = determineEyeStatic()[randomQuarter].getPointsLeft()[UnityEngine.Random.Range(0, determineEyeStatic()[randomQuarter].getPointsLeft().Count)];

                                RotateToPointParent.transform.Rotate(determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].getRotationValues()[0], determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].getRotationValues()[1], 0);
                                activeLight.intensity = (4 / Mathf.Pow(10, (float.Parse(determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].getThresholdIntensityStore()) / 10)));
                                currentStaticIntensity = int.Parse(determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].getThresholdIntensityStore());

                                lightMoved = true;
                            }
                        }
                        else
                        {
                            //Reset the stimulus properties
                            RotateToPointParent.transform.localEulerAngles = new Vector3(0, 0, 0);
                            activeLight.intensity = 0;
                            lightMoved = false;
                            currentUserResponseDuration += Time.deltaTime;
                            if (Input.GetKeyDown(KeyCode.Mouse0))//YES - Did see it TRUE
                            {
                                resetTimers();
                                storeThresholdResult(randomQuarter, randomPoint, false);
                            }
                            else if (currentUserResponseDuration > maxUserResponseDuration) //NO - Didn't see it TRUE
                            {
                                //start changing light intensity
                                currentStaticIntensity -= 1;
                                determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].setThresholdIntensityStore(currentStaticIntensity.ToString());//Set the threshold intensity of the user didn't see the 1dB dimmer light.
                                resetTimers();
                                if (currentStaticIntensity < 0)
                                {
                                    storeThresholdResult(randomQuarter, randomPoint, false);//Store the results. The user has a blind spot
                                }
                            }
                        }
                    }
                }
                else
                {
                    currentTimeBetweenLightAppearing += Time.deltaTime;
                    resetLight();
                }
            }
            else
            {
                //Reset the test state if the user isn't looking at the target with the correct eye.
                resetLight();
                resetTimers();
                currentStaticIntensity = 20;
                intensityStep = -4;
                waitingForUserResponse = true;
                randomQuarter = 0;
                randomPoint = 0;
            }
        }
    }

    void cancelTest()//Reset the test to its original state
    {
        masterControllerScript.enableControls();
        staticTestResume = false;
        resetLight();
        resetTimers();
        currentStaticIntensity = 20;
        intensityStep = -4;
        waitingForUserResponse = true;
        randomQuarter = 0;
        randomPoint = 0;
    }

    void storeThresholdResult(int randomQuarter, int randomPoint, bool threshold)
    {
        if (currentStaticIntensity < 0)
        {
            determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].setIntensityStore("b");//Set blind spot character if the intensity is graeter than its maximum value
        }
        else
        {
            determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].setIntensityStore(currentStaticIntensity.ToString());//Set current intensity otherwise
        }
        determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].getIntensityStoreText().text = determineEyeStatic()[randomQuarter].getPointListStore()[randomPoint].getIntensityStore();//Set the corresponding text object's text.
        determineEyeStatic()[randomQuarter].getPointsLeft().RemoveAll(x => x == randomPoint);//Remove the point from the point list.

        if (determineEyeStatic()[randomQuarter].getPointsLeft().Count == 0)//Remove the current quarter from the list if there are no points left to use
        {
            determineEyeStaticLeftToUse().RemoveAll(x => x == randomQuarter);
        }

        if (determineEyeStaticLeftToUse().Count <= 0)//End the test if there are no quarters left to use.
        {
            initialiseVariables();
            cancelTest();
        }


        if (threshold)
        {
            for (int i = 0; i < 15; i++)
            {
                determineEyeStatic()[randomQuarter].getPointListStore()[i].setThresholdIntensityStore(currentStaticIntensity.ToString());//Set the threshold values for every point in the current quarter from the result of the threshold test.
            }
            currentStaticIntensity = 20;//Reset the threshold test variables
            intensityStep = -4;
        }
        resetLight();
        randomQuarter = 0;
        randomPoint = 0;
    }

    void initialiseVariables()
    {
        //Set the default state for all of the variables in this script
        staticTestResume = false;
        thresholdIntensity = false;
        thresholdArrayCount = 0;

        resetLight();
        activeLight.spotAngle = 0.43f * 32; //spot angle size III
        currentStaticIntensity = 20;
        intensityStep = -4;

        resetTimers();
        waitingForUserResponse = true;
        randomQuarter = 0;
        randomPoint = 0;
    }

    void resetLight()
    {
        activeLight.intensity = 0;
        RotateToPointParent.transform.localEulerAngles = new Vector3(0, 0, 0);
        lightMoved = false;
    }

    void resetTimers()//Reset the timers used for various stages of a stimulus appearing in te scene
    {
        currentTimeBetweenLightAppearing = 0;
        currentLightFlashDuration = 0;
        currentUserResponseDuration = 0;
    }

    public void determineStaticTestState()// Method to determine the current test state
    {
        staticTestResume = masterControllerScript.StaticTestResume;
    }

    List<masterController.dB_quarterStore> determineEyeStatic()//Method returns the db store list for the current eye being tested, from the master controller script
    {
        if (masterControllerScript.CurrentEye == "Left")
        {
            return masterControllerScript.DB_StoreSL;
        }
        else
        {
            return masterControllerScript.DB_StoreSR;
        }
    }

    List<int> determineEyeStaticLeftToUse()//Method returns the quarters left to use list for the current eye being tested, from the master controller script
    {
        if (masterControllerScript.CurrentEye == "Left")
        {
            return masterControllerScript.QuartersLeftToUseStoreSL;
        }
        else
        {
            return masterControllerScript.QuartersLeftToUseStoreSR;
        }
    }

}