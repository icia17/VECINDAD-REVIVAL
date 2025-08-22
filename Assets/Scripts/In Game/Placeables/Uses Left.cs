using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsesLeft : MonoBehaviour
{
    public int usesLeft = 3;
    [SerializeField] AudioClip buildSFX;
    AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(buildSFX);
    }

    public void Use() {
        if (gameObject == null) return;
        
        if (usesLeft > 1) {
            usesLeft--;
        } else {
            Destroy(gameObject);
        }
    }
}
