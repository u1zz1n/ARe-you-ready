using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class PlacementController : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;

    //[SerializeField]
    //private Button dismissButton;
    [SerializeField]
    private bool applyScalingPerObject = false;

    [SerializeField]
    private Slider scaleSlider;

    [SerializeField]
    private Camera arCamera;

    private GameObject placedObject;

    private Vector2 touchPosition = default;

    private ARSessionOrigin aRSessionOrigin; 

    private ARRaycastManager arRaycastManager;
    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;

    private GameObject PlacedPrefab{
        get{
            return placedPrefab;
        }
        set{
            placedPrefab = value;
        }
    }

    void Awake() {
        arRaycastManager = GetComponent<ARRaycastManager>();
        aRSessionOrigin = GetComponent<ARSessionOrigin>();
        //dismissButton.onClick.AddListener(Dismiss);
        scaleSlider.onValueChanged.AddListener(ScaleChanged);
    }

    private void ScaleChanged(float newValue)
    {
        if(applyScalingPerObject)
        {
            if(lastSelectedObject != null && lastSelectedObject.Selected) 
            {
                lastSelectedObject.transform.localScale = Vector3.one * newValue;
            }
        }
        else{
            aRSessionOrigin.transform.localScale = Vector3.one * newValue;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            touchPosition = touch.position;

            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if(Physics.Raycast(ray, out hitObject))
                {
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    if(lastSelectedObject != null)
                    {
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach(PlacementObject placementObject in allOtherObjects)
                        {
                            if(placementObject != lastSelectedObject)
                            {
                                placementObject.Selected = false;
                            }
                            else{
                                placementObject.Selected = true;
                            }
                        }
                    }
                }

                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if(lastSelectedObject == null)
                    {
                        lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    }
                }
            }

            if(touch.phase == TouchPhase.Moved)
            {
                if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if(lastSelectedObject != null && lastSelectedObject.Selected) {
                            
                        lastSelectedObject.transform.parent.position = hitPose.position;
                        lastSelectedObject.transform.parent.rotation = hitPose.rotation;
                    }
                }
            }
        }
    }
}
