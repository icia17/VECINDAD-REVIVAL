using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float resizeAmount;
    float value = 1;
    AudioSource audioSource;
    [SerializeField] AudioClip splat;

    private void Start() {
        audioSource = GetComponent<AudioSource>();

        int random = Random.Range(0,35);

        if (random == 0) {
            audioSource.PlayOneShot(splat);
        }
    }

    private void OnEnable() {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable() {
        Ticker.OnTickAction -= Tick;
    }

    void Tick() {
        value -= resizeAmount;

        transform.localScale = Vector3.one * value;
        
        if (value < 0) {
            Destroy(gameObject);
        }
    }
}
