using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class PlacingAndDragging : MonoBehaviour
{
    [SerializeField] AudioSource scanSfx;
    [SerializeField] AudioSource scanCompleteSfx;

    //for check pannel 
    public event Action CanPlayBall;

    static public bool spawnable = false; //when spawn UI pressed, true.
    bool LimitBall = false;
    bool changeColor = false;
    bool destroyAll = false;
    float timeToRestart = 0f;
    
    static public bool playsfxcheck = false;
    static public bool playsfxcheck2 = false;

    [SerializeField]
    public Text debugLog;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject bowingBall;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Vector2 touchPosition = default;

    public static int spawnObjectNum = 0;
    public static int spawnObjectLength = 0;

    [SerializeField]
    private List<GameObject> placedPrefabs = new List<GameObject>();

    ///////select object
    [SerializeField]
    private Color activeColor = Color.red;

    [SerializeField]
    private Color inactiveColor = Color.gray;

    [SerializeField]
    private Color OriginPinColor = Color.white;

    [SerializeField]
    private Material OriginBallColor;

    [SerializeField]
    private bool displayOverlay = false;
    private PlacementObject lastSelectedObject;
    private bool onTouchHold = false;

    public Text checkPlaneLog;

    ARPlane curPlane;
    public static float currPlaneY2 = 1;

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

    // bowling information
    bool fisrtTimerCheck = true;
    public static bool secondTimerCheck = true;
    float timer1 = 0;
    float timer2 = 0;
    public Image infoP;

    private void Awake() {
        spawnable = false;
        changeColor = false;
        LimitBall = false;
        destroyAll = false;
        timeToRestart = 0f;
        playsfxcheck = false;
        playsfxcheck2 = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        spawnObjectNum = 0;

        timer1 = 0;
        timer2 = 0;

        fisrtTimerCheck = true;
        secondTimerCheck = true;

        debugLog.text = "Wait until detecting your world.\n If you can't find the plane for too long \n" + "Please, restart it or move your camera";
        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            Destroy(plane);
        }

        FilteredPlane.isBig = false;
        //OriginPinColor =  placedPrefab.GetComponent<MeshRenderer>().material.color;
    }

    void Init()
    {
        playsfxcheck = false;
        spawnable = false;
        changeColor = false;
        LimitBall = false;
        destroyAll = false;
        timeToRestart = 0f;
        lastSelectedObject = null;

        //debugLog.text = "Restart and init";
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "ObjectTracking")
        {
            spawnObjectNum = GPS.locationNumber;
        }

        if (!playsfxcheck && !scanSfx.isPlaying)
        {
            scanSfx.Play();
            //SoundManager.instance.PlaySfx("ScanningPlane");
        }
        var cameraForward = arCamera.transform.forward;
        //debugLog.text = cameraForward.x +", " + cameraForward.y + ", " + cameraForward.z;

        checkPlaneLog.text = "Plane Check : Finding...";//FilteredPlane.isBig.ToString();

        if (FilteredPlane.isBig)
        {
            foreach (ARPlane plane in aRPlaneManager.trackables)
            {
                if (plane.extents.x * plane.extents.y >= FilteredPlane.dismenstionsForBigPlanes.x * FilteredPlane.dismenstionsForBigPlanes.y)
                {
                    aRPlaneManager.enabled = false;
                    playsfxcheck = true;

                    //checkPlaneLog.text -= "Finding...";
                    if(SceneManager.GetActiveScene().name == "ObjectTracking")
                    {
                        CanPlayBall.Invoke();
                    }
                    checkPlaneLog.text = "Plane Check : Done!";
                    //SoundManager.instance.PlaySfx("ScanningComplete");
                }
                else
                {
                    aRPlaneManager.enabled = false;
                    plane.gameObject.SetActive(aRPlaneManager.enabled);

                }
            }

            if(!scanCompleteSfx.isPlaying && !playsfxcheck2)
            {
                //debugLog.text = "Plane Detecting Success! \n " +
                //    "Touch the screen to put and dragging pins \n" +
                //    "then press 'Ball' to spawn ball.";

                debugLog.gameObject.SetActive(true);
                fisrtTimerCheck = false;
                debugLog.text = "Plane Detecting Success! \n" + "Touch the screen to put and dragging pins \n" + "then press 'Ball' to spawn ball.";

                playsfxcheck2 = true;
                scanSfx.Stop();
                scanCompleteSfx.Play();
            }

            if(fisrtTimerCheck == false)
            {
                timer1 += Time.deltaTime;

                if (timer1 > 3)
                {
                    debugLog.gameObject.SetActive(false);
                    infoP.gameObject.SetActive(false);
                    fisrtTimerCheck = true;
                    timer1 = 0;
                }
            }

            if (secondTimerCheck == false)
            {
                infoP.gameObject.SetActive(true);
                debugLog.gameObject.SetActive(true);
                debugLog.text = "Place the ball where you want it\n" + "and then press the Start rolling button to get ready to roll the ball. \n" + "If you're ready, you can swipe and roll the ball.";

                timer2 += Time.deltaTime;

                if (timer2 > 3)
                {
                    debugLog.gameObject.SetActive(false);
                    infoP.gameObject.SetActive(false);
                    secondTimerCheck = true;
                    timer2 = 0;
                }
            }

            curPlane = FindObjectOfType<ARPlane>();
            currPlaneY2 = curPlane.transform.position.y;

            if (destroyAll)
            {
                timeToRestart += Time.deltaTime;
                if(timeToRestart > 6f)
                {
                    //Init();
                    //swipeBall.Init();
                    if(SceneManager.GetActiveScene().name == "ObjectTracking")
                    {
                        SceneManager.LoadScene("ObjectTracking");
                    }
                    else
                    {
                        SceneManager.LoadScene("RollBall");
                    }
                }
            }
            if(swipeBall.toBeDestroy)
            {
                PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                foreach (PlacementObject placementObject in allOtherObjects)
                {
                    Destroy(placementObject.gameObject, 3f);
                }
            
                destroyAll = true;
                //SceneManager.LoadScene("RollBall");
            }

            if (swipeBall.rollable && !changeColor)
            {
                changeColor = true;
                PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                foreach (PlacementObject placementObject in allOtherObjects)
                {

                    MeshRenderer meshRenderer = placementObject.GetComponent<MeshRenderer>();
                    //placementObject.Selected = false;

                    if (placementObject.transform.name == "pin(Clone)")
                    {
                        //debugLog.text = "Pin";
                        meshRenderer.material.color = OriginPinColor;
                    }
                    else if (placementObject.transform.name == "Sphere(Clone)")
                    {
                        //debugLog.text = "Spehre";
                        meshRenderer.material = OriginBallColor;
                    }
                }
            }

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

                    if (Physics.Raycast(ray, out hitObject) && !swipeBall.rollable)
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
 
                    if (spawnable)
                    {
                        if (lastSelectedObject == null && !LimitBall)
                        {
                            //debugLog.text = "spawn ball";
                            LimitBall = true;
                            lastSelectedObject = Instantiate(bowingBall, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                            SoundManager.instance.PlaySfx("Placement");
                            float yDiff = curPlane.transform.localPosition.y - (lastSelectedObject.GetComponent<SphereCollider>().bounds.min.y);
                            Vector3 spawnPosition = new Vector3(lastSelectedObject.transform.position.x, lastSelectedObject.transform.position.y + yDiff, lastSelectedObject.transform.position.z);
                            lastSelectedObject.Size = 1;
                            lastSelectedObject.transform.position = spawnPosition;
                            lastSelectedObject.YPosition = spawnPosition.y;

                        }
                        else
                        {
                            if (lastSelectedObject.Selected)
                            {
                                Vector3 newPosition = new Vector3(hitPose.position.x, lastSelectedObject.YPosition, hitPose.position.z);
                                lastSelectedObject.transform.position = newPosition;
                                lastSelectedObject.transform.rotation = hitPose.rotation;
                            }
                        }

                        //spawnable = false;
                    }
                    else if (!swipeBall.rollable)
                    {
                        if (lastSelectedObject == null)
                        {
                            //debugLog.text = "spawn pins";
                            if (GPS.locationNumber == 2)
                            {
                                lastSelectedObject = Instantiate(placedPrefabs[spawnObjectNum], hitPose.position, Quaternion.Euler(90, 180, 0)).GetComponent<PlacementObject>();
                                //lastSelectedObject.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                            }
                            else
                            {
                                lastSelectedObject = Instantiate(placedPrefabs[spawnObjectNum], hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                            }
                            SoundManager.instance.PlaySfx("Placement");
                            
                            float yDiff = 0;

                            if(GPS.locationNumber == 2)
                            {
                                yDiff = curPlane.transform.localPosition.y - (lastSelectedObject.GetComponent<BoxCollider>().bounds.min.y);
                            }
                            else
                            {
                                yDiff = curPlane.transform.localPosition.y - (lastSelectedObject.GetComponent<CapsuleCollider>().bounds.min.y);
                            }

                            Vector3 spawnPosition = new Vector3(lastSelectedObject.transform.position.x, lastSelectedObject.transform.position.y + yDiff, lastSelectedObject.transform.position.z);
                            lastSelectedObject.Size = 1;
                            lastSelectedObject.transform.position = spawnPosition;
                            lastSelectedObject.YPosition = spawnPosition.y;
                        }
                        else
                        {
                            if (lastSelectedObject.Selected)
                            {
                                Vector3 newPosition = new Vector3(hitPose.position.x, lastSelectedObject.YPosition, hitPose.position.z);
                                lastSelectedObject.transform.position = newPosition;
                                lastSelectedObject.transform.rotation = hitPose.rotation;
                            }
                        }
                    }

                }
            }
        }

        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
