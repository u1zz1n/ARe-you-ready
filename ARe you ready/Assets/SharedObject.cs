using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SharedObject : MonoBehaviourPun, IPunObservable
{
    //[SerializeField]
    //private Camera arCamera;
    //[SerializeField]
    // Start is called before the first frame update
    private Color Sync = Color.white;
    private Color Tmpc;
    private Vector3 Tmp;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if(stream.IsWriting)
        {
            Tmpc = gameObject.GetComponent<MeshRenderer>().material.color;
            Tmp = new Vector3(Tmpc.r, Tmpc.g, Tmpc.b);

            stream.Serialize(ref Tmp);
        }
        else if(stream.IsReading)
        {
            stream.Serialize(ref Tmp);
            Sync = new Color(Tmp.x, Tmp.y, Tmp.z);
            //this.gameObject.GetComponent<MeshRenderer>().material.color = (Color)stream.ReceiveNext();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //if(!photonView.IsMine)
        //{
        //    this.gameObject.GetComponent<MeshRenderer>().material.color = Sync;
        //}

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return;
            }

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    ShowAndroidToastMessage(hitObject.transform.gameObject.name);
                    if (SpawnInMulti.CanChangeColor && hitObject.transform.gameObject/*.name*/ == this.gameObject/*"MultiInteraction(Clone)"*/)
                    {
                        if (base.photonView.IsMine)
                        {
                            ShowAndroidToastMessage("CanChanged");
                            //PhotonView phtonView = PhotonView.Get(this);
                            this.photonView.RPC("RPC_ChangeColor", RpcTarget.All);
                            //Sync = Color.red;
                            // //this.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                            //this.GetComponent<MeshRenderer>.material.color;
                        }
                    }

                }
            }
        }
    }

    [PunRPC]
    private void RPC_ChangeColor()
    {
        ShowAndroidToastMessage("IN A RPC CALL");
        //Color c = new Color(color.x, color.y, color.z);
        this.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
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
