using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class TutorialScriptG : MonoBehaviour
{
    [SerializeField] private TMP_Text uiText;

    [Header("EDIT DEL TEXTO DEL TUTORIAL")]
    [SerializeField] private List<TutorialLine> tutorialLines;
    [SerializeField] private float typingSpeed = 0.07f;
    
    [Header("TYPING SOUND")]
    [SerializeField] private string typingSoundName = "TypeSound";
    [SerializeField] private bool playOnEveryCharacter = true;
    [SerializeField] private int soundInterval = 2;

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    // Cache for optimization
    private WaitForSeconds typingDelay;
    private bool tutorialFinished = false;
    private bool inputPressed = false;
    // 'wasPaused' is no longer needed

    void Start()
    {
        if (WaveManager.finishTutorial)
        {
            uiText.gameObject.SetActive(false);
            tutorialFinished = true;
            return;
        }

        if (tutorialLines == null || tutorialLines.Count == 0)
        {
            Debug.LogWarning("No hay tutorial lines asignados.");
            tutorialFinished = true;
            return;
        }

        // Cache the WaitForSeconds to avoid creating new ones every frame
        typingDelay = new WaitForSeconds(typingSpeed);
        
        typingCoroutine = StartCoroutine(ShowLine());
    }

    IEnumerator ShowLine()
    {
        isTyping = true;
        uiText.text = "";

        // Cache the tutorial line to avoid repeated list access
        TutorialLine currentTutorialLine = tutorialLines[currentLine];
        
        uiText.rectTransform.anchoredPosition = currentTutorialLine.position;

        int charCount = 0;
        string lineText = currentTutorialLine.text;
        int textLength = lineText.Length;
        
        for (int i = 0; i < textLength; i++)
        {
            char c = lineText[i];
            uiText.text += c;
            
            // Play sound (skip spaces for better effect)
            if (c != ' ')
            {
                if (playOnEveryCharacter)
                {
                    PlayTypingSound();
                }
                else if (charCount % soundInterval == 0)
                {
                    PlayTypingSound();
                }
            }
            
            charCount++;
            
            // Check if skip was pressed during typing
            if (inputPressed)
            {
                inputPressed = false;
                uiText.text = lineText; // Complete the text immediately
                break;
            }
            
            yield return typingDelay;
        }

        isTyping = false;
    }

    void PlayTypingSound()
    {
        // Null check moved outside to avoid repeated checks
        AudioManager.Instance?.PlaySFX(typingSoundName);
    }

    void Update()
    {
        if (tutorialFinished) return;

        // If the game is paused, stop all further execution in Update.
        // 1. This prevents 'X' from being processed.
        // 2. This allows the ShowLine coroutine to "freeze" naturally
        //    because it's waiting on 'typingDelay' (a WaitForSeconds).
        if (Time.timeScale == 0)
        {
            return; 
        }

        // This code will now ONLY run if the game is NOT paused
        if (Input.GetKeyDown(KeyCode.X))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            // Signal the coroutine to skip
            inputPressed = true;
            
            // Stop the coroutine
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            // Complete the current line text
            uiText.text = tutorialLines[currentLine].text;
            isTyping = false;
        }
        else
        {
            currentLine++;

            if (currentLine < tutorialLines.Count)
            {
                typingCoroutine = StartCoroutine(ShowLine());
            }
            else
            {
                EndTutorial();
            }
        }
    }

    void EndTutorial()
    {
        tutorialFinished = true;
        uiText.gameObject.SetActive(false);

        WaveManager.Instance?.StartTimerCountdownAfterTutorial();
    }

    private void OnDestroy()
    {
        // Clean up coroutine if script is destroyed
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
    }
}

[Serializable]
public struct TutorialLine
{
    [TextArea(2, 5)]
    public string text;
    public Vector2 position;
}