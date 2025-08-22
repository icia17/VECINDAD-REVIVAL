using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOverTitle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMesh;
    private bool isHovered = false;
    private Color hoverColor = Color.red;
    
    [Header("Fade Speeds")]
    public float fadeIn = 10;
    public float fadeOut = 3;

    private Coroutine realTimeUpdateCoroutine;

    private void Awake() {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        if (realTimeUpdateCoroutine == null) {
            realTimeUpdateCoroutine = StartCoroutine(RealTimeUpdate());
        }
    }

    private void OnDisable() {
        if (realTimeUpdateCoroutine != null) {
            StopCoroutine(realTimeUpdateCoroutine);
            realTimeUpdateCoroutine = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovered = false;
    }
    
    private void FixedUpdate() {
        if (isHovered) {
            textMesh.color = Color.Lerp(textMesh.color, hoverColor, fadeIn * Time.deltaTime);
        } else {
            textMesh.color = Color.Lerp(textMesh.color, Color.white, fadeOut * Time.deltaTime);
        }
    }

    private IEnumerator RealTimeUpdate() {
        while (true) {
            float unscaledDeltaTime = Time.unscaledDeltaTime;

            if (isHovered) {
                textMesh.color = Color.Lerp(textMesh.color, hoverColor, fadeIn * unscaledDeltaTime);
            } else {
                textMesh.color = Color.Lerp(textMesh.color, Color.white, fadeOut * unscaledDeltaTime);
            }

            // Wait for the next frame in real-time
            yield return null;
        }
    }
}