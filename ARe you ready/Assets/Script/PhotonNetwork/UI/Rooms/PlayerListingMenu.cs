using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;

    [SerializeField]
    private PlayerListing _playerListing;

    [SerializeField]
    private Text _readyUpText;

    [SerializeField]
    private Button readyButton;

    [SerializeField]
    private Text readyCountText;

    [SerializeField]
    private Button startButton;

    private bool _ready = false;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomsCanvases _roomsCanvases;

    static public ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable();


    static public List<Vector3> colors = new List<Vector3>();

    public void OnClick_Color()
    {
        if(colors.Count == 0)
        {
            //colors.Add(red);
            //_myCustomProperties["MyColor"] = red;
           // PhotonNetwork.MasterClient.CustomProperties = _myCustomProperties;
            //Vector3 ColorGet = (Vector3)PhotonNetwork.MasterClient.CustomProperties["MyColor"];
            //Color Red = new Color(ColorGet.x, ColorGet.y, ColorGet.z);
            //colorselect.color = Red;
        }
        else if(colors.Count == 1)
        {
            //colors.Add(blue);
            //_myCustomProperties["MyColor"] = blue;
            //PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperties;
            //Vector3 ColorGet = (Vector3)PhotonNetwork.LocalPlayer.CustomProperties["MyColor"];
            //Color Blue = new Color(ColorGet.x, ColorGet.y, ColorGet.z);
            //colorselect.color = Blue;
        }
    }

    private void Awake()
    {
        GetCurrentRoomPlayers();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SetReadyUp(false);
    }

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    private void SetReadyUp(bool state)
    {
        _ready = state;
        if (_ready)
            _readyUpText.text = "Ready";
        else
            _readyUpText.text = "NotReady";
    }

    public override void OnLeftRoom()
    {
        _content.DetachChildren();
    }

    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Player player)
    {
        PlayerListing listing = Instantiate(_playerListing, _content);
        if (listing != null)
        {
            listing.SetPlayerInfo(player);
            _listings.Add(listing);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomsCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < _listings.Count; i++)
            {
                if (_listings[i].Player != PhotonNetwork.LocalPlayer)
                {
                    if (!_listings[i].Ready)
                        return;
                }
            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.LoadLevel(7);
        }
    }

    public void OnClick_ReadyUp()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            SetReadyUp(!_ready);
            base.photonView.RPC("RPC_ChangeReadyState", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer, _ready);
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(false);

            if (_ready)
            {
                readyCountText.text = "Ready : 2/2";
            }
            else
            {
                readyCountText.text = "Ready : 1/2";
            }
        }
    }

    [PunRPC]
    private void RPC_ChangeReadyState(Player player, bool ready)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        _listings[index].Ready = ready;

        if(ready == true)
        {
            readyCountText.text = "Ready : 2/2";
        }
        else
        {
            readyCountText.text = "Ready : 1/2";
        }
    }
}

