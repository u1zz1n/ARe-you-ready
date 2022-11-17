using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlacementAndDragging : MonoBehaviour
{
    // Start is called before the first frame update
    public static int spawnObjectNum = 0;
    public static int spawnObjectLength = 0;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    public Button planeButton;
    public Button planeButton2;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private List<GameObject> placedPrefabs = new List<GameObject>();

    [SerializeField]
    private GameObject bowingBall;

    private PlacementObject[] placedObjects;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector2 touchPosition = default;


    ///////select object
    [SerializeField]
    private Color activeColor = Color.red;

    [SerializeField]
    private Color inactiveColor = Color.gray;

    [SerializeField]
    private bool displayOverlay = false;

    private PlacementObject lastSelectedObject;

    private bool onTouchHold = false;

    [SerializeField]
    private bool applyScalingPerObject;

    [SerializeField]
    private Slider scaleSlider;

    public Text placeObejctName;
    public Text scaleCheck;
    public Text checkLog;
    public Text checkLog2;
    //public Text checkLog3;
    public Text checkLog4;


    bool useDisableButton = false;
    bool isCheck = false;

    public static bool forAll = true;

    private GameObject PlacedPrefab
    {
        get
        {
            return placedPrefabs[spawnObjectNum];
        }
        set
        {
            placedPrefabs[spawnObjectNum] = value;
        }
    }



    void Awake()
    {
    }

    /*private void ChangePrefabSelection(string name)
    {
        GameObject loadedGameObject = Resources.Load<GameObject>($"Prefabs/{name}");
        if (loadedGameObject != null)
        {
            PlacedPrefab = loadedGameObject;
            Debug.Log($"Game object with name {name} was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name {name}");
        }
    }*/

    void Start()
    {
        spawnObjectNum = 0;
        scaleSlider.onValueChanged.AddListener(ScaleChanged);
        spawnObjectLength = placedObjects.Length;
        useDisableButton = false;
        FilteredPlane.isBig = false;
        aRPlaneManager.enabled = true;
        forAll = true;
    }

    public void TogglePlaneDetection()
    {
        if (aRPlaneManager.enabled == false)
        {
            useDisableButton = !useDisableButton;
        }      

        
        aRPlaneManager.enabled = !aRPlaneManager.enabled;

        if(useDisableButton)
        {
            aRPlaneManager.enabled = false;
        }

        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            plane.gameObject.SetActive(aRPlaneManager.enabled);
        }

        planeButton.GetComponentInChildren<Text>().text = aRPlaneManager.enabled ?
        "Disable" : "Enable";
    }

    public void TogglePlaneDetection2()
    {
        if (aRPlaneManager.enabled == false)
        {
            useDisableButton = !useDisableButton;
        }

        aRPlaneManager.enabled = !aRPlaneManager.enabled;

        if (useDisableButton)
        {
            aRPlaneManager.enabled = false;
        }

        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            plane.gameObject.SetActive(aRPlaneManager.enabled);
        }

        planeButton2.GetComponentInChildren<Text>().text = aRPlaneManager.enabled ?
        "Remove" : "Restart";


        if (aRPlaneManager.enabled)
        {
            PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();

            foreach (PlacementObject placementObject in allOtherObjects)
            {
                placementObject.gameObject.SetActive(true);
            }
        }
        else
        {
            PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();

            foreach (PlacementObject placementObject in allOtherObjects)
            {
                placementObject.gameObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        isCheck = aRPlaneManager.enabled;
        checkLog.text = "use : " + useDisableButton + "  enable : " + isCheck + " big :  " + FilteredPlane.isBig;

        //hihi.text = "scale is " + scaleSlider.value.ToString();
        //spawnObjectLength = placedObjects.Length;
        checkLog2.text = "if horizontal check toggle, please do not move and wait";

        if (FilteredPlane.isBig)
        {
            if (!useDisableButton)
            {
                foreach (ARPlane plane in aRPlaneManager.trackables)
                {
                    if (plane.extents.x * plane.extents.y >= 1 * 1)
                    {
                        aRPlaneManager.enabled = false;
                    }
                    else
                    {
                        aRPlaneManager.enabled = false;
                        plane.gameObject.SetActive(aRPlaneManager.enabled);

                    }
                }
            }

            checkLog2.text = "Done!";

            printObjectName();

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                touchPosition = touch.position;

                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = arCamera.ScreenPointToRay(touch.position);
                    RaycastHit hitObject;

                    if (Physics.Raycast(ray, out hitObject))
                    {
                        lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();

                        if (lastSelectedObject != null)
                        {
                            PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();

                            foreach (PlacementObject placementObject in allOtherObjects)
                            {
                                MeshRenderer meshRenderer = placementObject.GetComponent<MeshRenderer>();

                                if (placementObject != lastSelectedObject)
                                {
                                    placementObject.Selected = false;
                                    meshRenderer.material.color = inactiveColor;
                                }
                                else
                                {
                                    placementObject.Selected = true;
                                    meshRenderer.material.color = activeColor;
                                }
                            }
                        }
                    }
                }

                //if (touch.phase == TouchPhase.Ended)
                //{
                //    lastSelectedObject.Selected = false;
                //}

                if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if (lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(placedPrefabs[spawnObjectNum], hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                        lastSelectedObject.Size = 1;
                        //lastSelectedObject.Selected = true;
                    }
                    else
                    {
                        if (lastSelectedObject.Selected)
                        {
                            lastSelectedObject.transform.position = hitPose.position;
                            lastSelectedObject.transform.rotation = hitPose.rotation;
                        }
                    }

                }
            }
        }
        
    }
    private void printObjectName()
    {
        if (spawnObjectNum == 0)
        {
            placeObejctName.text = "Water Bottle";
        }
        else if (spawnObjectNum == 1)
        {
            placeObejctName.text = "Bowling Pin";
        }
        else if (spawnObjectNum == 2)
        {
            placeObejctName.text = "Pill";
        }
        else if (spawnObjectNum == 3)
        {
            placeObejctName.text = "Bowling Pin Set";
        }
    }

    private void ScaleChanged(float newValue)
    {
        forAll = true;

        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();

        foreach (PlacementObject placementObject in allOtherObjects)
        {
            float newVal = placementObject.PreSliderValue - newValue;

            checkLog4.text = "chagne value: " + newVal;

            placementObject.Size = newVal;
            placementObject.PreSliderValue = newValue;

            aRSessionOrigin.MakeContentAppearAt(placementObject.gameObject.transform, Quaternion.identity);

            placementObject.gameObject.transform.localScale = new Vector3(placementObject.gameObject.transform.localScale.x - newVal, placementObject.gameObject.transform.localScale.y - newVal, placementObject.gameObject.transform.localScale.z - newVal);

            scaleCheck.text = "change: " + newVal + "scale: " + placementObject.Size;
        }
    }
}