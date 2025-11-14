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
    [SerializeField] private string typingSoundName = "TypeSound"; // Name of the sound in AudioManager
    [SerializeField] private bool playOnEveryCharacter = true; // If false, plays on every Nth character
    [SerializeField] private int soundInterval = 2; // Play sound every N characters (if playOnEveryCharacter is false)

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (WaveManager.finishTutorial)
        {
            uiText.gameObject.SetActive(false);
            return;
        }

        // Evita errores si la lista está vacía
        if (tutorialLines == null || tutorialLines.Count == 0)
        {
            Debug.LogWarning("No hay tutorial lines asignados.");
            return;
        }

        typingCoroutine = StartCoroutine(ShowLine());
    }

    IEnumerator ShowLine()
    {
        isTyping = true;
        uiText.text = "";

        // Use anchoredPosition instead of position for UI elements
        uiText.rectTransform.anchoredPosition = tutorialLines[currentLine].position;

        int charCount = 0;
        foreach (char c in tutorialLines[currentLine].text)
        {
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
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void PlayTypingSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(typingSoundName);
        }
    }

    void Update()
    {
        if (WaveManager.finishTutorial) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
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
    }

    void EndTutorial()
    {
        uiText.gameObject.SetActive(false);

        if (WaveManager.Instance != null)
            WaveManager.Instance.StartTimerCountdownAfterTutorial();
    }
}

[Serializable]
public struct TutorialLine
{
    [TextArea(2, 5)]
    public string text;
    public Vector2 position;
}