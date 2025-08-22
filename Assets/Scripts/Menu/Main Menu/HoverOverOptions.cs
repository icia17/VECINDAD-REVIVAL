using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class HoverOverOptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Mesh Parent")]
    private Light2D flashlight;
    public TextMeshProUGUI textMesh;
    public bool isHovered = false;
    private Color hoverColor = Color.red;
    private float intensityVal = 0;
    private GameObject flashlightImage;
    private Coroutine realTimeUpdateCoroutine;

    private void Start() {
        flashlight = GetComponentInChildren<Light2D>();
        flashlightImage = transform.Find("Flashlight").gameObject;
        realTimeUpdateCoroutine = StartCoroutine(RealTimeUpdate());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    private void OnDisable() {
        if (realTimeUpdateCoroutine != null) {
            StopCoroutine(realTimeUpdateCoroutine);
        }
    }

    private IEnumerator RealTimeUpdate() {
        while (true) {
            if (isHovered) {
                textMesh.color = Color.Lerp(textMesh.color, hoverColor, 10 * Time.unscaledDeltaTime);
                intensityVal = Mathf.Lerp(intensityVal, 1, 10 * Time.unscaledDeltaTime);
            } else {
                textMesh.color = Color.Lerp(textMesh.color, Color.white, 10 * Time.unscaledDeltaTime);
                intensityVal = Mathf.Lerp(intensityVal, 0, 25 * Time.unscaledDeltaTime);
            }

            flashlight.intensity = intensityVal;

            if (intensityVal < 0.025) {
                flashlightImage.SetActive(false);
            } else {
                flashlightImage.SetActive(true);
            }

            yield return null;
        }
    }
}
