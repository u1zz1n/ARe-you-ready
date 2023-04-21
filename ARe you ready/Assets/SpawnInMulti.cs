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
    //static public bool CanChangeColor = false;

    //public Text checkPlaneLog;
    public Text synPlaneLog;
    public Image pannelImage;
    float pannelTime = 0f;
    float startTime = 0f;
    bool disTime = false;

    ARPlane idealPlaneForMaster;
    ARPlane idealPlaneForPlayer;

    GameObject planeForMasterToSpawn;
    GameObject planeForPlayerToSpawn;

    GameObject spawn;

    Vector3 centerForMaster;
    Quaternion rotationForMaster;
    Vector2 extentsForMaster;

    Vector3 centerForPlayer;
    Quaternion rotationForPlayer;
    Vector2 extentsForPlayer;

    Vector3 centerDiffFromMaster;

    public static bool masterIsDone = false;
    public static bool playerIsDone = false;

    bool checkAllDoen = false;
    bool once1 = false;
    bool once2 = false;
    bool once3 = false;

    public event Action CanPlayScene;

    int red = 0;
    int blue = 0;
    [SerializeField]
    Text C_text;
    [SerializeField]
    Text T_text;


    bool gameStart = false;
    static public bool result = false;

    float currentTime = 0f;
    float startingTIme = 10f;
    [SerializeField]
    Text time;

    static public bool playBGM = false;
    bool readySfx = false;
    bool resultSfx = false;

    public void OnClick_StartGame()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        photonView.RPC("gameStartUpdate", RpcTarget.All);
    }


    public void OnClick_Count()
    {
        GameObject[] interactionCube = GameObject.FindGameObjectsWithTag("Interaction");
        foreach (var objs in interactionCube)
        {
            if (objs.GetComponent<MeshRenderer>().material.color == Color.red)
            {
                red++;
            }
            else if (objs.GetComponent<MeshRenderer>().material.color == Color.blue)
            {
                blue++;
            }
        }

        //C_text.text = "Red: " + red.ToString() + " Blue: " + blue.ToString();

    }
    public void OnClick_Spawn()
    {
        SoundManager.instance.PlaySfx("UI_Press");

        photonView.RPC("PlaySfx", RpcTarget.All, "Placement");
        photonView.RPC("canSpawnPlus", RpcTarget.All);
        //CanSpawn++;
    }

    void Start()
    {
        foreach (ARPlane plane in aRPlaneManager.trackables)
        {
            Destroy(plane);
        }

        FilteredPlane.isBig = false;

        //checkPlaneLog.text = "Wait until detecting your world.\n If you can't find the plane for too long \n" + "Please, restart it or move your camera";

        if (PhotonNetwork.IsMasterClient)
        {
            T_text.text = "Team Red";
            T_text.color = Color.red;
        }

        else
        {
            T_text.text = "Team Blue";
            T_text.color = Color.blue;
        }


        masterIsDone = false;
        playerIsDone = false;
        checkAllDoen = false;

        once1 = false;
        once2 = false;
        once3 = false;

        CanSpawn = 0;
        gameStart = false;
        currentTime = startingTIme;
        result = false;
        playBGM = false;
        readySfx = false;
        resultSfx = false;
        //CanChangeColor = false;
    }

    void Update()
    {
        //checkPlaneLog.text = "Plane Check : Finding...";
        if (!playBGM)
        {
            synPlaneLog.text = "Wait until detecting your world.\n Please stand facing the same direction\n If you can't find the plane for too long \n" + "Please, restart it or move your camera";
        }

        if (FilteredPlane.isBig)
        {
            if(!playBGM)
            {
                synPlaneLog.text = "Plane found.\n Please wait a moment \n";
            }

            foreach (ARPlane plane in aRPlaneManager.trackables)
            {
                if (plane.extents.x * plane.extents.y >= FilteredPlane.dismenstionsForBigPlanes.x * FilteredPlane.dismenstionsForBigPlanes.y)
                {
                    aRPlaneManager.enabled = false;

                    if (SceneManager.GetActiveScene().name == "Multiplayer")
                    {
                        CanPlayScene.Invoke();
                    }

                    //checkPlaneLog.text = "Plane Check : Done!" + "Then press 'spawn' to spawn box";
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
                //synPlaneLog.text = "No player";
            }

            if (!masterIsDone || !playerIsDone) //if two player find ideal plane
            {
                synPlaneLog.text = "Please wait for another player to find a plane\n";
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (!once1)
                {
                    //synPlaneLog.text = "this is master";
                    idealPlaneForMaster = FindObjectOfType<ARPlane>(); //ideal plane that is used for current device

                    //if (idealPlaneForMaster == null)
                    //{
                    //    synPlaneLog.text = "ideal is null at save master";
                    //}

                    Vector3 planeNormal = idealPlaneForMaster.normal;
                    //photonView.RPC("UpdateIdealPlaneForMaster", RpcTarget.All, idealPlaneForMaster.center, Quaternion.LookRotation(planeNormal), idealPlaneForMaster.extents);

                    photonView.RPC("UpdateIdealPlaneForMaster", RpcTarget.All);
                    //photonView.RPC("UpdateIdealPlaneForMaster", RpcTarget.All, idealPlaneForMaster);

                    once1 = true;
                }
            }
            else
            {
                if (!once2)
                {
                    //synPlaneLog.text = "this is player";

                    idealPlaneForPlayer = FindObjectOfType<ARPlane>(); //ideal plane that is used for current device

                    //if (idealPlaneForPlayer == null)
                    //{
                    //    synPlaneLog.text = "ideal is null at save player";
                    //}

                    Vector3 planeNormal = idealPlaneForPlayer.normal;
                    //photonView.RPC("UpdateIdealPlaneForPlayer", RpcTarget.All, idealPlaneForPlayer.center, Quaternion.LookRotation(planeNormal), idealPlaneForPlayer.extents);

                    photonView.RPC("UpdateIdealPlaneForPlayer", RpcTarget.All);
                    //photonView.RPC("UpdateIdealPlaneForPlayer", RpcTarget.All, idealPlaneForPlayer);

                    once2 = true;
                }
            }

            if (CanSpawn == 2 && !gameStart)
            {
                synPlaneLog.text = "Press the game start button to start the game";
            }

            if (masterIsDone && playerIsDone) //if two player find ideal plane
            {
                if (!playBGM)
                {
                    synPlaneLog.text = "All players are ready.\n Press the spawn button to spawn the game object";
                }

                if (gameStart)
                {
                    if(!playBGM)
                    {
                        synPlaneLog.text = "Click on the cube for a given time to change it to your own color \n The Player who turns more cubes into own color after the end of time wins.";

                        if (PhotonNetwork.IsMasterClient && !disTime)
                        {
                            if (!readySfx)
                            {
                                readySfx = true;
                                photonView.RPC("PlaySfx", RpcTarget.All, "Ready");
                            }
                            pannelTime += Time.deltaTime;

                            if (pannelTime > 5)
                            {
                                //photonView.RPC("PlaySfx", RpcTarget.All, "Start");
                                photonView.RPC("timeUpdate", RpcTarget.All);
                            }
                        }
                    }

                    if (disTime)
                    {
                        if (!playBGM)
                        {
                            playBGM = true;
                            photonView.RPC("PlayBGM", RpcTarget.All);
                        }
                        if (!result)
                        {
                            synPlaneLog.gameObject.SetActive(false);
                            pannelImage.gameObject.SetActive(false);
                        }

                        if (currentTime != 0)
                        {
                            currentTime -= 1 * Time.deltaTime;
                            time.text = currentTime.ToString("0");

                            photonView.RPC("UpdateText", RpcTarget.All);
                        }

                        if (currentTime <= 0)
                        {
                            currentTime = 0;
                        }

                        if (currentTime == 0 && !result)
                        {
                            photonView.RPC("StopBGM", RpcTarget.All);
                            result = true;
                            OnClick_Count();
                            photonView.RPC("WhosWinner", RpcTarget.All);
                        }
                    }
                }


                if (CanSpawn == 1 && Input.touchCount > 0)
                {
                    //checkPlaneLog.text = "You can spawn the box. Please touch the screen where you want to place box";

                    Touch touch = Input.GetTouch(0);

                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        return;
                    }

                    List<ARRaycastHit> hits = new List<ARRaycastHit>();

                    //if (arRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                    //{
                    //    Pose hitPose = hits[0].pose;
                    spawn = MasterManager.Networkinstantiate(cube, FindObjectOfType<ARPlane>().center, Quaternion.identity);

                    //if(spawn != null)
                    //{

                    //}

                    //if (PhotonNetwork.IsMasterClient)
                    //{
                    //Vector3 centerDiff = idealPlaneForMaster.center - (spawn.GetComponent<Renderer>().bounds.center);
                    //}
                    //else
                    //{
                    //photonView.RPC("calCenterDiff", RpcTarget.All);

                    //Vector3 newCenter = idealPlaneForPlayer.transform.localPosition - centerDiffFromMaster;                            
                    //spawn.transform.position = newCenter;
                    //}

                    photonView.RPC("canSpawnPlus", RpcTarget.All);

                    //CanSpawn++;
                    //}
                }
            }

            if (result && !resultSfx)
            {
                resultSfx = true;
                synPlaneLog.gameObject.SetActive(true);
                pannelImage.gameObject.SetActive(true);

                if (time.color == Color.red)
                {
                    synPlaneLog.fontSize = 150;
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SoundManager.instance.PlaySfx("Win");
                        synPlaneLog.text = "You Win!!";
                        synPlaneLog.color = Color.red;
                    }
                    else
                    {
                        SoundManager.instance.PlaySfx("Lose");
                        synPlaneLog.text = "You Lose!!";
                        synPlaneLog.color = Color.blue;
                    }
                }
                else if (time.color == Color.blue)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SoundManager.instance.PlaySfx("Lose");
                        synPlaneLog.text = "You Lose!!";
                        synPlaneLog.color = Color.blue;
                    }
                    else
                    {
                        SoundManager.instance.PlaySfx("Win");
                        synPlaneLog.text = "You Win!!";
                        synPlaneLog.color = Color.red;
                    }
                }
                else
                {
                    synPlaneLog.text = "Draw!!";
                    synPlaneLog.color = Color.white;
                }
            }
            if (CanSpawn == 2)
            {
                //if (!PhotonNetwork.IsMasterClient)
                //{
                //spawn.transform.position = FindObjectOfType<ARPlane>().center;
                GameObject.Find("FlipTiles(Clone)").transform.position = FindObjectOfType<ARPlane>().center;
                //synPlaneLog.text = "player center : " + FindObjectOfType<ARPlane>().center.ToString();
                //}
                //else
                //{
                //synPlaneLog.text = "master center : " + FindObjectOfType<ARPlane>().center.ToString();

                //}
            }
        }
    }
    [PunRPC]
    void PlayBGM()
    {
        SoundManager.instance.PlayBGM(0);
    }

    [PunRPC]
    void StopBGM()
    {
        SoundManager.instance.StopBGM(0);
    }

    [PunRPC]
    void PlaySfx(String title)
    {
        SoundManager.instance.PlaySfx(title);
    }

    [PunRPC]
    void canSpawnPlus()
    {
        if (CanSpawn < 3)
        {
            CanSpawn++;
        }
    }

    [PunRPC]
    void timeUpdate()
    {
        disTime = true;
    }

    [PunRPC]
    void gameStartUpdate()
    {
        gameStart = true;
    }

    [PunRPC]
    void calCenterDiff(Vector3 center)
    {
        centerDiffFromMaster = center;
    }
    [PunRPC]
    void UpdateText()
    {
        time.text = currentTime.ToString("0");
    }
    [PunRPC]
    void WhosWinner()
    {
        if (red > blue)
        {
            time.text = "Red Win";
            time.color = Color.red;
        }
        else if (red < blue)
        {
            time.text = "Blue Win";
            time.color = Color.blue;
        }
        else if (red == blue)
        {
            time.text = "Both Win";
            time.color = Color.white;
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
