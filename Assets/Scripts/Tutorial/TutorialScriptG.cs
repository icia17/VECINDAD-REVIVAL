using System.Collections;
using UnityEngine;
using TMPro;

public class TutorialScriptG : MonoBehaviour
{
    [SerializeField] private TMP_Text uiText;
    [Header("EDIT DEL TEXTO DEL TUTORIAL")]
    [TextArea(2,5)]
    [SerializeField] private string[] tutorialLines;
    [SerializeField] private float typingSpeed = 0.07f;
    
    

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    
    void Start()
    {
       if(WaveManager.finishTutorial)
       {
            uiText.gameObject.SetActive(false);
            return;
       }
        
       typingCoroutine = StartCoroutine(ShowLine());
        
        
        
        
    }

    IEnumerator ShowLine()
    {
        isTyping = true;
        uiText.text = "";

        foreach (char c in tutorialLines[currentLine])
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
                uiText.text = tutorialLines[currentLine];
                isTyping = false;
            }
            else
            {
                
                currentLine++;

                if (currentLine < tutorialLines.Length)
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