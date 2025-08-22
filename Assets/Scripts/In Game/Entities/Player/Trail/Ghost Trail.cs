using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public void Init(Sprite sprite) {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}
