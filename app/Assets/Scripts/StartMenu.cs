using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public void StartApp()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
