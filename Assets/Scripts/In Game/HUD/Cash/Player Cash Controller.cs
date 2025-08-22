using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCashController : MonoBehaviour
{
    TextMeshProUGUI tmp;
    public float lerpFontSize;
    float lerpCash;
    float fontSize;

    private void Start() {
        tmp = GetComponent<TextMeshProUGUI>();

        fontSize = tmp.fontSize;
    }

    private void Update() {
        float cash = GameManager.cash;

        GameManager.cash = Mathf.RoundToInt(cash);

        lerpCash = Mathf.Round(Mathf.LerpUnclamped(lerpCash, cash, Time.deltaTime * 5));

        if (Mathf.Abs(cash - lerpCash) < 25) {
            lerpCash = cash;
        }

        tmp.text = lerpCash.ToString();

        if (lerpCash != cash) {
            tmp.fontSize = Mathf.Lerp(tmp.fontSize, lerpFontSize, Time.deltaTime * 2);

            if (lerpCash < cash) {
                tmp.color = Color.Lerp(tmp.color, Color.green, Time.deltaTime * 3);
            } else {
                tmp.color = Color.Lerp(tmp.color, Color.red, Time.deltaTime * 3);
            }

        } else {
            tmp.fontSize = Mathf.Lerp(tmp.fontSize, fontSize, Time.deltaTime * 2);
            tmp.color = Color.Lerp(tmp.color, Color.white, Time.deltaTime * 3);
        }
    }
}
