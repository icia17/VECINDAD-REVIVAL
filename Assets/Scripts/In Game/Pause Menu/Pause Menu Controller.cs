using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Input Menu Game Objects")]
    public RectTransform pauseMenu;
    public RectTransform settingsMenu;

    private Vector3 basePausePos;
    private Vector3 baseSettingsPos;

    public bool isPaused = false;

    private void Start() {
        basePausePos = pauseMenu.anchoredPosition;
        baseSettingsPos = settingsMenu.anchoredPosition;

        settingsMenu.anchoredPosition = new Vector3(100000,0,0);
        pauseMenu.anchoredPosition = new Vector3(100000,0,0);
    }

    public void ToSettings() {
        AudioManager.Instance.PlaySFX("Click");

        pauseMenu.anchoredPosition = new Vector3(100000,0,0);
        settingsMenu.anchoredPosition = baseSettingsPos;
    }

    public void ToPauseMenu() {
        AudioManager.Instance.PlaySFX("Click");

        pauseMenu.anchoredPosition = basePausePos;
        settingsMenu.anchoredPosition = new Vector3(100000,0,0);
    }

    public void BackToGame() {
        AudioManager.Instance.PlaySFX("Click");

        Time.timeScale = 1;
        isPaused = false;
        pauseMenu.anchoredPosition = new Vector3(100000,0,0);
    }

    public void EscToGame() {
        AudioManager.Instance.PlaySFX("Click");

        Time.timeScale = 1;
        isPaused = false;
        pauseMenu.anchoredPosition = new Vector3(100000,0,0);
        settingsMenu.anchoredPosition = new Vector3(100000,0,0);
    }

    public void QuitToTitle() {
        AudioManager.Instance.PlaySFX("Click");

        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OpenPauseMenu() {
        AudioManager.Instance.PlaySFX("Click");
        
        Time.timeScale = 0;
        isPaused = true;
        pauseMenu.anchoredPosition = basePausePos;
    }
}
