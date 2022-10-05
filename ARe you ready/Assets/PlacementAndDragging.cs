using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlacementAndDragging : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject placedPrefab;

    //[SerializeField]
    //private GameObject welcomePanel;

    //[SerializeField]
    //private Button dismissButton;

    [SerializeField]
    private Camera arCamera;

    private PlacementObject[] placedObjects;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;

    //[SerializeField]
    //private Button redButton, greenButton, blueButton;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        //dismissButton.onClick.AddListener(Dismiss);

        //if (redButton != null && greenButton != null && blueButton != null)
        //{
        //    redButton.onClick.AddListener(() => ChangePrefabSelection("ARRed"));
        //    greenButton.onClick.AddListener(() => ChangePrefabSelection("ARGreen"));
        //    blueButton.onClick.AddListener(() => ChangePrefabSelection("ARBlue"));
        //}
    }

    private void ChangePrefabSelection(string name)
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
    }
    //private void Dismiss() => welcomePanel.SetActive(false);

    private GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // do not capture events unless the welcome panel is hidden
        //if (welcomePanel.activeSelf)
        //    return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

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
                            placementObject.Selected = placementObject == lastSelectedObject;
                        }
                    }
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                lastSelectedObject.Selected = false;
            }
        }

        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            if (lastSelectedObject == null)
            {
                lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
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