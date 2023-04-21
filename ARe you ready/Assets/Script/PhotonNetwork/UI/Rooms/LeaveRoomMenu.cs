using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
    private RoomsCanvases _roomsCanvas;
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvas = canvases;
    }

    public void OnClick_LeaveRoom()
    {
        SoundManager.instance.PlaySfx("UI_Press");
        PhotonNetwork.LeaveRoom(true);
        _roomsCanvas.CurrentRoomCanvas.Hide();
    }
}
