using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    //private Vector3 red = new Vector3(1, 0, 0);
    //private Vector3 blue = new Vector3(0, 1, 0);

    private Vector3 red = new Vector3(1, 0, 0);
    private Vector3 blue = new Vector3(0, 1, 0);

    public Player Player { get; private set; }
    public bool Ready = false;
    public void SetPlayerInfo(Player player)
    {
        Player = player;
        //_color = c;
        //_myCustomProperties["MyColor"] = c;
        //PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
        //if(c == red)
        _text.text = player.NickName;
        //else if (c == blue)
            //C_text.text = "blue" + player.NickName;
    }
}
