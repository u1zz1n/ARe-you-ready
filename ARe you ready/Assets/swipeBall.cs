using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class swipeBall : MonoBehaviour
{
    static public bool rollable = false; //when roll UI pressed, true.
    static public bool startroll = false; //taking time to unpressed the roll UI
    
    static public bool collideWpin = false; //when the ball hit the pin
    static public bool rolling = false;  //when player finish swiping ball (unpress screen)
    static public bool toBeDestroy = false; //make object destroy

    string btnName;

    static public float time = 0f;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    private Vector2 touchPosition = default;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject bowingBall;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private bool displayOverlay = false;
    private PlacementObject lastSelectedObject;
    private bool onTouchHold = false;

    Vector2 startPos, endPos/*, direction*/;
    Vector3 startWPos, endWPos, direction;
    float touchTimeStart, touchTimeFinish, timeInterval;

    [SerializeField]
    float throwForceInXandY = 0f;

    [SerializeField]
    float throwForceInZ = 5f;

    Rigidbody rb;

    [SerializeField]
    public Text debugLog;

    [SerializeField]
    public Text debugLog2;
    // Start is called before the first frame update
    private void Awake() {
        rollable = false;
        startroll = false;
        collideWpin = false;
        rolling = false;
        toBeDestroy = false;

        time = 0f;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody> ();     
        //debugLog.text = "Cool time to roll";
    }
    static public void Init()
    {
        rollable = false;
        startroll = false;
        collideWpin = false;
        rolling = false;
        toBeDestroy = false;

        time = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        var cameraForward = arCamera.transform.forward;
        //debugLog.text = cameraForward.x +", " + cameraForward.y + ", " + cameraForward.z;

        if(rollable)
        {
            time += Time.deltaTime;

            if (time > 0.5f)
            {
                startroll = true;
                //debugLog.text = "You can roll now";
            }

            if(rolling)
            {
                if(collideWpin)
                {
                    toBeDestroy = true;
                }
                else if(rb.velocity.magnitude < 0.1f)
                {        
                    toBeDestroy = true;    
                }            
            }

        }

        

        if(Input.touchCount > 0 && startroll)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;
            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit Hit;
                if(Physics.Raycast(ray, out Hit))
                {
                    btnName = Hit.transform.name;
                    if(btnName == "Sphere(Clone)")
                    {
                        
                        //debugLog.text = "detect sphere";
                        touchTimeStart = Time.time;
                        //startPos = Input.GetTouch(0).position;
                        //debugLog.text = touchPosition.x + ", " + touchPosition.y;
                        Vector3 pos = new Vector3(touchPosition.x, touchPosition.y, 1f);
                        pos = Camera.main.ScreenToWorldPoint(pos);
                        startWPos = pos;
                        //debugLog2.text = startWPos.x +", " + startWPos.y + ", "+ startWPos.z;
                    }
                }
            }

            if(touch.phase == TouchPhase.Ended)
            {
                //debugLog.text = "Rolling";
                touchTimeFinish = Time.time;
                timeInterval = touchTimeFinish - touchTimeStart;
            
                //endPos = Input.GetTouch(0).position;
                //debugLog.text = touchPosition.x + ", " + touchPosition.y;
                Vector3 pos = new Vector3(touchPosition.x, touchPosition.y, 1f);
                pos = Camera.main.ScreenToWorldPoint(pos);
                endWPos = pos;
                //debugLog2.text = endWPos.x +", "+ endWPos.y + ", " + endWPos.z;
                direction = endWPos - startWPos;
                rb.isKinematic = false;
                if(cameraForward.x > 0 && cameraForward.z > 0)
                {
                    //debugLog2.text = "direction north";
                    rb.AddForce(direction.x * 300f, this.transform.position.y, direction.y * 300f);
                }
                else if(cameraForward.x < 0  && cameraForward.z < 0)
                {
                    rb.AddForce(direction.x * 300f, this.transform.position.y, -direction.y * 300f);
                }
                else if(cameraForward.x > 0 && cameraForward.z < 0)
                {
                    rb.AddForce(direction.x * 300f, this.transform.position.y, -direction.y * 300f);
                }
                else if(cameraForward.x < 0 && cameraForward.z > 0)
                {
                    rb.AddForce(direction.x * 300f, this.transform.position.y, direction.y * 300f);
                }
                rolling = true;
                //Destroy(gameObject, 3f);
            }
        }
        /*
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && startroll)
        {
            debugLog.text = "Roll start";
            touchTimeStart = Time.time;
            startPos = Input.GetTouch(0).position;
        }

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && startroll) 
        {
            debugLog.text = "Rolling";
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            
            endPos = Input.GetTouch(0).position;
            direction = startPos - endPos;

            rb.isKinematic = false;
            rb.AddForce(-direction.x * 0.3f, 0, -direction.y * 0.3f);
            
            Destroy(gameObject, 3f);
        }
        */
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "pin(Clone)")
        {
            collideWpin = true;
            debugLog.text = "Collide with pin";
        }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "pin(Clone)" && startroll)
        {
            collideWpin = true;
            debugLog.text = "Collide with pin";
        }
    }
}
