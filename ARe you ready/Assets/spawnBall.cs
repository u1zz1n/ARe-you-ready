using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class spawnBall : MonoBehaviour
{
    [SerializeField]
    GameObject ball;

    [SerializeField]
    public Text debugLog;

    public void Spawn()
    {
        debugLog.text = "spawn complete"; 
        Instantiate(ball, new Vector3(0f, 1f, -8f), Quaternion.identity);
    }


}
