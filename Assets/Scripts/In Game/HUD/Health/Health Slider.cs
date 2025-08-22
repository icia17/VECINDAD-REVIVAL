using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] Image image;
    Slider slider;
    float lerpRed = 15;
    float currentRed = 1;
    float baseRed;
    
    private void Start() {
        slider = GetComponent<Slider>();

        baseRed = currentRed;
    }

    private void FixedUpdate() {
        slider.value = Mathf.Lerp(slider.value, PlayerListLists.chosenPlayer.playerHealth.health, 10 * Time.deltaTime);

        if (PlayerListLists.chosenPlayer.playerHealth.bloodLust) {
            currentRed = Mathf.Lerp(currentRed, lerpRed, Time.deltaTime * 2.5f);

            image.material.SetColor("_Color", new Color(currentRed,0,0));
        } else {
            currentRed = Mathf.Lerp(currentRed, baseRed, Time.deltaTime * 2.5f);

            image.material.SetColor("_Color", new Color(currentRed,0,0));
        }
    }
}
