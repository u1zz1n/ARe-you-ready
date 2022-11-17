using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class swipeBall : MonoBehaviour
{
    static public bool rollable = false;
    private bool startroll = false;
    
    bool collideWpin = false;
    bool rolling = false;
    static public bool toBeDestroy = false;

    string btnName;

    float time = 0f;

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject bowingBall;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [SerializeField]
    private bool displayOverlay = false;
    private PlacementObject lastSelectedObject;
    private bool onTouchHold = false;

    Vector2 startPos, endPos, direction;
    float touchTimeStart, touchTimeFinish, timeInterval;

    [SerializeField]
    float throwForceInXandY = 0f;

    [SerializeField]
    float throwForceInZ = 5f;

    Rigidbody rb;

    [SerializeField]
    public Text debugLog;

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

    // Update is called once per frame
    void Update()
    {
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
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit Hit;
                if(Physics.Raycast(ray, out Hit))
                {
                    btnName = Hit.transform.name;
                    if(btnName == "Sphere(Clone)")
                    {
                        
                        //debugLog.text = "detect sphere";
                        touchTimeStart = Time.time;
                        startPos = Input.GetTouch(0).position;
                    }
                }
            }

            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                //debugLog.text = "Rolling";
                touchTimeFinish = Time.time;
                timeInterval = touchTimeFinish - touchTimeStart;
            
                endPos = Input.GetTouch(0).position;
                direction = startPos - endPos;

                rb.isKinematic = false;
                rb.AddForce(-direction.x * 0.3f, 0, -direction.y * 0.3f);
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
