using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerThrow : MonoBehaviour
{
    public ItemSO thrownWeapon;
    public bool firstGrab = false;
    SpriteRenderer spriteRenderer;
    float alpha = 1;

    public void Init(ItemSO thrownWeapon) {
        GetComponents();

        this.thrownWeapon = thrownWeapon;

        spriteRenderer.sprite = thrownWeapon.itemSprite;
    }

    private void GetComponents() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        spriteRenderer.color = new Vector4(255,255,255,alpha);

        alpha -= Time.deltaTime / 30;

        if (alpha < 0.025) {
            Destroy(gameObject);
        }
    }
}
