using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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
    /*static*/ bool isFirstPlane = true;
    /*static*/ bool checkOneTime = true;
    public static bool forAll = true;

    ARPlane curPlane;

    public float allSzie = 1;
    public static float currPlaneY = 1;

    public GameObject PlacedPrefab
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

    static int counts = 0;
    static int counts2 = 0;

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
        counts = 0;
        counts2 = 0;

        //foreach (ARPlane plane in aRPlaneManager.trackables)
        //{
        //    plane.gameObject.SetActive(false);
        //}

        //checkLog2.text = "Done!" + "plane is " + counts2;

        isFirstPlane = true;
        checkOneTime = true;

        spawnObjectNum = 0;
        scaleSlider.onValueChanged.AddListener(ScaleChanged);
        spawnObjectLength = placedObjects.Length;
        useDisableButton = false;
        FilteredPlane.isBig = false;
        aRPlaneManager.enabled = true;
        //arPointManager.enabled = true;
        forAll = true;
    }

    public void TogglePlaneDetection()
    {
        if (aRPlaneManager.enabled == false)
        {
            useDisableButton = !useDisableButton;
            checkOneTime = true;
            isFirstPlane = true;
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
            checkOneTime = true;
            isFirstPlane = true;
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


        if (useDisableButton)
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
        checkLog.text = "isPlane : " + isFirstPlane + " one : " + checkOneTime;
        //checkLog.text = "use : " + useDisableButton + "  enable : " + isCheck + " big :  " + FilteredPlane.isBig + " one : " + checkOneTime;
        //spawnObjectLength = placedObjects.Length;
        checkLog2.text = "Please continue to try to recognize the plane until 'Done' appears";

        if (FilteredPlane.isBig)
        {
            //checkLog2.text = "Wait for seconds";
            if (!useDisableButton && checkOneTime)
            {
                foreach (ARPlane plane in aRPlaneManager.trackables)
                {  
                    if ((plane.extents.x * plane.extents.y >= FilteredPlane.dismenstionsForBigPlanes.x * FilteredPlane.dismenstionsForBigPlanes.y) && isFirstPlane)
                    {
                        //counts++;
                        aRPlaneManager.enabled = false;
                        isFirstPlane = false;
                    }
                    else /*if((plane.extents.x * plane.extents.y < FilteredPlane.dismenstionsForBigPlanes.x * FilteredPlane.dismenstionsForBigPlanes.y) || (isFirstPlane == false))*/
                    {
                        aRPlaneManager.enabled = false;
                        //Destroy(plane);
                        plane.gameObject.SetActive(aRPlaneManager.enabled);
                    }
                }

                foreach (ARPlane plane in aRPlaneManager.trackables)
                {
                    counts++;
                }

                checkOneTime = false;
            }

            curPlane = FindObjectOfType<ARPlane>();
            currPlaneY = curPlane.gameObject.transform.position.y;

            checkLog2.text = "Done!" + "plane is " + counts + "position : " + curPlane.gameObject.transform.position;

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
                        //float yDiff = lastSelectedObject.GetComponent<Renderer>().bounds.center.y - curPlane.transform.localPosition.y;
                        float yDiff = curPlane.transform.localPosition.y - (lastSelectedObject.GetComponent<CapsuleCollider>().bounds.min.y);
                        Vector3 spawnPosition = new Vector3(lastSelectedObject.transform.position.x, lastSelectedObject.transform.position.y + yDiff, lastSelectedObject.transform.position.z);
                        lastSelectedObject.Size = 1;
                        lastSelectedObject.transform.position = spawnPosition;
                        lastSelectedObject.YPosition = spawnPosition.y;
                        //lastSelectedObject.Selected = true;
                    }
                    else
                    {
                        if (lastSelectedObject.Selected)
                        {
                            Vector3 newPosition = new Vector3(hitPose.position.x, lastSelectedObject.YPosition, hitPose.position.z);
                            lastSelectedObject.transform.position = newPosition;              
                            lastSelectedObject.transform.rotation = hitPose.rotation;
                            //checkLog.text = "it moved";
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
        int sizeee = allOtherObjects.Length - 1;
        checkLog4.text = allOtherObjects[sizeee].name + allOtherObjects[sizeee].transform.position;


        foreach (PlacementObject placementObject in allOtherObjects)
        {
            if(placementObject.name == "waterbottle" || placementObject.name == "pin" || placementObject.name == "pills" || placementObject.name == "BowlingPins" 
                || placementObject.name == "pin1" || placementObject.name == "pin2" || placementObject.name == "pin3" || placementObject.name == "pin4" || placementObject.name == "pin5"
                || placementObject.name == "pin6" || placementObject.name == "pin7" || placementObject.name == "pin8" || placementObject.name == "pin9" || placementObject.name == "pin10")
            {

            }
            //else
            {
                float newVal = placementObject.PreSliderValue - newValue;

                //checkLog4.text = "chagne value: " + allOtherObjects;

                placementObject.Size = newVal;
                allSzie = newVal;
                placementObject.PreSliderValue = newValue;
                placementObject.PreEachSliderValue = newValue;

                //aRSessionOrigin.MakeContentAppearAt(placementObject.gameObject.transform, Quaternion.identity);

                placementObject.gameObject.transform.localScale = new Vector3(placementObject.gameObject.transform.localScale.x - newVal, placementObject.gameObject.transform.localScale.y - newVal, placementObject.gameObject.transform.localScale.z - newVal);

                if (placementObject.gameObject.transform.localScale.x <= 0 || placementObject.gameObject.transform.localScale.y <= 0 || placementObject.gameObject.transform.localScale.x <= 0)
                {
                    placementObject.gameObject.transform.localScale = new Vector3(0, 0, 0);
                }
                //placementObject.gameObject.transform.position = Vector3.Scale(placementObject.gameObject.transform.position - placementObject.GetComponent<Renderer>().bounds.center, placementObject.gameObject.transform.localScale) + placementObject.GetComponent<Renderer>().bounds.center;
                //float yDiff = curPlane.transform.localPosition.y - (placementObject.GetComponent<CapsuleCollider>().bounds.min.y);
                //Vector3 newPosition = new Vector3(placementObject.gameObject.transform.position.x, placementObject.gameObject.transform.position.y + yDiff, placementObject.gameObject.transform.position.z);

                //placementObject.transform.position = newPosition;
                //placementObject.YPosition = newPosition.y;

                scaleCheck.text = "change: " + newVal + "scale: " + placementObject.Size;
            }     
        }
    }
}