using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class masterController : MonoBehaviour
{
    [SerializeField]
    //External headset camera object.
    public FoveInterfaceBase FoveInterface;

    //Clinician Control Panel panels
    public GameObject settingsPanel;
    public GameObject DetermineEyePanel;
    public GameObject DetermineTestPanel;
    public GameObject TestInfoPanel;
    public GameObject ControlsPanel;
    public GameObject KineticControlPanel;
    public GameObject StaticControlPanel;
    public GameObject PatientDetailsInputPanel;

    //Patient details panel
    public InputField patientNameInputField;
    public InputField patientAgeInputField;
    public Dropdown patientGenderDropDown;

    public Button confirmPatientDetailsBtn;

    //Clinician Test Info Panel
    public Button NewPatientBtn;
    public Button QuitApplicationBtn;
    public Text PatientNameText;
    public Text PatientAgeText;
    public Text PatientGenderText;
    public Text TodaysDateText;

    public Button KineticTestBtn;
    public Button StaticTestBtn;
    public Button LeftEyeBtn;
    public Button RightEyeBtn;

    public Button toSettingsPanelBtn;
    public Text CurrentStimuliText;
    public Button TestStateBtn;

    //clinician test grid Panel
    public Text patientErrorText;
    public Text backspaceText;
    //Kinetic grid panel objects
    public GameObject defaultImageParent;
    public Image defaultImage;
    public RawImage leftEyeGraph;
    public RawImage rightEyeGraph;
    public GameObject gridImagesParentLeftKinetic;
    public GameObject gridImagesParentRightKinetic;
    private Image newImage;
    public GameObject defaultLineRenderer;
    public GameObject lineRenderersParentLeft;
    public GameObject lineRenderersParentRight;
    private GameObject newLineRenderer;
    //Static grid panel objects
    public RawImage StaticDBGraph;
    public GameObject StaticDefaultTextParent;
    public Text StaticDefaultText;
    public GameObject dBTextParent;

    //Clinician controls
    private bool exportedGraph;
    float exportGraphCounter;
    public Button createImageBtn;
    public Text TestDurationText;
    private bool testStarted;
    private float testDuration;
    private TimeSpan tSpan;
    public Button PauseResumeTestBtn;
    private bool testResume;
    //Kinetic clinician controls and variables
    public InputField angleInputField;
    public Button enterAngleBtn;
    public Button removeLastImageButton;
    private string lastStimuliUsed;
    private imageAndAngleStore lastImageAndAngle;
    private string lastEyeUsed;
    public Button drawIsopterBtn;
    public GameObject StimuliScrollView;
    public GameObject scrollContent;
    public Text defaultStimuliText;
    //Array of 20 hexadecimal colours to be used to identify 20 unique stimuli. It is very unlikely this limit will be reached, but validation has been included to prevent more than 20 stimulus types from being used
    private string[] stimuliColours = { "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "FF7700", "FF0077", "77FF00", "7700FF", "00FF77", "0077FF", "777000", "770070", "707700", "700077", "007770", "007077", "000000", "777777" };
    //Static clinician controls and variables
    public Button startStaticDataCollectionBtn;
    private bool staticTestResume;
    public bool StaticTestResume { get { return staticTestResume; } }



    //Stored Values for the current session
    //Patient Info
    private string patientName;
    private string patientAge;
    private string patientGender;
    public string PatientName { get { return patientName; } }
    public string PatientAge { get { return patientAge; } }
    public string PatientGender { get { return patientGender; } }
    ///////////////////////////////////////////Kinetic
    //clinician settings panel variables
    public Slider intensitySlider_1_4;
    public Text intensityNumber;
    public Slider intensitySlider_a_e;
    public Text intensityLetter;
    public Slider speedSlider;
    public Text speedNumber;
    public Slider lightSizeSlider;
    public Text lightSizeNumber;
    public Button confirmSettingsBtn;
    //clinician settings panel stored values
    private int currentIntensityN;
    private char currentIntensityL;
    private float currentIntensity1;
    private float currentIntensity2;
    private float currentSpeed;
    private string currentLightSizeRNumerals;
    private float currentSize;
    //For use in other scripts
    public int CurrentIntensityN { get { return currentIntensityN; } }
    public char CurrentIntensityL { get { return currentIntensityL; } }
    public float CurrentIntensity1 { get { return currentIntensity1; } }
    public float CurrentIntensity2 { get { return currentIntensity2; } }
    public float CurrentSpeed { get { return currentSpeed; } }
    public string CurrentLightSizeRNumerals { get { return currentLightSizeRNumerals; } }
    public float CurrentSize { get { return currentSize; } }
    //Stimuli Used
    private string currentStimuli;
    private float currentKineticAngle;
    public float CurrentKineticAngle { get { return currentKineticAngle; } }


    //TestLights and components and test properties
    public GameObject TestLightStatic;
    private testLightController_Static staticScript;
    public GameObject TestLight_Kinetic;
    private testLightController_Kinetic kineticScript;

    //Stores current test type and eye, and are publicly accessible by other scripts when needed
    private string currentTest;
    private string currentEye;
    public string CurrentTest { get { return currentTest; } }
    public string CurrentEye { get { return currentEye; } }

    private int whichEye;
    public int WhichEye { get { return whichEye; } }
    //Kinetic Data Store - Object Oriented Programming approach
    private List<stimuliUsed> stimuliUseKL; //Stores a list of stimuli used for an eye
    private List<stimuliUsed> stimuliUseKR;
    class stimuliUsed
    {
        private string stimuli; //Stores the current stimuli string
        private List<imageAndAngleStore> gridImagesAndAngles; //Stores a list of images and angles associated with this stimulus
        private GameObject isopterLineRenderer; //Stores a line renderer to draw an isopter for this stimulus type
        private Text stimuliUsedListItemText; //Stores the text object used as a key to show what colour on the graph, represents what stimulus
        private string stimuliColour; //Stores the colour used to represent this stimulus on the graph
        //Set and get the above variables
        public void setStimuli(string currentStimuli) { stimuli = currentStimuli; }
        public string getStimuli() { return stimuli; }
        public void setGridImagesAndAngle(List<imageAndAngleStore> currentImageAndAngle) { gridImagesAndAngles = currentImageAndAngle; }
        public List<imageAndAngleStore> getGridImagesAndAngleStore() { return gridImagesAndAngles; }
        public void setIsopterRenderer(GameObject currentIsopterRenderer) { isopterLineRenderer = currentIsopterRenderer; }
        public GameObject getIsopterRenderer() { return isopterLineRenderer; }
        public void setstimuliUsedListItemText(Text currentStimuliUsedListItemText) { stimuliUsedListItemText = currentStimuliUsedListItemText; }
        public Text getstimuliUsedListItemText() { return stimuliUsedListItemText; }
        public void setStimuliColour(string currentStimuliColour) { stimuliColour = currentStimuliColour; }
        public string getStimuliColour() { return stimuliColour; }
    }
    class imageAndAngleStore
    {
        private Image gridImage; //Stores a grid image with all of its properties, which is generated when a stimulus has been responded to by the user
        private float imageAngle; //Stores the angle the stimulus approached the fixation target from
        //Set and get the above variables
        public void setgridImage(Image currentGridImage) { gridImage = currentGridImage; }
        public Image getGridImage() { return gridImage; }
        public void setImageAngle(float currentImageAngle) { imageAngle = currentImageAngle; }
        public float getImagesAngle() { return imageAngle; }
    }
    //Static Data Store - Object Oriented Programming approach
    private List<Text> dBTextList;
    private Vector2[] quadrantTextPositions;
    //Lists to store data about each quarter of the perimetry bowl as well as what quarters have points left to use in them
    private List<dB_quarterStore> dB_StoreSL;
    private List<int> quartersLeftToUseStoreSL;
    private List<dB_quarterStore> dB_StoreSR;
    private List<int> quartersLeftToUseStoreSR;
    public List<dB_quarterStore> DB_StoreSL { get { return dB_StoreSL; } }//Can be accessed in other scripts if necessary
    public List<int> QuartersLeftToUseStoreSL { get { return quartersLeftToUseStoreSL; } }
    public List<dB_quarterStore> DB_StoreSR { get { return dB_StoreSR; } }
    public List<int> QuartersLeftToUseStoreSR { get { return quartersLeftToUseStoreSR; } }

    public class dB_quarterStore
    {
        private List<dB_pointStore> pointListStore = new List<dB_pointStore>();// List of points in this quarter
        private List<int> pointsLeftToUseStore = new List<int>(); //List of points left to use
        //Set and get the above variables
        public void setPointListStore(List<dB_pointStore> currentPointStore) { pointListStore = currentPointStore; }
        public List<dB_pointStore> getPointListStore() { return pointListStore; }
        public void setPointsLeft(List<int> currentPointsLeft) { pointsLeftToUseStore = currentPointsLeft; }
        public List<int> getPointsLeft() { return pointsLeftToUseStore; }
    }

    public class dB_pointStore
    {
        private string intensityStore; //Store this point's intensity
        private Text intensityStoreText; //Text object on the clinicin graph associated with this position on the perimetry bowl
        private int[] rotationValues = { 0, 0 }; //Stores rotation values for the test's spot light
        private string thresholdIntensityStore; //Store this point's threshold intensity from the threshold test
        //Set and get the above variables
        public void setIntensityStore(string newIntensity) { intensityStore = newIntensity; }
        public string getIntensityStore() { return intensityStore; }
        public void setIntensityStoreText(Text newIntensityText) { intensityStoreText = newIntensityText; }
        public Text getIntensityStoreText() { return intensityStoreText; }
        public void setRotationValues(int[] newRotationValues) { rotationValues = newRotationValues; }
        public int[] getRotationValues() { return rotationValues; }
        public void setThresholdIntensityStore(string newThresholdIntensity) { thresholdIntensityStore = newThresholdIntensity; }
        public string getThresholdIntensityStore() { return thresholdIntensityStore; }
    }

    // Use this for initialization
    void Start()
    {
        //Add necessary click listeners methods to  the buttons of the application
        confirmPatientDetailsBtn.onClick.AddListener(patientDetailsClick);

        NewPatientBtn.onClick.AddListener(NewPatientClick);
        QuitApplicationBtn.onClick.AddListener(QuitApplication);

        KineticTestBtn.onClick.AddListener(KineticTestClick);
        StaticTestBtn.onClick.AddListener(StaticTestClick);
        LeftEyeBtn.onClick.AddListener(LeftEyeClick);
        RightEyeBtn.onClick.AddListener(RightEyeClick);
        toSettingsPanelBtn.onClick.AddListener(toSettingsPanelClick);
        confirmSettingsBtn.onClick.AddListener(confirmSettingsClick);
        TestStateBtn.onClick.AddListener(TestStateClick);
        createImageBtn.onClick.AddListener(createImageClick);

        //Light Settings
        intensitySlider_1_4.onValueChanged.AddListener(delegate { intensityNumberChange(); });
        intensitySlider_a_e.onValueChanged.AddListener(delegate { intensityLetterChange(); });
        speedSlider.onValueChanged.AddListener(delegate { speedChange(); });
        lightSizeSlider.onValueChanged.AddListener(delegate { lightSizeChange(); });

        //Kinetic Test buttons
        PauseResumeTestBtn.onClick.AddListener(PauseResumeTestClick);
        enterAngleBtn.onClick.AddListener(callAddMovingLightClick);
        removeLastImageButton.onClick.AddListener(removeLastIAAClick);
        drawIsopterBtn.onClick.AddListener(drawIsoptersClick);
        //Get reference to the static and kinetic scripts
        kineticScript = TestLight_Kinetic.GetComponent<testLightController_Kinetic>();
        staticScript = TestLightStatic.GetComponent<testLightController_Static>();
        startStaticDataCollectionBtn.onClick.AddListener(startDataCollectionClick);

        //Initialise all of the variables in the script as well as the clinician interface panels to their default state
        initialiseVariables();
        TestLightStatic.SetActive(false);
        TestLight_Kinetic.SetActive(false);
        DetermineTestPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        settingsPanel.SetActive(false);

    }


    //Update is called once per frame
    void Update()
    {
        //Set the date text to the current date
        TodaysDateText.text = "Today's Date: " + DateTime.Now.ToString("dd'/'MM'/'yyyy");
        if (ControlsPanel.activeSelf)
        {
            if (testResume)
            {
                //Start the test duration timer text when the test has begun and the controls panel is active
                testDuration += Time.deltaTime;
                tSpan = new TimeSpan(0, 0, (int)testDuration);
                TestDurationText.text = "Test Duration: " + string.Format("{0} : {1:00}", (int)tSpan.Minutes, tSpan.Seconds);
            }
        }
        if (exportedGraph)
        {
            //Timer is used to determine when to set the interface lements to their default state.
            //This is required because the screen capture requires some frames to complete, so the properties of the ui elements can't be reset instantly
            exportGraphCounter += Time.deltaTime;
            if (exportGraphCounter > 1)
            {
                if (CurrentTest == "Kinetic")
                {
                    //Reset all of the ui elements changed for the screen capture for the kinetic test
                    toSettingsPanelBtn.gameObject.SetActive(true);
                    foreach (Transform child in KineticControlPanel.gameObject.transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    KineticControlPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(290, 600);
                    StimuliScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 300);
                }
                else
                {
                    //Reset all of the ui elements changed for the screen capture for the static test
                    StaticControlPanel.SetActive(true);
                }
                //Reset all of the commonly shared ui elements for both tests
                NewPatientBtn.gameObject.SetActive(true);
                QuitApplicationBtn.gameObject.SetActive(true);
                TestStateBtn.gameObject.SetActive(true);
                patientErrorText.gameObject.SetActive(true);
                PauseResumeTestBtn.gameObject.SetActive(true);
                createImageBtn.gameObject.SetActive(true);
                exportedGraph = false;
                exportGraphCounter = 0;
            }
        }
    }

    //This method handles the validation of the patient details entered in to the patient's details panel.
    //It ensures each field has entered data and then stores this information using their associated variables.
    void patientDetailsClick()
    {
        patientName = patientNameInputField.text;
        patientAge = patientAgeInputField.text;
        patientGender = patientGenderDropDown.options[patientGenderDropDown.value].text;
        bool nameValid = false;
        bool ageValid = false;

        if (patientNameInputField.text.Length > 0)
        {
            nameValid = true;
        }

        if (patientAgeInputField.text.Length > 0)
        {
            ageValid = true;
        }

        if (nameValid && ageValid && patientGenderDropDown.value > 0)
        {
            PatientNameText.text = "Patient's Name: " + patientName;
            PatientAgeText.text = "Patient's Age: " + patientAge;
            PatientGenderText.text = "Patient's Gender: " + patientGender;

            patientNameInputField.text = null;
            patientAgeInputField.text = null;
            patientGenderDropDown.value = 0;
            PatientDetailsInputPanel.SetActive(false);
        }
    }

    //Reset the test environment and ui to the start-up state, ready for a new patient to be tested.
    void NewPatientClick()
    {
        initialiseVariables();
        PatientDetailsInputPanel.SetActive(true);
    }

    void QuitApplication()
    {
        //Quit Application
        Application.Quit();
    }

    void LeftEyeClick()// Set the colour property of the eyes buttons so that the left eye button is white, and the right eye button is grey.
    {
        LeftEyeBtn.GetComponent<Image>().color = Color.white;
        RightEyeBtn.GetComponent<Image>().color = Color.grey;
        currentEye = "Left"; //Set the current eye that is to be tested
        if (DetermineTestPanel.activeSelf)
        { }
        else
        {
            DetermineTestPanel.SetActive(true); //Enable the test type panel if it isn't already
        }
    }

    void RightEyeClick()// Set the colour property of the eyes buttons so that the right eye button is white, and the left eye button is grey
    {
        RightEyeBtn.GetComponent<Image>().color = Color.white;
        LeftEyeBtn.GetComponent<Image>().color = Color.grey;
        currentEye = "Right"; //Set the current eye that is to be tested
        if (DetermineTestPanel.activeSelf)
        { }
        else
        {
            DetermineTestPanel.SetActive(true); //Enable the test type panel if it isn't already
        }
    }

    void KineticTestClick()// Set the colour property of the test type buttons so that the kinetic test button is white, and the static test button is grey
    {
        KineticTestBtn.GetComponent<Image>().color = Color.white;
        StaticTestBtn.GetComponent<Image>().color = Color.grey;
        currentTest = "Kinetic";
        TestStateBtn.enabled = true; //Enable the test state button
    }

    void StaticTestClick()// Set the colour property of the test type buttons so that the static test button is white, and the kinetic test button is grey
    {
        StaticTestBtn.GetComponent<Image>().color = Color.white;
        KineticTestBtn.GetComponent<Image>().color = Color.grey;
        currentTest = "Static";
        TestStateBtn.enabled = true; //Enable the test state button
    }

    void confirmSettingsClick()//Set the current stimulus variables and text object when the stimulus panel confirm button is clicked
    {
        settingsPanel.SetActive(false);
        currentStimuli = currentLightSizeRNumerals + currentIntensityN + currentIntensityL + " " + currentSpeed;
        CurrentStimuliText.text = "Current Stimuli: " + currentStimuli + " deg/sec";
    }

    void toSettingsPanelClick()//Make the stimulus settings panel appear
    {
        settingsPanel.SetActive(true);
    }

    void TestStateClick() //Start the test if it hasn't been, or stop the test if it is currently being conducted
    {
        testStarted = !testStarted; //Invert the current test state
        moveHeadsetPosition();
        if (testResume) //Set the test pause/resume button to pause when the test is being stopped
        {
            PauseResumeTestClick();
        }
        if (testStarted)
        {
            //Set the left panel's elements to disabled if the test has been started
            LeftEyeBtn.enabled = false;
            RightEyeBtn.enabled = false;
            KineticTestBtn.enabled = false;
            StaticTestBtn.enabled = false;
            TestStateBtn.GetComponentInChildren<Text>().text = "Stop Test";
            ControlsPanel.SetActive(true);//Enable the right (controls) panel

            NewPatientBtn.GetComponent<Image>().color = Color.grey;//Set grey colours for, and disable the quit and new patient buttons
            QuitApplicationBtn.GetComponent<Image>().color = Color.grey;
            NewPatientBtn.enabled = false;
            QuitApplicationBtn.enabled = false;

            if (currentTest == "Kinetic")
            {
                TestLight_Kinetic.SetActive(true);// Enable all of the kinetic test game objects an ui elements
                KineticControlPanel.SetActive(true);
                if (currentEye == "Left")
                {
                    leftEyeGraph.enabled = true;
                    //Enable all of the grid image (plotted points) and line renderer parents
                    gridImagesParentLeftKinetic.SetActive(true);
                    lineRenderersParentLeft.SetActive(true);
                    foreach (stimuliUsed stimuliUsed in stimuliUseKL) //Enable the stimuli used text objects for this eye in the stimuli used list box
                    {
                        stimuliUsed.getstimuliUsedListItemText().gameObject.SetActive(true);
                    }
                    whichEye = 1; //Set this variable to 1 (=left)
                }
                else
                {
                    rightEyeGraph.enabled = true;
                    //Enable all of the grid image (plotted points) and line renderer parents
                    gridImagesParentRightKinetic.SetActive(true);
                    lineRenderersParentRight.SetActive(true);
                    foreach (stimuliUsed stimuliUsed in stimuliUseKR) // Enable the stimuli used text objects for this eye in the stimuli used list box
                    {
                        stimuliUsed.getstimuliUsedListItemText().gameObject.SetActive(true);
                    }
                    whichEye = 2; //Set this variable to 2(= right)
                }

            }
            else
            {
                //Enable the static test game objects
                TestLightStatic.SetActive(true);
                StaticControlPanel.SetActive(true);
                StaticDBGraph.enabled = true;
                dBTextParent.SetActive(true);
                setGridTextValues();
                if (currentEye == "Left")
                {
                    whichEye = 1; //Set this variable to 1(= left)
                }
                else
                {
                    whichEye = 2; //Set this variable to 2(= right)
                }
            }
        }
        else
        {
            //Reset all of the UI elements to their default state for when a test isn't being conducted
            testDuration = 0;
            TestDurationText.text = "Test Duration: -- : --";
            LeftEyeBtn.enabled = true;
            RightEyeBtn.enabled = true;
            KineticTestBtn.enabled = true;
            StaticTestBtn.enabled = true;
            TestStateBtn.GetComponentInChildren<Text>().text = "Start Test";
            leftEyeGraph.enabled = false;
            rightEyeGraph.enabled = false;
            gridImagesParentLeftKinetic.SetActive(false);
            gridImagesParentRightKinetic.SetActive(false);
            lineRenderersParentLeft.SetActive(false);
            lineRenderersParentRight.SetActive(false);
            KineticControlPanel.SetActive(false);
            StaticControlPanel.SetActive(false);
            ControlsPanel.SetActive(false);
            TestLight_Kinetic.SetActive(false);
            TestLightStatic.SetActive(false);
            foreach (stimuliUsed stimuliUsed in stimuliUseKL)
            {
                stimuliUsed.getstimuliUsedListItemText().gameObject.SetActive(false);
            }
            foreach (stimuliUsed stimuliUsed in stimuliUseKR)
            {
                stimuliUsed.getstimuliUsedListItemText().gameObject.SetActive(false);
            }
            whichEye = 0;
            intensityNumberChange();
            intensityLetterChange();
            speedChange();
            lightSizeChange();

            StaticDBGraph.enabled = false;


            NewPatientBtn.GetComponent<Image>().color = Color.white;
            QuitApplicationBtn.GetComponent<Image>().color = Color.white;
            NewPatientBtn.enabled = true;
            QuitApplicationBtn.enabled = true;
            dBTextParent.SetActive(false);
            setErrorText("", "");
        }
    }

    void setGridTextValues()
    {
        if (currentEye == "Left")
        {
            for (int h = 0; h < 4; h++)
            {
                for (int i = 0; i < 15; i++)
                {
                    dB_StoreSL[h].getPointListStore()[i].getIntensityStoreText().text = dB_StoreSL[h].getPointListStore()[i].getIntensityStore();//Set the text for the static test plot points to the intensity stored in the dBstore list for the left eye
                }
            }
        }
        else
        {
            for (int h = 0; h < 4; h++)
            {
                for (int i = 0; i < 15; i++)
                {
                    dB_StoreSR[h].getPointListStore()[i].getIntensityStoreText().text = dB_StoreSR[h].getPointListStore()[i].getIntensityStore(); //Set the text for the static test plot points to the intensity stored in the dBstore list for the right eye
                }
            }
        }
    }

    public void setErrorText(string targetCollisionText, string correctEyeText) //This method is used by the static and kinetic test controller scripts to determine the error text displayed
    {
        patientErrorText.text = targetCollisionText + " " + correctEyeText;
    }

    void moveHeadsetPosition() //This method moves the headset camera, so correct eye camera is aligned centrally to the fixation target depending on what eye is being tested.
    {
        if (!testStarted)
        {
            FoveInterface.transform.localPosition = new Vector3(0, 0, 0);
        }
        else if (currentEye == "Left")
        {
            FoveInterface.transform.localPosition = new Vector3(0.33f, 0, 0);
        }
        else
        {
            FoveInterface.transform.localPosition = new Vector3(-0.33f, 0, 0);
        }
    }

    void PauseResumeTestClick()// This method sets the kinetic or static controls appropriately when the test is resumed or paused
    {
        testResume = !testResume;
        if (currentTest == "Kinetic")
        {
            if (testResume)
            {
                PauseResumeTestBtn.GetComponentInChildren<Text>().text = "Pause Test";
                angleInputField.enabled = true;
                enterAngleBtn.enabled = true;
                patientErrorText.gameObject.SetActive(true);
            }
            else
            {
                PauseResumeTestBtn.GetComponentInChildren<Text>().text = "Resume Test";
                angleInputField.enabled = false;
                enterAngleBtn.enabled = false;
                patientErrorText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (testResume)
            {
                PauseResumeTestBtn.GetComponentInChildren<Text>().text = "Pause Test";
                patientErrorText.gameObject.SetActive(true);
                //static test button
                startStaticDataCollectionBtn.enabled = true;
            }
            else
            {
                PauseResumeTestBtn.GetComponentInChildren<Text>().text = "Resume Test";
                patientErrorText.gameObject.SetActive(false);
                //static test button
                startStaticDataCollectionBtn.enabled = false;
            }
        }

    }

    void createImageClick() //This method is used to create an screen capture of the current clinician interface.
    {
        if (testResume)
        {
            PauseResumeTestClick();
        }

        NewPatientBtn.gameObject.SetActive(false);//Hide the generic control elements and buttons
        QuitApplicationBtn.gameObject.SetActive(false);
        TestStateBtn.gameObject.SetActive(false);

        patientErrorText.gameObject.SetActive(false);
        PauseResumeTestBtn.gameObject.SetActive(false);
        createImageBtn.gameObject.SetActive(false);

        if (CurrentTest == "Kinetic")
        {            
            toSettingsPanelBtn.gameObject.SetActive(false);

            int childCount = KineticControlPanel.gameObject.transform.childCount;
            foreach (Transform child in KineticControlPanel.gameObject.transform)//Hide all of the child elements of the kinetic control panel, apart from the stimuli used list box and heading text object
            {
                if ((child != KineticControlPanel.gameObject.transform.GetChild(childCount - 1)) && (child != KineticControlPanel.gameObject.transform.GetChild(childCount - 2)))
                {
                    child.gameObject.SetActive(false);
                }
            }
            KineticControlPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(290, 600);//Resize the list box so that all of the stimuli used can be seen.
            StimuliScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 550);
        }
        else
        {
            StaticControlPanel.SetActive(false); //Hide the static control elements and buttons
        }
        //Screen capture the monitor and so the clinician interface
        ScreenCapture.CaptureScreenshot(@"..\" + patientName + "-" + patientAge + "-" + patientGender + "-" + currentTest + "-" + currentEye + "-" + DateTime.Now.ToString("yyyy'_'MM'_'dd") + "-" + DateTime.Now.ToString("HH_mm_ss") + ".png");

        exportedGraph = true;
    }

    public void enableControls()//This method disabled all of the buttons in the info and control panels so that the user can't click the buttons when responding to a stimulus
    {
        foreach (Transform child in TestInfoPanel.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Button>() != null)
                child.GetComponent<Button>().enabled = true;
        }
        foreach (Transform child in ControlsPanel.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Button>() != null)
                child.GetComponent<Button>().enabled = true;
        }
        if (currentTest == "Static") //Reset the static test state
        {
            staticTestResume = false;
            staticScript.determineStaticTestState();
        }
        backspaceText.gameObject.SetActive(false);//Hide the backspace text object

    }

    void initialiseVariables()//This method sets all of the variables stored in this script to their default states
    {
        //Destroy all of the grid images, and line renderers for both eyes, as well as the stimuli text in the list box
        foreach (Transform child in gridImagesParentLeftKinetic.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in gridImagesParentRightKinetic.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in lineRenderersParentLeft.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in lineRenderersParentRight.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in scrollContent.transform)
        {
            if (!(child.transform.name == defaultStimuliText.transform.name))
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        foreach (Transform child in dBTextParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        dBTextList = new List<Text>();
        initialiseQuadrantTextPositions();
        initialiseTwoStaticLists();
        dBTextParent.SetActive(false);
        defaultStimuliText.gameObject.SetActive(false);
        defaultImage.enabled = false;
        stimuliUseKL = new List<stimuliUsed>();
        stimuliUseKR = new List<stimuliUsed>();
        intensityNumberChange();
        intensityLetterChange();
        speedChange();
        lightSizeChange();
        leftEyeGraph.enabled = false;
        rightEyeGraph.enabled = false;
        TestStateBtn.enabled = false;
        currentStimuli = currentLightSizeRNumerals + currentIntensityN + currentIntensityL + " " + currentSpeed;
        CurrentStimuliText.text = "Current Stimuli: " + currentStimuli + "deg/sec";
        testStarted = false;
        testDuration = 0;
        testResume = false;
        angleInputField.enabled = false;
        enterAngleBtn.enabled = false;
        removeLastImageButton.GetComponent<Image>().color = Color.grey;
        removeLastImageButton.enabled = false;
        PatientDetailsInputPanel.SetActive(true);
        whichEye = 0;
        exportedGraph = false;
        exportGraphCounter = 0;

        backspaceText.gameObject.SetActive(false);

        TestDurationText.text = "Test Duration: -- : --";
        KineticControlPanel.SetActive(false);
        StaticControlPanel.SetActive(false);
        StaticDBGraph.enabled = false;
        TestLight_Kinetic.SetActive(false);
        TestLightStatic.SetActive(false);
        DetermineTestPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        staticTestResume = false;


    }
    //////////////////////////////////////////////////////////////////////////////////////////////////Kinetic Test methods

    public void intensityNumberChange()//This method sets current intensity variables for the numbered scale (5dB steps) and is executed when the corresponding slider game object is changed from the settings panel.
    {
        switch (int.Parse(intensitySlider_1_4.value.ToString()))
        {
            case 1:
                intensityNumber.text = "1";
                currentIntensity1 = 15f;
                currentIntensityN = 1;
                break;
            case 2:
                intensityNumber.text = "2";
                currentIntensity1 = 10f;
                currentIntensityN = 2;
                break;
            case 3:
                intensityNumber.text = "3";
                currentIntensity1 = 5f;
                currentIntensityN = 3;
                break;
            case 4:
                intensityNumber.text = "4";
                currentIntensity1 = 0f;
                currentIntensityN = 4;
                break;
        }
    }

    public void intensityLetterChange() //This method sets current intensity variables for the lettered scale (1dB steps) and is executed when the corresponding slider game object is changed from the settings panel.
    {
        switch (int.Parse(intensitySlider_a_e.value.ToString()))
        {
            case 1:
                intensityLetter.text = "a";
                currentIntensity2 = 4f;
                currentIntensityL = 'a';
                break;
            case 2:
                intensityLetter.text = "b";
                currentIntensity2 = 3f;
                currentIntensityL = 'b';
                break;
            case 3:
                intensityLetter.text = "c";
                currentIntensity2 = 2f;
                currentIntensityL = 'c';
                break;
            case 4:
                intensityLetter.text = "d";
                currentIntensity2 = 1f;
                currentIntensityL = 'd';
                break;
            case 5:
                intensityLetter.text = "e";
                currentIntensity2 = 0f;
                currentIntensityL = 'e';
                break;
        }
    }

    public void speedChange()//This method sets speed that the kinetic spot light should be rotated by, and is executed when the corresponding slider game object is changed from the settings panel.
    {
        if (currentTest == "Static")
        {
            speedNumber.text = 0.ToString();
            currentSpeed = 0;
        }
        else
        {
            speedNumber.text = (speedSlider.value).ToString();
            currentSpeed = speedSlider.value;
        }
    }

    public void lightSizeChange() //This method sets current stimulus size variables, and is executed when the corresponding slider game object is changed from the settings panel.
    {
        switch (int.Parse(lightSizeSlider.value.ToString()))
        {
            case 0:
                lightSizeNumber.text = "0";
                currentSize = 0.055f;
                currentLightSizeRNumerals = "0";
                break;
            case 1:
                lightSizeNumber.text = "I";
                currentSize = 0.11f;
                currentLightSizeRNumerals = "I";
                break;
            case 2:
                lightSizeNumber.text = "II";
                currentSize = 0.22f;
                currentLightSizeRNumerals = "II";
                break;
            case 3:
                lightSizeNumber.text = "III";
                currentSize = 0.43f;
                currentLightSizeRNumerals = "III";
                break;
            case 4:
                lightSizeNumber.text = "IV";
                currentSize = 0.86f;
                currentLightSizeRNumerals = "IV";
                break;
            case 5:
                lightSizeNumber.text = "V";
                currentSize = 1.72f;
                currentLightSizeRNumerals = "V";
                break;
        }
    }

    public void addPointToUIKinetic(testLightController_Kinetic.pointDataStore CurrentKineticData)// This mewthod adds an image to the kinetic graph
    {

        if (!(determineKineticEyeStimuli().FindIndex(o => o.getStimuli() == currentStimuli) > -1))//Determines if the current stimulus has been used before
        {
            //Create a new instance of stimuliUsed and sets its variables accordingly
            int stimuliCount = determineKineticEyeStimuli().Count;
            stimuliUsed tempStimuliUsed = new stimuliUsed();
            tempStimuliUsed.setStimuli(currentStimuli);
            List<imageAndAngleStore> newIAStore = new List<imageAndAngleStore>();//Create a new list of images and angles from the imageAndAngleStore class and sets its variables accordingly
            tempStimuliUsed.setGridImagesAndAngle(newIAStore);
            tempStimuliUsed.setStimuliColour(stimuliColours[stimuliCount]);
            Text tempText = Instantiate(defaultStimuliText, scrollContent.transform, false);
            tempText.name = (currentTest + currentEye);
            string tempTextString = "<size=28><color=#" + tempStimuliUsed.getStimuliColour() + ">\t\u220E</color></size> " + currentStimuli + " deg/sec";//Sets colour of square character to a colour from the hex colours array
            tempText.text = tempTextString;
            tempText.gameObject.SetActive(true);
            tempStimuliUsed.setstimuliUsedListItemText(tempText);
            determineKineticEyeStimuli().Add(tempStimuliUsed);
        }
        defaultImage.enabled = true;
        //SetPosition of marker on clinician ui grid
        defaultImageParent.transform.Rotate(0, 0, CurrentKineticData.getLightAngleParentZ());
        //y rotation needs to be used on x axis for position change
        defaultImage.rectTransform.localPosition = new Vector2(CurrentKineticData.getLightPositionY() * (352f / 90f), 0);

        //Place new marker in grid image array
        newImage = Instantiate(defaultImage, defaultImageParent.transform, false);
        newImage.name = (CurrentKineticData.getlightSizeRNumerals() + ";" + CurrentKineticData.getIntensityN() + ";" + CurrentKineticData.getIntensityL() + ";");
        newImage.transform.position = defaultImage.transform.position;
        imageAndAngleStore currentImageAndAngle = new imageAndAngleStore();
        currentImageAndAngle.setgridImage(newImage);
        currentImageAndAngle.setImageAngle(CurrentKineticData.getLightAngleParentZ());

        //Store the last stimuli and imageAngle used. So it can be removed if the clinician wants to
        lastStimuliUsed = currentStimuli;
        lastImageAndAngle = currentImageAndAngle;
        lastEyeUsed = currentEye;
        removeLastImageButton.GetComponent<Image>().color = Color.white;
        removeLastImageButton.enabled = true;

        //Add image and angle to the relevant stimuli Used object
        int currentIndex = determineKineticEyeStimuli().FindIndex(o => o.getStimuli() == currentStimuli);
        Color tempColor;
        if (ColorUtility.TryParseHtmlString(("#" + determineKineticEyeStimuli()[currentIndex].getStimuliColour()), out tempColor))
        {
            currentImageAndAngle.getGridImage().color = tempColor;
        }
        determineKineticEyeStimuli()[currentIndex].getGridImagesAndAngleStore().Add(currentImageAndAngle);

        if (currentTest == "Kinetic")
        {
            if (currentEye == "Left")//Place the new image as a child of the appropriate parent for the current eye
            {
                newImage.transform.SetParent(gridImagesParentLeftKinetic.transform);
            }
            else
            {
                newImage.transform.SetParent(gridImagesParentRightKinetic.transform);
            }
        }

        //Reset default positions
        defaultImageParent.transform.localEulerAngles = new Vector3(0, 0, 0);
        defaultImage.rectTransform.localPosition = new Vector2(0, 0);
        defaultImage.enabled = false;
    }

    void callAddMovingLightClick()//This method introduces a moving stimulus with the currently set parameters/settings
    {
        int stimuliCount = determineKineticEyeStimuli().Count;
        if (stimuliCount < 19)//Prevents the number of stimulus types from exceeding the number of colours in the hex colour array
        {
            if (angleInputField.text.Length > 0 && int.Parse(angleInputField.text) < 360)//Check if a valid integer has been entered
            {
                //Add a new moving stimulus to the scene, using the kinetic spot light and parent objects
                patientErrorText.text = "";
                currentKineticAngle = int.Parse(angleInputField.text);
                kineticScript.addMovingLight();

                //Disable all of the buttons in the info and control panels
                foreach (Transform child in TestInfoPanel.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<Button>() != null)
                        child.GetComponent<Button>().enabled = false;
                }
                foreach (Transform child in ControlsPanel.GetComponentsInChildren<Transform>())
                {
                    if (child.GetComponent<Button>() != null)
                        child.GetComponent<Button>().enabled = false;
                }
                backspaceText.gameObject.SetActive(true);//Enable the backspace text
            }
        }
    }


    void removeLastIAAClick()
    {
        if (lastImageAndAngle != null)//Check if there was a last stimulus used
        {
            //Remove the last image and angle store from the appropriate list (left or right eye) and remove the stimulus text from the list box if there are no images and angle in that stimuli used list
            if (lastEyeUsed == "Left")
            {
                Destroy(lastImageAndAngle.getGridImage());
                stimuliUseKL.Find(o => o.getStimuli() == lastStimuliUsed).getGridImagesAndAngleStore().Remove(lastImageAndAngle);
                if (stimuliUseKL.Find(o => o.getStimuli() == lastStimuliUsed).getGridImagesAndAngleStore().Count == 0)
                {
                    Destroy(stimuliUseKL.Find(o => o.getStimuli() == lastStimuliUsed).getstimuliUsedListItemText());
                    stimuliUseKL.Remove(stimuliUseKL.Find(o => o.getStimuli() == lastStimuliUsed));
                }
            }
            else
            {
                Destroy(lastImageAndAngle.getGridImage());
                stimuliUseKR.Find(o => o.getStimuli() == lastStimuliUsed).getGridImagesAndAngleStore().Remove(lastImageAndAngle);
                if (stimuliUseKR.Find(o => o.getStimuli() == lastStimuliUsed).getGridImagesAndAngleStore().Count == 0)
                {
                    Destroy(stimuliUseKR.Find(o => o.getStimuli() == lastStimuliUsed).getstimuliUsedListItemText());
                    stimuliUseKR.Remove(stimuliUseKR.Find(o => o.getStimuli() == lastStimuliUsed));
                }
            }
        }
        //Reset the last stimulus variables and disable the use of the remove last stimulus button
        lastStimuliUsed = "";
        lastImageAndAngle = null;
        lastEyeUsed = "";
        removeLastImageButton.GetComponent<Image>().color = Color.grey;
        removeLastImageButton.enabled = false;
    }

    List<stimuliUsed> determineKineticEyeStimuli()//Return the appropriate stimuli used list depending on the current eye
    {
        if (currentEye == "Left")
        {
            return stimuliUseKL;
        }
        else
        {
            return stimuliUseKR;
        }
    }

    GameObject determineLineRendererParent() //Return the appropriate line renderer list depending on the current eye
    {
        if (currentEye == "Left")
        {
            return lineRenderersParentLeft;
        }
        else
        {
            return lineRenderersParentRight;
        }
    }

    void drawIsoptersClick()// This method instantiates a line renderer for each stimulus type used for the current eye, and fills it with an array of the associated image positions. These act as the isopters.
    {
        foreach (Transform child in determineLineRendererParent().transform)//Destroy the existing line renderers
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < determineKineticEyeStimuli().Count; i++)//Determine if there are any stimuli present, and instantiate a new line renderer for each stimulus type stored.
        {
            determineKineticEyeStimuli()[i].setGridImagesAndAngle(determineKineticEyeStimuli()[i].getGridImagesAndAngleStore().OrderBy(o => o.getImagesAngle()).ToList());
            newLineRenderer = Instantiate(defaultLineRenderer, determineLineRendererParent().transform, false);
            newLineRenderer.name = (determineKineticEyeStimuli()[i].getStimuli());
            newLineRenderer.transform.SetParent(determineLineRendererParent().transform);
            Vector3[] thisStimuliPositions = new Vector3[determineKineticEyeStimuli()[i].getGridImagesAndAngleStore().Count];
            for (int j = 0; j < determineKineticEyeStimuli()[i].getGridImagesAndAngleStore().Count; j++)//count how many stimuli are to be included in this isopter/line renderer
            {
                thisStimuliPositions[j] = determineKineticEyeStimuli()[i].getGridImagesAndAngleStore()[j].getGridImage().transform.localPosition;
                thisStimuliPositions[j].z = -0.1f;
            }
            //Set the positions of the new line renderer
            newLineRenderer.GetComponent<LineRenderer>().positionCount = thisStimuliPositions.Count();
            newLineRenderer.GetComponent<LineRenderer>().SetPositions(thisStimuliPositions);

            Color tempColor;
            if (ColorUtility.TryParseHtmlString(("#" + determineKineticEyeStimuli()[i].getStimuliColour()), out tempColor))//Set the colour of the new line renderer
            {
                newLineRenderer.GetComponent<Renderer>().material.color = tempColor;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////Static Test methods

    void initialiseQuadrantTextPositions()// This method sets 2 values (x and y) in an array of size 15. This is used to set the positions and rotation values of the decibel valuues, and static stimuli during the test
    {
        quadrantTextPositions = new Vector2[15];
        int height = 0;
        int widthMax = 0;
        int arrayCount = 0;

        for (int i = 4; i >= height; i--)//This for loop combination creates the triangle shaped structure of the decibel values for 1 quarter. This will be rotated 4 times to create a full grid when the text objects are created.
        {

            for (int j = 0; j <= widthMax; j++)
            {
                quadrantTextPositions[arrayCount] = new Vector2((29.4f + (58.8f * j)), (29.4f + (58.8f * i)));//The numbers used, come from the dividing the graph up based on its pixel width and height
                arrayCount++;
            }
            widthMax++;
        }

        StaticDefaultText.enabled = true;
        Text newText;
        float amountToRotate = 0;
        foreach (Transform child in dBTextParent.transform)//Destroy the existing decibel text objects
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < 4; i++)//This for loop combination instantiates a total of 60 text objects between 4 quarters (15 each) in a triangle structure.
        {
            amountToRotate = i * -90;
            StaticDefaultTextParent.transform.localRotation = Quaternion.Euler(0, 0, amountToRotate);
            for (int j = 0; j < 15; j++)
            {
                //add new text object and set string to "-"
                StaticDefaultText.rectTransform.localPosition = new Vector2(quadrantTextPositions[j].x, quadrantTextPositions[j].y); //Instantiate a new text object from the default one
                newText = Instantiate(StaticDefaultText, StaticDefaultTextParent.transform, false);
                newText.name = "static" + i + " " + j + "Text";
                newText.transform.position = StaticDefaultText.transform.position;
                newText.transform.SetParent(dBTextParent.transform);
                newText.text = "-";
                dBTextList.Add(newText);

                StaticDefaultText.rectTransform.localPosition = new Vector2(0, 0);
            }
        }
        //Reset the default text parent's rotation
        StaticDefaultTextParent.transform.localRotation = Quaternion.Euler(0, 0, 0);
        StaticDefaultText.enabled = false;
    }

    void initialiseTwoStaticLists()
    {
        //Initialise the cretaion of the static data store lists and quarters left to use lists for each eye.
        dB_StoreSL = new List<dB_quarterStore>();
        quartersLeftToUseStoreSL = new List<int>();
        dB_StoreSR = new List<dB_quarterStore>();
        quartersLeftToUseStoreSR = new List<int>();
        initialiseStaticList("Left");
        initialiseStaticList("Right");
    }

    void initialiseStaticList(string eyeText)//Crete the decibel store lists
    {
        List<dB_quarterStore> current_dB_Store = new List<dB_quarterStore>();
        List<int> currentQuartersLeft = new List<int>();

        dB_quarterStore newDBQuarterStore;
        int newQuarterLeft;

        dB_pointStore newDBPointStore;
        int newDBPointLeft;

        for (int h = 0; h < 4; h++)
        {
            //add new quarter store
            newDBQuarterStore = new dB_quarterStore();
            newDBQuarterStore.setPointListStore(new List<dB_pointStore>());
            newDBQuarterStore.setPointsLeft(new List<int>());
            for (int i = 0; i < 15; i++)
            {
                //add new point store
                newDBPointStore = new dB_pointStore();

                newDBPointStore.setIntensityStore("-");
                newDBPointStore.setIntensityStoreText(dBTextList[i + (15 * h)]); // get child of specific type Text
                newDBPointStore.getIntensityStoreText().transform.localRotation = Quaternion.Euler(0, 0, 0);

                switch (h)//Set the rotation values depending on which quarter the point is from. The top right and bottom left quarters have the x and y quadrant positions swapped around because these quadrants had their text positions set using rotation, so the values change value in the wrong direction.
                {
                    case 0://Set the rotation values
                        newDBPointStore.setRotationValues(new int[2] { ((int)Math.Round((quadrantTextPositions[i].y - 29.4f) / 58.8f) * -6) + (-6 / 2), ((int)Math.Round((quadrantTextPositions[i].x - 29.4f) / 58.8f) * 6) + (6 / 2) });
                        break;
                    case 1:
                        newDBPointStore.setRotationValues(new int[2] { ((int)Math.Round((quadrantTextPositions[i].x - 29.4f) / 58.8f) * 6) + (6 / 2), ((int)Math.Round((quadrantTextPositions[i].y - 29.4f) / 58.8f) * 6) + (6 / 2) });
                        break;
                    case 2:
                        newDBPointStore.setRotationValues(new int[2] { ((int)Math.Round((quadrantTextPositions[i].y - 29.4f) / 58.8f) * 6) + (6 / 2), ((int)Math.Round((quadrantTextPositions[i].x - 29.4f) / 58.8f) * -6) + (-6 / 2) });
                        break;
                    case 3:
                        newDBPointStore.setRotationValues(new int[2] { ((int)Math.Round((quadrantTextPositions[i].x - 29.4f) / 58.8f) * -6) + (-6 / 2), ((int)Math.Round((quadrantTextPositions[i].y - 29.4f) / 58.8f) * -6) + (-6 / 2) });
                        break;
                }

                newDBPointStore.setThresholdIntensityStore("0");

                newDBQuarterStore.getPointListStore().Add(newDBPointStore);

                //add new point left
                newDBPointLeft = i;
                newDBQuarterStore.getPointsLeft().Add(newDBPointLeft);
            }
            current_dB_Store.Add(newDBQuarterStore);

            //add new quarter left
            newQuarterLeft = new int();
            newQuarterLeft = h;
            currentQuartersLeft.Add(newQuarterLeft);
        }

        if (eyeText == "Left")//Determine which decibel stores should be set to the new lists created, depending on the current eye
        {
            dB_StoreSL = current_dB_Store;
            quartersLeftToUseStoreSL = currentQuartersLeft;
        }
        else
        {
            dB_StoreSR = current_dB_Store;
            quartersLeftToUseStoreSR = currentQuartersLeft;
        }
    }

    void startDataCollectionClick()
    {
        //Disable the info and control panels as the test is about to start
        foreach (Transform child in TestInfoPanel.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Button>() != null)
                child.GetComponent<Button>().enabled = false;
        }
        foreach (Transform child in ControlsPanel.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<Button>() != null)
                child.GetComponent<Button>().enabled = false;
        }
        patientErrorText.text = "";
        if (currentEye == "Left")//Re initialise the decibel lists depending on the current eye. This setup means that a new test is started every time.
        {
            dB_StoreSL = new List<dB_quarterStore>();
            quartersLeftToUseStoreSL = new List<int>();
            initialiseStaticList("Left");
        }
        else
        {
            dB_StoreSR = new List<dB_quarterStore>();
            quartersLeftToUseStoreSR = new List<int>();
            initialiseStaticList("Right");
        }
        setGridTextValues();//Set the grid text value to the appropriate decibel store list and resume the static test
        staticTestResume = true;
        staticScript.determineStaticTestState();
        backspaceText.gameObject.SetActive(true);
    }
}
