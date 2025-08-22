using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    private Animator canvasAnimator;
    private Animator playButton;
    private RectTransform mainMenu;
    private RectTransform settings;

    void Start()
    {
        AudioManager.Instance.PlayMusic("0");
        AudioManager.Instance.audioMixer.SetFloat("lowpass", 5000);

        Screen.fullScreen = true;
        
        sceneLoader.PreloadScene("In Game");

        mainMenu = transform.Find("Main Menu Anchor").gameObject.transform.Find("Main Menu").gameObject.GetComponent<RectTransform>();

        canvasAnimator = GetComponent<Animator>();  
        playButton = mainMenu.GetComponent<Animator>();

        settings = transform.Find("Settings Menu").gameObject.GetComponent<RectTransform>();
    }

    public void PlayGame()
    {
        playButton.Play("Play");
    }

    public void QuitGame() {
        Application.Quit();
    
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void SettingsEnter() {
        canvasAnimator.Play("ToSettings");
    }

    public void SettingsExit() {
        canvasAnimator.Play("ToMainMenu");
    }
}
