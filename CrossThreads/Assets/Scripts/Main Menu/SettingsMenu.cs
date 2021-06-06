using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown graphicDropdown;
    public TMP_Dropdown resolutionDropdown;

    public Resolution[] availableResolutions;

    private void Start()
    {
        if(graphicDropdown != null)
        {
            graphicDropdown.value = GameManager.instance.currentGraphicQuality;
            graphicDropdown.RefreshShownValue();
        }

        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
           string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
           options.Add(option);

            if (availableResolutions[i].width == Screen.currentResolution.width && availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        GameManager.instance.SetVolume(volume);
    }

    public void SetQuality (int qualityIndex)
    {
        GameManager.instance.SetQuality(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        GameManager.instance.SetFullScreen(isFullScreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        GameManager.instance.SetResolution(resolutionIndex);
    }
}
