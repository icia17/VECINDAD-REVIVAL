using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenFollower : MonoBehaviour
{
    [Header("Camera Lerp Speed")]
    [SerializeField] float lerpSpeed;
    
    private void Update() {
        if (PlayerListLists.chosenPlayer == null) return;
        
        Vector2 vision = PlayerListLists.chosenPlayer.vision.transform.position;

        transform.position = Vector2.Lerp(transform.position, vision, lerpSpeed * Time.deltaTime);
    }
}
