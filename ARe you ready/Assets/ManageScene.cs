using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    public void TechDemo()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void BowlingDemo()
    {
        SceneManager.LoadScene("RollBall");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
