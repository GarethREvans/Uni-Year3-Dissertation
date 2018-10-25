using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class testLightController_Kinetic : MonoBehaviour {

    //Look At Target Variables
    public GameObject TargetCubePrefab;
    private LookAtTarget lookAtTargetScript;
    private bool TargetCollision = true;
    private bool correctEye = true;
    string[] patientErrorMessages = { "Patient not looking at target.", "Patient not looking with correct eye." }; //Array of error messages which are used if the users eye isn't looking at the target with the correct eye

    //Look At Loader Variables
    public Canvas canvasClinician;
    private masterController masterControllerScript;
    private string testedEye;

    //In test objects and variables
    public GameObject RotateToEdgeParent;
    public Light activeLight;
    private pointDataStore currentKineticData;
    private bool lightMoving;

    // Use this for initialization
    void Start () {
        lookAtTargetScript = TargetCubePrefab.GetComponent<LookAtTarget>();
        masterControllerScript = canvasClinician.GetComponent<masterController>();
        lightMoving = false;
    }

    // Update is called once per frame
    void Update() {
        // If statements that determine which error messages to display in the error text object, based on boolean values from the look at target script
        if(!TargetCollision && !correctEye)
        {
            masterControllerScript.setErrorText(patientErrorMessages[0], patientErrorMessages[1]);
        } else if (!TargetCollision)
        {
            masterControllerScript.setErrorText(patientErrorMessages[0], "");
        } else if (!correctEye)
        {
            masterControllerScript.setErrorText("", patientErrorMessages[1]);
        } else
        {
            masterControllerScript.setErrorText("", "");
        }
        //Cancel the current stimulus if the backspace button has been pressed
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //Reset the clinician controls
            masterControllerScript.enableControls();
            //Reset spot light to original position
            transform.localEulerAngles = new Vector3(0, 0, 0);
            RotateToEdgeParent.transform.localEulerAngles = new Vector3(0, 0, 0);
            lightMoving = false;
            //Reset the ligt intensity and spot angle to 0, so it can't be seen
            activeLight.intensity = 0;
            activeLight.spotAngle = 0;
        }
        //Check if a stimulus is present in the scene
        if (lightMoving == true) {

            //Determine if the user is looking at the target wit the correct eye
            TargetCollision = lookAtTargetScript.TargetCollision;
            correctEye = lookAtTargetScript.CorrectEye;
            if (TargetCollision && correctEye)
            {
                //Rotate the parent of the light source towrds the fixation point using the speed set in the stimulus properties panel
                RotateToEdgeParent.transform.Rotate(Vector3.down * Time.deltaTime * masterControllerScript.CurrentSpeed, Space.Self);
                if(Input.GetKeyDown(KeyCode.Mouse0))
                {
                    //store results and reset light if user responds to light stimulus
                    storeCurrentPoint();
                    masterControllerScript.addPointToUIKinetic(currentKineticData);
                    masterControllerScript.enableControls();
                    activeLight.intensity = 0;
                    lightMoving = false;
                } else if((RotateToEdgeParent.transform.localRotation.eulerAngles.y < 3 && RotateToEdgeParent.transform.localRotation.eulerAngles.y > -3))
                {
                    //store results and reset light if not detected. The stimulus result will appear near the centre of the fixation point indicating a blind area of the visual field
                    storeCurrentPoint();
                    masterControllerScript.addPointToUIKinetic(currentKineticData); //Send instance of point data to the master script to be stored there
                    masterControllerScript.enableControls();
                    activeLight.intensity = 0;
                    lightMoving = false;
                }
            }
            else
            {
                //Reset spot light to original position if the user isn't looking at the target with the correct eye.
                transform.localEulerAngles = new Vector3(0, 0, 0);
                RotateToEdgeParent.transform.localEulerAngles = new Vector3(0, 0, 0);
                lightMoving = false;
                activeLight.intensity = 0;
                activeLight.spotAngle = 0;
                masterControllerScript.enableControls();

            }
        }
    }

    void storeCurrentPoint() //Store the data for the current stimulus in the scene by creating a new instance of the pointDataStore class and setting all of its variables accordingly
    {
        currentKineticData = new pointDataStore();

        currentKineticData.setIntensityN(masterControllerScript.CurrentIntensityN);
        currentKineticData.setIntensityL(masterControllerScript.CurrentIntensityL);
        currentKineticData.setlightSizeRNumerals(masterControllerScript.CurrentLightSizeRNumerals);

        currentKineticData.setIntensity(masterControllerScript.CurrentIntensity1 + masterControllerScript.CurrentIntensity2);
        currentKineticData.setDegreesPerSecond(masterControllerScript.CurrentSpeed);
        currentKineticData.setSizeOfLight(masterControllerScript.CurrentSize);
        currentKineticData.setLightAngleParentZ(transform.localRotation.eulerAngles.z);
        currentKineticData.setLightPositionY(RotateToEdgeParent.transform.localRotation.eulerAngles.y);

        //Reset spot light to original position
        transform.localEulerAngles = new Vector3(0, 0, 0);
        RotateToEdgeParent.transform.localEulerAngles = new Vector3(0,0,0);
    }

    public void addMovingLight()
    {
        //Rotate this game object around the z-axis by the amount specified in the angle input field  
        transform.Rotate(0, 0, masterControllerScript.CurrentKineticAngle);
        //Rotate this game objects child (which is the spotlight's parent object) to the edge of the perimetry bowl.
        //This means the light stimulus will travel in a straight trajectory towards the fixation target when the stimulus starts to move
        RotateToEdgeParent.transform.Rotate(0, 90, 0, Space.Self);

        //Set the spotlight's intensity using the equation stated in the design documentation
        activeLight.intensity = (4/Mathf.Pow(10,((masterControllerScript.CurrentIntensity1 + masterControllerScript.CurrentIntensity2)/10)));
        activeLight.spotAngle = masterControllerScript.CurrentSize * 32;// Set the current stimulus size, multiplying it by 32, to compensate for the distance the light is from the perimetry bowl
        lightMoving = true;
    }

    public class pointDataStore
    {
        //Variables to store the intensity labels and size of a stimulus
        private int intensityNumber;
        private char intensityLetter;
        private string lightSizeRNumerals;

        //Variables to store the raw values of the stimulus properties when the stimulus has been responded to
        private float intensity;
        private float degreesPerSecond;
        private float sizeOfLight;
        private float lightAngleParentZ;
        private float lightAngleY;

        //Allows the variables above to be set, and retrieved
        public void setIntensityN(int currentIntensityN) { intensityNumber = currentIntensityN; }
        public int getIntensityN() { return intensityNumber; }
        public void setIntensityL(char currentIntensityL) { intensityLetter = currentIntensityL; }
        public char getIntensityL() { return intensityLetter; }
        public void setlightSizeRNumerals(string currentlightSizeRNumerals) { lightSizeRNumerals = currentlightSizeRNumerals; }
        public string getlightSizeRNumerals() { return lightSizeRNumerals; }
        public void setIntensity(float currentIntensity){intensity = currentIntensity;}
        public float getIntensity(){return intensity;}
        public void setDegreesPerSecond(float currentDegreesPerSecond) { degreesPerSecond = currentDegreesPerSecond; }
        public float getDegreesPerSecond() { return degreesPerSecond; }
        public void setSizeOfLight(float currentSizeOfLight) { sizeOfLight = currentSizeOfLight; }
        public float getSizeOfLight() { return sizeOfLight; }
        public void setLightAngleParentZ(float currentLightAngleParentZ) { lightAngleParentZ = currentLightAngleParentZ; }
        public float getLightAngleParentZ() { return lightAngleParentZ; }
        public void setLightPositionY(float currentLightAngleY) { lightAngleY = currentLightAngleY; }
        public float getLightPositionY() { return lightAngleY; }
    }
}