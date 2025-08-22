using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();    
    }

    public void OnBlood() {
        animator.Play("OverchargeOn");
    }
}
