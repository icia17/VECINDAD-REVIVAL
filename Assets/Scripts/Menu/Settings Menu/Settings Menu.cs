using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    private Resolution[] resolutions;

    [Header("Settings To Save")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    private void Start() {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        HashSet<string> uniqueResolutions = new HashSet<string>();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            if (uniqueResolutions.Add(option))
            {
                options.Add(option);
            }

            if (
                resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height
            )
            {
                currentResolutionIndex = options.IndexOf(option);
            }
        }

        resolutionDropdown.AddOptions(options);

        // Load and set saved settings
        LoadSettings();
    }

    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("music", volume);
        PlayerPrefs.SetFloat("music", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxMixer.SetFloat("sfx", volume);
        PlayerPrefs.SetFloat("sfx", volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        string[] selectedResolution = resolutionDropdown.options[resolutionIndex].text.Split('x');
        int width = int.Parse(selectedResolution[0].Trim());
        int height = int.Parse(selectedResolution[1].Trim());

        Resolution resolution = resolutions.First(r => r.width == width && r.height == height);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Load music
        if (PlayerPrefs.HasKey("music"))
        {
            float volume = PlayerPrefs.GetFloat("music");
            musicMixer.SetFloat("music", volume);
            volumeSlider.value = volume;
        }

        // Load sfx
        if (PlayerPrefs.HasKey("sfx"))
        {
            float volume = PlayerPrefs.GetFloat("sfx");
            sfxMixer.SetFloat("sfx", volume);
            volumeSlider.value = volume;
        }

        // Load fullscreen
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
            Screen.fullScreen = isFullscreen;
            fullscreenToggle.isOn = isFullscreen;
        }

        // Load resolution
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex");
            resolutionDropdown.value = resolutionIndex;
            resolutionDropdown.RefreshShownValue();

            SetResolution(resolutionIndex); // Set resolution immediately
        }
    }
}