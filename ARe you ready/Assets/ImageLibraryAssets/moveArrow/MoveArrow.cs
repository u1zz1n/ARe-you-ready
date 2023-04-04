using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveArrow : MonoBehaviour
{
    public Button down;
    public Button up;
    public Button left;
    public Button right;
    public Button back;
    public Button front;

    public Text arrowDebug;

    [SerializeField]
    private Canvas canvas;

    Vector3 spawnPosition;

    Vector3 downPosition;
    Vector3 upPosition;
    Vector3 leftPosition;
    Vector3 rightPosition;
    Vector3 backPosition;
    Vector3 frontPosition;


    // Start is called before the first frame update
    void Start()
    {
        downPosition = down.transform.position;
        upPosition = up.transform.position;
        leftPosition = left.transform.position;
        rightPosition = right.transform.position;
        backPosition = back.transform.position;
        frontPosition = front.transform.position;

        down.gameObject.SetActive(false);
        up.gameObject.SetActive(false);
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        front.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(TrackedImageInfoMultipleManager.curSelectedObject != null)
        {
            down.gameObject.SetActive(true);
            up.gameObject.SetActive(true);
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
            front.gameObject.SetActive(true);

            //spawnPosition = TrackedImageInfoMultipleManager.curSelectedObject.transform.position;

            float xOffset = TrackedImageInfoMultipleManager.curSelectedObject.GetComponent<BoxCollider>().size.x / 2;
            float yOffset = TrackedImageInfoMultipleManager.curSelectedObject.GetComponent<BoxCollider>().size.y / 2;

            arrowDebug.text = spawnPosition.x.ToString() + " " + spawnPosition.y.ToString() + TrackedImageInfoMultipleManager.curSelectedObject.name.ToString() + TrackedImageInfoMultipleManager.imagePosition.x.ToString() + " " + TrackedImageInfoMultipleManager.imagePosition.y.ToString();
            //ShowAndroidToastMessage(spawnPosition.x.ToString() + " " + spawnPosition.y.ToString());

            //down.transform.position = new Vector3(spawnPosition.x, spawnPosition.y - yOffset - 250.0f, spawnPosition.z);
            //up.transform.position = new Vector3(spawnPosition.x, spawnPosition.y + yOffset + 250.0f, spawnPosition.z);
            //left.transform.position = new Vector3(spawnPosition.x - xOffset - 250.0f, spawnPosition.y , spawnPosition.z);
            //right.transform.position = new Vector3(spawnPosition.x + xOffset + 250.0f, spawnPosition.y , spawnPosition.z);
        }
        else
        {
            down.gameObject.SetActive(false);
            up.gameObject.SetActive(false);
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
            back.gameObject.SetActive(false);
            front.gameObject.SetActive(false);
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
}
