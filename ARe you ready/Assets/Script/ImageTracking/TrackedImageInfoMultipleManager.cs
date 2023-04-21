using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]

public class TrackedImageInfoMultipleManager : MonoBehaviour
{
    public ARRaycastManager arRaycastManager;

    bool isDragging = false;


    //[SerializeField]
    //private Text draggingText;

    //[SerializeField]
    //private Text DebugLog;

    [SerializeField]
    private Camera arCamera;

    //[SerializeField]
    //private Text imageTrackedText;

    [SerializeField]
    private GameObject[] arObjectsToPlace;

    [SerializeField]
    private GameObject Youtube;

    [SerializeField]
    private GameObject Twitter;

    [SerializeField]
    private GameObject AndroidCall;

    [SerializeField]
    private GameObject Uijin;

    [SerializeField]
    private GameObject Hagyeong;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(0.01f, 1f, 0.01f);

    [SerializeField]
    private  ARTrackedImageManager m_TrackedImageManager;

    private readonly Dictionary<string, GameObject> arObjects = new();

    private readonly Dictionary<string, GameObject> arObjects4card = new();
    private readonly Dictionary<string, GameObject> arObjects4HGcard = new();

    private Vector3 newPosition;
    //bool IsCard;
    //private GameObject a;
    private Vector2 touchPosition = default;
    
    private bool touchYoutube;
    private bool touchTwitter;
    private bool touchAndroid;

    private PlacementObject lastSelectedObject;

    public static GameObject curSelectedObject;

    public static Vector3 imagePosition;

    private float lastTouchTime;
    private const float doubleTouchDelay = 0.5f;

    private void Awake() {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        //Youtube.SetActive(false);
        //IsCard = false;
    }

    private void Start() {
        //a = Instantiate(Youtube, Vector3.zero, Quaternion.identity);
        touchYoutube = false;
        touchTwitter = false;
        touchAndroid = false;
        isDragging = false;

        lastTouchTime = Time.time;
    }

    private void Update() {
        
        if(touchYoutube)
        {
            //DebugLog.text = "YOUTUBE TOUCHED";
            Application.OpenURL("http://www.youtube.com/channel/UCmwT0R0CAhvev4MA4Qamejw");
            touchYoutube = false;
        }

        if(touchTwitter)
        {
            //DebugLog.text = "TWITTER TOUCHED";
            Application.OpenURL("http://twitter.com/NCTsmtown_DREAM?ref_src=twsrc%5Egoogle%7Ctwcamp%5Eserp%7Ctwgr%5Eauthor");
            touchTwitter = false;
        }

        if(touchAndroid)
        {
            //DebugLog.text = "ANDROID TOUCHED";
            Application.OpenURL("tel://[+12066949766");
            touchAndroid = false;
        }

        //isDragging = false;

        if (Input.touchCount > 0)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                //DebugLog.text = "TOUCHED";

                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if(Time.time - lastTouchTime < doubleTouchDelay)
                    {
                        if (hitObject.transform.gameObject.name == "Youtube(Clone)")
                            touchYoutube = true;
                        if (hitObject.transform.gameObject.name == "Twitter(Clone)")
                            touchTwitter = true;
                        if (hitObject.transform.gameObject.name == "Android(Clone)")
                            touchAndroid = true;
                    }
                    


                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    curSelectedObject = hitObject.transform.gameObject;

                    isDragging = true;           
                }
                else
                {
                     isDragging = false;
                     lastSelectedObject = null;
                     curSelectedObject = null;
                }
            }
            else if (touch.phase == TouchPhase.Moved /*&& isDragging*/)
            {
                //if (touch.position.x >= 0 && touch.position.x <= Screen.width && touch.position.y >= 0 && touch.position.y <= Screen.height)
                //{
                    //Vector3 newPos = arCamera.ScreenToWorldPoint(touch.position) + imagePosition;
                    //newPos.z = lastSelectedObject.gameObject.transform.position.z; // Maintain the object's z position
                    //ShowAndroidToastMessage("Dragging Start with" + lastSelectedObject.gameObject.name.ToString());
                    //Debug.Log(newPos.ToString());
                    //Vector3 newPos = lastSelectedObject.transform.position;
                    //newPos.x += 0.05f;
                    //lastSelectedObject.transform.position = newPos;
                    //curSelectedObject = lastSelectedObject.gameObject;
                //}
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                // Stop dragging the object
                //isDragging = false;
                //lastSelectedObject = null;
                //curSelectedObject = null;
                lastTouchTime = Time.time;
                //draggingText.text = "  " + "isDragging is " + isDragging;

            }

            if(lastSelectedObject != null)
            {
                curSelectedObject = lastSelectedObject.gameObject;
            }

            //if (isDragging)
            //{
                //draggingText.text += lastSelectedObject.gameObject.name.ToString();
                //draggingText.text += "  " + "isDragging is " + isDragging;
            //}
        }
    }
    void OnEnable(){
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable(){
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(var trackedImage in eventArgs.added)
        {
            //imageTrackedText.text = trackedImage.referenceImage.name;

            var name = trackedImage.referenceImage.name;

            foreach(var curPrefab in arObjectsToPlace)
            {
                if(string.Compare(curPrefab.name, name, StringComparison.Ordinal) == 0 && !arObjects.ContainsKey(name))
                {
                    var newARObject = Instantiate(curPrefab, trackedImage.transform);
                    arObjects[name] = newARObject;

                    if(name == "StudentID")
                    {    
                        newPosition = new Vector3(0, 0, 0.35f);
                        //IsCard = true;
                        //other icons for business card
                        var obj = Instantiate(Youtube, trackedImage.transform);
                        arObjects4card["Youtube"] = obj;
                        arObjects4card["Youtube"].transform.position = trackedImage.transform.position + new Vector3(0.35f, 0, 0.2f);

                        obj = Instantiate(Twitter, trackedImage.transform);
                        arObjects4card["Twitter"] = obj;
                        arObjects4card["Twitter"].transform.position = trackedImage.transform.position + new Vector3(-0.3f, 0, 0.05f);

                        obj = Instantiate(AndroidCall, trackedImage.transform);
                        arObjects4card["Android"] = obj;
                        arObjects4card["Android"].transform.position = trackedImage.transform.position + new Vector3(0.3f, 0, -0.1f);

                        obj = Instantiate(Uijin, trackedImage.transform);
                        arObjects4card["Uijin"] = obj;
                        arObjects4card["Uijin"].transform.position = trackedImage.transform.position + new Vector3(0, 0.05f, 0.35f);
                    }
                    else if(name == "HGStudentID")
                    {
                        imagePosition = trackedImage.transform.position;
                        newPosition = new Vector3(0, 0, 0.35f);
                        var objHG = Instantiate(Youtube, trackedImage.transform);
                        arObjects4HGcard["Youtube"] = objHG;
                        arObjects4HGcard["Youtube"].transform.position = trackedImage.transform.position + new Vector3(0.35f, 0, 0.2f);

                        objHG = Instantiate(Twitter, trackedImage.transform);
                        arObjects4HGcard["Twitter"] = objHG;
                        arObjects4HGcard["Twitter"].transform.position = trackedImage.transform.position + new Vector3(-0.3f, 0, 0.05f);

                        objHG = Instantiate(AndroidCall, trackedImage.transform);
                        arObjects4HGcard["Android"] = objHG;
                        arObjects4HGcard["Android"].transform.position = trackedImage.transform.position + new Vector3(0.3f, 0, -0.1f);

                        objHG = Instantiate(Hagyeong, trackedImage.transform);
                        arObjects4HGcard["Hagyeong"] = objHG;
                        arObjects4HGcard["Hagyeong"].transform.position = trackedImage.transform.position + new Vector3(0, 0.05f, 0.6f);
                    }
                    else
                    {
                        newPosition = new Vector3(0, 0, 0);
                    }
                    arObjects[name].transform.position = trackedImage.transform.position + newPosition;

                    //ShowAndroidToastMessage("Instantiated!");

                }
            }
        }

        foreach(var trackedImage in eventArgs.updated)
        {
            arObjects[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);

            if(!arObjects["StudentID"].activeSelf)
            {
                //IsCard = false;
                arObjects4card["Youtube"].SetActive(false);
                arObjects4card["Twitter"].SetActive(false);
                arObjects4card["Android"].SetActive(false);
                arObjects4card["Uijin"].SetActive(false);
            }
            else{
                arObjects4card["Youtube"].SetActive(true);
                arObjects4card["Twitter"].SetActive(true);
                arObjects4card["Android"].SetActive(true);
                arObjects4card["Uijin"].SetActive(true);
            }

            if(!arObjects["HGStudentID"].activeSelf)
            {
                arObjects4HGcard["Youtube"].SetActive(false);
                arObjects4HGcard["Twitter"].SetActive(false);
                arObjects4HGcard["Android"].SetActive(false);
                arObjects4HGcard["Hagyeong"].SetActive(false);
            }
            else{
                arObjects4HGcard["Youtube"].SetActive(true);
                arObjects4HGcard["Twitter"].SetActive(true);
                arObjects4HGcard["Android"].SetActive(true);
                arObjects4HGcard["Hagyeong"].SetActive(true);
            }

            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                //imageTrackedText.text = trackedImage.referenceImage.name;
                if(trackedImage.referenceImage.name/*name*/ == "StudentID" || trackedImage.referenceImage.name/*name*/ == "HGStudentID")
                {
                    newPosition = new Vector3(0, 0, 0.35f);

                    //arObjects4card["Youtube"].transform.position = trackedImage.transform.position+ new Vector3(0.35f, 0, 0.2f);
                    //arObjects4card["Twitter"].transform.position = trackedImage.transform.position + new Vector3(-0.3f, 0, 0.05f);
                    //arObjects4card["Android"].transform.position = trackedImage.transform.position + new Vector3(0.3f, 0, -0.1f);
                    //arObjects4card["Uijin"].transform.position = trackedImage.transform.position + new Vector3(0, 0.05f, 0.35f);
                    
                    //arObjects4HGcard["Youtube"].transform.position = trackedImage.transform.position + new Vector3(0.35f, 0, 0.2f);
                    //arObjects4HGcard["Twitter"].transform.position = trackedImage.transform.position + new Vector3(-0.3f, 0, 0.05f);
                    //arObjects4HGcard["Android"].transform.position = trackedImage.transform.position + new Vector3(0.3f, 0, -0.1f);
                    //arObjects4HGcard["Hagyeong"].transform.position = trackedImage.transform.position + new Vector3(0, 0.05f, 0.6f);
                }
                else
                {
                    newPosition = new Vector3(0, 0, 0);
                }

                arObjects[trackedImage.referenceImage.name/*name*/].transform.position = trackedImage.transform.position + newPosition;
            }
        }

        foreach(var trackedImage in eventArgs.removed)
        {
            if(trackedImage.referenceImage.name == "StudentID")
            {
                //IsCard = false;
                Destroy(arObjects4card["Youtube"]);
                arObjects4card.Remove("Youtube");

                Destroy(arObjects4card["Twitter"]);
                arObjects4card.Remove("Twitter");

                Destroy(arObjects4card["Android"]);
                arObjects4card.Remove("Android");

                Destroy(arObjects4card["Uijin"]);
                arObjects4card.Remove("Uijin");
            }

            else if(trackedImage.referenceImage.name == "HGStudentID")
            {
                Destroy(arObjects4HGcard["Youtube"]);
                arObjects4HGcard.Remove("Youtube");

                Destroy(arObjects4HGcard["Twitter"]);
                arObjects4HGcard.Remove("Twitter");

                Destroy(arObjects4HGcard["Android"]);
                arObjects4HGcard.Remove("Android");
                
                Destroy(arObjects4HGcard["Hagyeong"]);
                arObjects4HGcard.Remove("Hagyeong");
            }

            Destroy(arObjects[trackedImage.referenceImage.name]);
            arObjects.Remove(trackedImage.referenceImage.name);
        }
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private static void ShowAndroidToastMessage(string message)
    {
#if UNITY_ANDROID
        using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (unityActivity == null) return;
        var toastClass = new AndroidJavaClass("android.widget.Toast");
        unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            // Last parameter = length. Toast.LENGTH_LONG = 1
            using var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText",
                unityActivity, message, 1);
            toastObject.Call("show");
        }));
#endif
    }
}
