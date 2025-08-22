using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : MonoBehaviour
{
    Material[] materials;
    ClosestTarget closestTarget;
    float fade = 0;
    public SpriteRenderer[] sprites;
    public LineRenderer line;
    public float invisibilityTreshhold;


    private void Start() {
        closestTarget = GetComponent<ClosestTarget>();

        materials = new Material[sprites.Length + 1];

        for(int i = 0; i < sprites.Length; i++) {
            materials[i] = sprites[i].material;
        }

        materials[sprites.Length] = line.material;
    }

    private void FixedUpdate() {
        if (closestTarget.closestPlayer == null) { return; }

        if (Vector2.Distance(transform.position, closestTarget.closestPlayer.transform.position) < invisibilityTreshhold) {
            fade = Mathf.Lerp(fade, 1, Time.deltaTime);
        } else {
            fade = Mathf.Lerp(fade, 0, Time.deltaTime);
        }

        foreach(var material in materials) {
            material.SetFloat("_Fade", fade);
        }
    }
}
