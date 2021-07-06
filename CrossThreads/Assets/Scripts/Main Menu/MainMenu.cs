using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.instance.ChangeScene("SandboxScene");
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}
