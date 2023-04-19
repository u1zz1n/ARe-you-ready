using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpawnInMulti : MonoBehaviourPun, IPunObservable
{
    public GameObject idealPlanePrefab; // Assign the custom networked object prefab in the inspector

    public ARRaycastManager arRaycastManager;
    public ARSessionOrigin aRSessionOrigin;
    public ARPlaneManager aRPlaneManager;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject cube;

    private int CanSpawn = 0;
    static public bool CanChangeColor = false;

    public Text checkPlaneLog;
    public Text synPlaneLog;

    ARPlane idealPlaneForMaster;
    ARPlane idealPlaneForPlayer;

    GameObject planeForMasterToSpawn;
    GameObject planeForPlayerToSpawn;

    Vector3 centerForMaster;
    Quaternion rotationForMaster;
    Vector2 extentsForMaster;

    Vector3 centerForPlayer;
    Quaternion rotationForPlayer;
    Vector2 extentsForPlayer;

    public static bool masterIsDone = false;
    public static bool playerIsDone = false;

    bool checkAllDoen = false;
    bool once1 = false;
    bool once2 = false;
    bool once3 = false;

    public event Action CanPlayScene;

    public void OnClick_Color()
    {
        CanChangeColor = true;
    }

    public void OnClick_Spawn()
    {
        CanSpawn++;
    }

    void Start()
    {
        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            Destroy(plane);
        }

        FilteredPlane.isBig = false;

        checkPlaneLog.text = "Wait until detecting your world.\n If you can't find the plane for too long \n" + "Please, restart it or move your camera";

        masterIsDone = false;
        playerIsDone = false;
        checkAllDoen = false;

        once1 = false;
        once2 = false;
        once3 = false;

        CanSpawn = 0;
        CanChangeColor = false;
    }

    void Update()
    {
        checkPlaneLog.text = "Plane Check : Finding...";

        if (FilteredPlane.isBig)
        {
            foreach (ARPlane plane in aRPlaneManager.trackables)
            {
                if (plane.extents.x * plane.extents.y >= FilteredPlane.dismenstionsForBigPlanes.x * FilteredPlane.dismenstionsForBigPlanes.y)
                {
                    aRPlaneManager.enabled = false;

                    if (SceneManager.GetActiveScene().name == "Multiplayer")
                    {
                        CanPlayScene.Invoke();
                    }
                    
                    checkPlaneLog.text = "Plane Check : Done!" + "Then press 'spawn' to spawn box";
                }
                else
                {
                    aRPlaneManager.enabled = false;
                    plane.gameObject.SetActive(aRPlaneManager.enabled);
                }
            }

            if (PhotonNetwork.CountOfPlayers == 1)
            {
                playerIsDone = true;
                synPlaneLog.text = "No player";
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (!once1)
                {
                    //synPlaneLog.text = "this is master";
                    idealPlaneForMaster = FindObjectOfType<ARPlane>(); //ideal plane that is used for current device

                    if (idealPlaneForMaster == null)
                    {
                        synPlaneLog.text = "ideal is null at save master";
                    }

                    Vector3 planeNormal = idealPlaneForMaster.normal;
                    //photonView.RPC("UpdateIdealPlaneForMaster", RpcTarget.All, idealPlaneForMaster.center, Quaternion.LookRotation(planeNormal), idealPlaneForMaster.extents);

                    photonView.RPC("UpdateIdealPlaneForMaster", RpcTarget.All);
                    //photonView.RPC("UpdateIdealPlaneForMaster", RpcTarget.All, idealPlaneForMaster);

                    once1 = true;
                }
            }
            else
            {
                if(!once2)
                {
                    //synPlaneLog.text = "this is player";

                    idealPlaneForPlayer = FindObjectOfType<ARPlane>(); //ideal plane that is used for current device

                    if (idealPlaneForPlayer == null)
                    {
                        synPlaneLog.text = "ideal is null at save player";
                    }

                    Vector3 planeNormal = idealPlaneForPlayer.normal;
                    //photonView.RPC("UpdateIdealPlaneForPlayer", RpcTarget.All, idealPlaneForPlayer.center, Quaternion.LookRotation(planeNormal), idealPlaneForPlayer.extents);

                    photonView.RPC("UpdateIdealPlaneForPlayer", RpcTarget.All);
                    //photonView.RPC("UpdateIdealPlaneForPlayer", RpcTarget.All, idealPlaneForPlayer);

                    once2 = true;
                }          
            }

            if (masterIsDone && playerIsDone) //if two player find ideal plane
            {
                if(!once3)
                {
                    synPlaneLog.text = "Plane Finding is done, you can spawn object";

                    /*if (PhotonNetwork.IsMasterClient)
                    {
                        //photonView.RPC("SpawningPlayerPlane", RpcTarget.All);

                        GameObject newPlane = Instantiate(idealPlanePrefab, centerForPlayer, rotationForPlayer);
                        //newPlane.transform.SetParent(aRSessionOrigin.transform, false);
                        newPlane.transform.localScale = extentsForPlayer;

                        //newPlane.transform.position = idealPlaneForPlayer.center;

                        if (newPlane == null)
                        {
                            synPlaneLog.text += "it is null at master";
                        }
                        else
                        {
                            synPlaneLog.text += "Spawing Plane for Player";
                        }
                    }
                    else
                    {
                        //photonView.RPC("SpawningMasterPlane", RpcTarget.All);

                        GameObject newPlane = Instantiate(idealPlanePrefab, centerForMaster, rotationForMaster);
                        //newPlane.transform.SetParent(aRSessionOrigin.transform, false);
                        newPlane.transform.localScale = extentsForMaster;

                        //newPlane.transform.position = idealPlaneForMaster.center;

                        if (newPlane == null)
                        {
                            synPlaneLog.text += "it is null at Player";
                        }
                        else
                        {
                            synPlaneLog.text += "Spawing Plane for Master";
                        }
                    }*/

                    once3 = true;
                }
            }

            if(once3)
            {
                if (CanSpawn == 1&& Input.touchCount > 0)
                {
                    checkPlaneLog.text = "You can spawn the box. Please touch the screen where you want to place box";

                    Touch touch = Input.GetTouch(0);

                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        return;
                    }

                    List<ARRaycastHit> hits = new List<ARRaycastHit>();

                    if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                    {
                        Pose hitPose = hits[0].pose;
                        GameObject spawn = MasterManager.Networkinstantiate(cube, hitPose.position, hitPose.rotation);

                        ShowAndroidToastMessage("Spawned");

                        CanSpawn ++;
                    }
                }
            }
        }
    }

    [PunRPC]
    void UpdateIdealPlaneForMaster(Vector3 center, Quaternion rotation, Vector2 extents)
    {
        masterIsDone = true;

        centerForMaster = center;
        rotationForMaster = rotation;
        extentsForMaster = extents;

        //GameObject planeObjMasterFun = new GameObject("IdealPlaneForoMasterToPlayer");
        //ARPlane idealPlane = planeObjMasterFun.AddComponent<ARPlane>();

        //planeObjMasterFun.transform.position = center;
        //planeObjMasterFun.transform.rotation = rotation;
        //planeObjMasterFun.transform.localScale = extents;

        //idealPlaneForPlayer = idealPlane;
    }

    [PunRPC]
    void UpdateIdealPlaneForPlayer(Vector3 center, Quaternion rotation, Vector2 extents)
    {
        playerIsDone = true;

        centerForPlayer = center;
        rotationForPlayer = rotation;
        extentsForPlayer = extents;

        //GameObject planeObjPlayerFun = new GameObject("IdealPlaneForPlayerToMaster");
        //ARPlane idealPlane = planeObjPlayerFun.AddComponent<ARPlane>();

        //planeObjPlayerFun.transform.position = center;
        //planeObjPlayerFun.transform.rotation = rotation;
        //planeObjPlayerFun.transform.localScale = extents;

        //idealPlaneForMaster = idealPlane;
    }

    [PunRPC]
    void UpdateIdealPlaneForMaster(GameObject idealPlanSaved)
    {
        planeForMasterToSpawn = idealPlanSaved;
        //idealPlaneForPlayer = idealPlanSaved.GetComponent<ARPlane>();
        masterIsDone = true;
    }

    [PunRPC]
    void UpdateIdealPlaneForPlayer(GameObject idealPlanSaved)
    {
        planeForPlayerToSpawn = idealPlanSaved;
        //idealPlaneForMaster = idealPlanSaved.GetComponent<ARPlane>();
        playerIsDone = true;
    }


    [PunRPC]
    void UpdateIdealPlaneForMaster()
    {
        masterIsDone = true;
    }

    [PunRPC]
    void UpdateIdealPlaneForPlayer()
    {
        playerIsDone = true;
    }

    [PunRPC]
    void SpawningMasterPlane()
    {
        GameObject newPlane = Instantiate(idealPlanePrefab, idealPlaneForMaster.center, Quaternion.identity);
        newPlane.transform.SetParent(aRSessionOrigin.transform, false);
        newPlane.transform.position = idealPlaneForMaster.center;

        if (newPlane == null)
        {
            synPlaneLog.text += "it is null at master";
        }
        else
        {
            synPlaneLog.text += "Spawing Plane for Player";
        }
    }

    [PunRPC]
    void SpawningPlayerPlane()
    {
        GameObject newPlane = Instantiate(idealPlanePrefab, idealPlaneForPlayer.center, Quaternion.identity);
        newPlane.transform.SetParent(aRSessionOrigin.transform, false);
        newPlane.transform.position = idealPlaneForPlayer.center;

        if (newPlane == null)
        {
            synPlaneLog.text += "it is null at Player";
        }
        else
        {
            synPlaneLog.text += "Spawing Plane for Master";
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new NotImplementedException();
    }
}
