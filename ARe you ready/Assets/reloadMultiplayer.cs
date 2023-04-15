using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class reloadMultiplayer : MonoBehaviourPunCallbacks
{
    public void OnClick_RestartMultiPlayer()
    {
        if (base.photonView.IsMine)
        {
            this.photonView.RPC("RPC_Restart", RpcTarget.All);

        }
    }

    [PunRPC]
    private void RPC_Restart()
    {
        PhotonNetwork.LoadLevel(7);

    }
}
