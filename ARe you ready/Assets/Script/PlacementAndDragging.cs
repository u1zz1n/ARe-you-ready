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

    public static bool placeBowling;
    public static bool RollBowling;
    private bool ballspawn;

    public static int spawnObjectNum = 0;
    public static int spawnObjectLength = 0;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    public Button planeButton;

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
    private bool applyScalingPerObject = false;

    [SerializeField]
    private Slider scaleSlider;

    public Text hihi;

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
        placeBowling = false;
        ballspawn = false;
        scaleSlider.onValueChanged.AddListener(ScaleChanged);
        spawnObjectLength = placedObjects.Length;
    }

    public void TogglePlaneDetection()
    {
        //hihi.text = "sisisisi";
        aRPlaneManager.enabled = !aRPlaneManager.enabled;

        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            plane.gameObject.SetActive(aRPlaneManager.enabled);
        }

        planeButton.GetComponentInChildren<Text>().text = aRPlaneManager.enabled ?
        "Enable" : "Disable";
        
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
        //hihi.text = "scale is " + scaleSlider.value.ToString();

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

                           if (displayOverlay)
                               placementObject.ToggleOverlay();
                       }
                   }
               }
           }

           if (touch.phase == TouchPhase.Ended)
           {
               lastSelectedObject.Selected = false;
           }

            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (placeBowling)
                {
                    if (lastSelectedObject == null && !ballspawn)
                    {
                        ballspawn = true;
                        lastSelectedObject = Instantiate(bowingBall, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                        //RollBall.ballReady = true;
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
                else
                {
                    if (lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(placedPrefabs[spawnObjectNum], hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
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
            hihi.text = "Water Bottle";
        }
        else if (spawnObjectNum == 1)
        {
            hihi.text = "Bowling Pin";
        }
        else if (spawnObjectNum == 2)
        {
            hihi.text = "Pill";
        }
        else if (spawnObjectNum == 3)
        {
            hihi.text = "Bowling Pin Set";
        }
    }

    private void ScaleChanged(float newValue)
    {
        if (applyScalingPerObject)
        {
            if (lastSelectedObject != null && lastSelectedObject.Selected)
            {
                lastSelectedObject.transform.localScale = Vector3.one * newValue;
            }
        }
        else
        {
            aRSessionOrigin.transform.localScale = Vector3.one * newValue;
        }
    }
}