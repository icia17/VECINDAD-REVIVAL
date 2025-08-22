using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusAnimatorManager : MonoBehaviour
{
    static float offset;

    private void Start() {
        StartCoroutine(AnimationDelay());
    }

    private IEnumerator AnimationDelay() {
        Animator animator = GetComponent<Animator>();
        
        offset += 0.5f;

        yield return new WaitForSeconds(offset);

        animator.Play("Wiggle");
    }
}
