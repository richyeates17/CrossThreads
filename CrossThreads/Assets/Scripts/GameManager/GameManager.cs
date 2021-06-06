using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.IO;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

    //Variables
    public AudioMixer audioMixer;
    public int currentGraphicQuality = 4; //Ultra


    //Unity Functions
    private void Awake()
    {
        //Create a Singleton instance of this class
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {

    }

    //SceneManagement
    public void ChangeScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
    }

    public void StartBattleScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
    }

    public void StartMenuScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
    }

    //Pause Management
    public void Pause(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Additive);
    }

    //Save/Load System
    public void SaveGame()
    {

    }

    public void LoadGame()
    {

    }

    //Audio Management
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    //Graphics Quality
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    //Exit the Game
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
