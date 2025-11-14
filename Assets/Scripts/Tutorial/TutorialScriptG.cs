using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class TutorialScriptG : MonoBehaviour
{
    [SerializeField] private TMP_Text uiText;

    [Header("EDIT DEL TEXTO DEL TUTORIAL")]
    [TextArea(2, 5)]
    [SerializeField] private List<TutorialLine> tutorialLines;
    [SerializeField] private float typingSpeed = 0.07f;

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

        uiText.transform.position = tutorialLines[currentLine].position;

        foreach (char c in tutorialLines[currentLine].text)
        {
            uiText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
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
