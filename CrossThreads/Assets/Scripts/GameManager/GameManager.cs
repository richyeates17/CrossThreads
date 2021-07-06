using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

    //Variables
    public AudioMixer audioMixer;
    public int currentGraphicQuality = 4; //Ultra

    //Reference Variables
    public string currentScene;
    public Player currentPlayer;

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
        currentScene = SceneToLoad;
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

    //implement player prefs and file system saving for 10 slots

    public void SaveGame(Player player, string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + fileName;
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public SaveData LoadGame(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("SaveData file not found in" + path);
            return null;
        }
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

