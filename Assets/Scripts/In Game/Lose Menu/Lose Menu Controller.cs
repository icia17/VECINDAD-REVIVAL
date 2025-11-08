using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenuController : MonoBehaviour
{
    public void OpenAnimation() {
        AudioManager.Instance.PlaySFX("Click");

        Animator animator = GetComponent<Animator>();

        animator.Play("Open");
    }

    public void QuitToTitleAnimation() {
        AudioManager.Instance.PlaySFX("Click");

        Animator animator = GetComponent<Animator>();

        animator.SetBool("quit", true);
    } 

    public void RetryAnimation() {
        AudioManager.Instance.PlaySFX("Click");

        Animator animator = GetComponent<Animator>();

        animator.SetBool("retry", true);
    } 

    public void QuitToTitleAction() {
        AudioManager.Instance.PlaySFX("Click");

        SceneManager.LoadScene("Main Menu");
    }

    public void RetryAction() {
        AudioManager.Instance.PlaySFX("Click");
        
        GameManager.cash = 3000;

        SceneManager.LoadScene("In Game");
    }
}
