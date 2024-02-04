using KSP.Game;
using KSP.Sim;
using KSP.Sim.impl;
using KSP.UI.Binding;
using LandingBuddyProj.Unity.Runtime;
using Steamworks;
using UitkForKsp2.API;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace LandingBuddyProj.UI;


/// <summary>
/// Controller for the MyFirstWindow UI.
/// </summary>

// The public class is being defined here. I need to point it to elements inside of my new GUI!
public class MyFirstWindowController : KerbalMonoBehaviour
{
    // get a reference to the active vessel
    private VesselComponent _activeVessel;

    // Stuff about thte current vessel
    public static VesselComponent ActiveVessel;

    // The UIDocument component of the window game object
    private UIDocument _window;

    // GUI Properties
    private VisualElement _rootElement;
    // Bool telling the update function that the gui is ready
    private bool initialized = false;
    private TextField _nameTextfield;
    private Toggle _noonToggle;
    private Label _greetingLabel;

    // The backing field for the IsWindowOpen property
    private bool _isWindowOpen;

    // Create field for all of your gui widgets
    Button closeButton;
    Button plotButton;
    Button goButton;
    Label orbitingLabel;
    Label deltaVLabel;
    FloatField targetLat;
    FloatField targetLon;
    DropdownField coordDropDown;
    GroupBox landSelectBox;
    RadioButton radioMild;
    RadioButton radioHot;
    RadioButton radioAtomic;

    // Variables for logic
    public string prevBody = "init";
    double[] xCoords = new double[] { };
    double[] yCoords = new double[] { };
    private string currLz;


    /// <summary>
    /// The state of the window. Setting this value will open or close the window.
    /// </summary>
    public bool IsWindowOpen
    {
        get => _isWindowOpen;
        set
        {
            _isWindowOpen = value;

            // Set the display style of the root element to show or hide the window
            _rootElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            // Alternatively, you can deactivate the window game object to close the window and stop it from updating,
            // which is useful if you perform expensive operations in the window update loop. However, this will also
            // mean you will have to re-register any event handlers on the window elements when re-enabled in OnEnable.
            // gameObject.SetActive(value);

            // Update the Flight AppBar button state
            GameObject.Find(LandingBuddyProjPlugin.ToolbarFlightButtonID)
                ?.GetComponent<UIValue_WriteBool_Toggle>()
                ?.SetValue(value);

            // Update the OAB AppBar button state
            GameObject.Find(LandingBuddyProjPlugin.ToolbarOabButtonID)
                ?.GetComponent<UIValue_WriteBool_Toggle>()
                ?.SetValue(value);
        }
    }

    /// <summary>
    /// Runs when the window is first created, and every time the window is re-enabled.
    /// </summary>
    // private void OnEnable()
    // {
    //     // Get the UIDocument component from the game object
    //     _window = GetComponent<UIDocument>();

    //     // Get the root element of the window.
    //     // Since we're cloning the UXML tree from a VisualTreeAsset, the actual root element is a TemplateContainer,
    //     // so we need to get the first child of the TemplateContainer to get our actual root VisualElement.
    //     _rootElement = _window.rootVisualElement[0];

    //     // Center the window by default
    //     _rootElement.CenterByDefault();

    //     // Get the close button from the window
    //     // closeButton = _rootElement.Q<Button>("CloseButtonIcon");


    //     // Get the orbit label from the window
    //     // orbitingLabel = _rootElement.Q<Label>("OrbitingLabel");

    //     // Hide the menu when the game starts up
    //     IsWindowOpen = false;
    // }

    // Function to update the dropdown lz based off of what you're orbiting
    public void updateLz(string currentBody)
    {
        switch (currentBody)
        {
            case "Kerbol":
                // populate lz options
                coordDropDown.choices = ["Select"];
                break;

            case "Moho":
                // populate lz options
                coordDropDown.choices = ["Select", "Moheart", "The Croissant", "Orge Rock"];
                xCoords = [88.55, 56.4, -10.075];
                yCoords = [-146.74, -29.37, -9.555];
                break;

            case "Eve":
                // populate lz options
                coordDropDown.choices = ["Select", "Tor", "Heart Island", "Frog Rock", "Tessellated Pavement"];
                xCoords = [-9.082, 52.254, -18.978, 14.529];
                yCoords = [-7.54, -110.530, -59.826, -34.729];
                break;

            case "Gilly":
                // populate lz options
                coordDropDown.choices = ["Select", "Precarious Stack", "Tipping Rock"];
                xCoords = [17.229, 28.788];
                yCoords = [-42.965, -46.606];
                break;

            case "Kerbin":
                // populate lz options
                coordDropDown.choices = ["Select", "Kerbal Space Center", "Kerb-2 Summit", "Stargazer Point", "Kapy Rock", "The Lost Center"];
                xCoords = [0, 48.638, -51.184, -48.782, 6.025];
                yCoords = [0, 131.959, -134.182, 58.724, 48.668];
                break;

            case "Mun":
                // populate lz options
                coordDropDown.choices = ["Select", "North Arch", "South Arch", "Monument", "Mun-Or-Bust Site", "Mun Mound"];
                xCoords = [2.486, -12.466, 12.4508, -13.4, 34.93];
                yCoords = [81.489, -140.989, 39.1959, -69.94, -28.48];
                break;

            case "Minmus":
                // populate lz options
                coordDropDown.choices = ["Select", "Monument", "Teetering Rock"];
                xCoords = [63.67, 26.64];
                yCoords = [-56.58, -22.8];
                break;

            case "Duna":
                // populate lz options
                coordDropDown.choices = ["Select", "Portobello Point", "Giant Rock", "Muchroom Garden", "Duna Face", "Monument"];
                xCoords = [-11.902, 4.09, -39.41, 17.054, 51.531];
                yCoords = [86.316, -32.11, -107.56, -85.471, -51.372];
                break;

            case "Ike":
                // populate lz options
                coordDropDown.choices = ["Select", "Rock 772-024J", "Giant's Linguini"];
                xCoords = [-24.035, 11.765];
                yCoords = [-90.5, -146.415];
                break;

            case "Dres":
                // populate lz options
                coordDropDown.choices = ["Select", "Fin Rock", "The Ultimate Quarterpipe East", "The Ultimate Quarterpipe West", "The Struts"];
                xCoords = [-33.53, -0.355, 0.48, -10.55];
                yCoords = [-171.05, 76.26, 58.15, 118.45];
                break;

            case "Jool":
                // populate lz options
                coordDropDown.choices = ["Select"];
                break;

            case "Layth":
                // populate lz options
                coordDropDown.choices = ["Select", "Megalofishicus Rex", "Ball Rocks"];
                xCoords = [-49.9, 1.036];
                yCoords = [-89.16, -132.335];
                break;

            case "Vall":
                // populate lz options
                coordDropDown.choices = ["Select", "Fallen Mega Penitente", "The Hoagie"];
                xCoords = [-38.546, 29.66];
                yCoords = [59.3, 153.44];
                break;

            case "Tylo":
                // populate lz options
                coordDropDown.choices = ["Select", "Monument", "Jebediah's Challenge", "Tunnel Rock"];
                xCoords = [-1.5, -11.095, 34.223];
                yCoords = [-110.55, -64.465, 56.853];
                break;

            case "Bop":
                // populate lz options
                coordDropDown.choices = ["Select", "The Cube", "Petrified Kraken", "Rocky Rovers"];
                xCoords = [-64.57, -37.9, -40.222];
                yCoords = [-49.9, -82.95, -96.033];
                break;

            case "Pol":
                // populate lz options
                coordDropDown.choices = ["Select", "Ice Tower", "Blurp"];
                xCoords = [18.06, 6.11];
                yCoords = [106.48, -146.65];
                break;

            case "Eeloo":
                // populate lz options
                coordDropDown.choices = ["Select", "Frozen Tsunami", "Captive Rock"];
                xCoords = [-25.05, 44.057];
                yCoords = [-109.42, 0.001];
                break;

            default:
                // what did you get?
                LandingBuddyProjPlugin.Logger.LogInfo($"Caught unexpected string: {currentBody}");
                coordDropDown.choices = ["Select"];
                break;
        }
    }

    public void Start()
    {
        SetupDocument();
    }

    public void SetupDocument()
    {
        // NOTE this seems excessive? Why does it start twice?
        // Get the UIDocument component from the game object
        _window = GetComponent<UIDocument>();
    }

    private void Update()
    {
        // If the gui is ready
        if (initialized)
        {
            // Get the active vehicle isActive is a field... whatever that is
            _activeVessel = Game?.ViewController?.GetActiveVehicle(true)?.GetSimVessel(true);
            // If we have an active vessel
            if (_activeVessel != null)
            {
                if (_activeVessel.mainBody.bodyName != prevBody)
                {
                    // Update the orbiting text
                    orbitingLabel.text = String.Format("Current Body: {0}", _activeVessel.mainBody.bodyName);
                    // Update the drop down
                    updateLz(_activeVessel.mainBody.bodyName);
                    // Update where we are
                    prevBody = _activeVessel.mainBody.bodyName;
                }

            }
            else
                return;
        }
        else
        {
            // Initialize the UI
            runInit();
            return;
        }

    }

    // Method to setup the gui
    private void runInit()
    {
        // Echo whats happening
        LandingBuddyProjPlugin.Logger.LogInfo($"Starting intialization");

        // Get the root element of the window.
        // Since we're cloning the UXML tree from a VisualTreeAsset, the actual root element is a TemplateContainer,
        // so we need to get the first child of the TemplateContainer to get our actual root VisualElement.
        _rootElement = _window.rootVisualElement[0];

        // Center the window by default
        _rootElement.CenterByDefault();

        // Setup buttons
        closeButton = _rootElement.Q<Button>("CloseButtonIcon");
        plotButton = _rootElement.Q<Button>("PlotButton");
        goButton = _rootElement.Q<Button>("GoButton");
        // Setup labels
        orbitingLabel = _rootElement.Q<Label>("OrbitingLabel");
        deltaVLabel = _rootElement.Q<Label>("DeltaVLabel");
        // Setup float inputs
        targetLat = _rootElement.Q<FloatField>("TargetLat");
        targetLon = _rootElement.Q<FloatField>("TargetLon");
        // Setup dropdown
        coordDropDown = _rootElement.Q<DropdownField>("CoordDropDown");
        // Setup radio button group and box
        landSelectBox = _rootElement.Q<GroupBox>("LandSelectBox");
        radioMild = _rootElement.Q<RadioButton>("RadioMild");
        radioHot = _rootElement.Q<RadioButton>("RadioHot");
        radioAtomic = _rootElement.Q<RadioButton>("RadioAtomic");

        // Add events
        closeButton.clicked += () => IsWindowOpen = false;
        coordDropDown.RegisterValueChangedCallback(evt =>
        {
            // Get the selected LZ for the current body
            string currentBody = _activeVessel.mainBody.bodyName;
            int ndx = coordDropDown.index;

            // get the coordinates
            double x = xCoords[ndx];
            double y = yCoords[ndx];

            // convert to latitude and longitude

            // Display lat and lon
                        
        });

        // Hide the menu when the game starts up
        IsWindowOpen = false;

        // Change intialized flag
        initialized = true;
    }
}
