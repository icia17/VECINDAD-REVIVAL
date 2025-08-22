using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    public static bool active;
    GameObject spawn;
    float radius;
    SpriteRenderer spriteRend;
    bool canPlace = true;
    Vector2 mousePos;

    private void Start() {
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (!active) {
            spriteRend.color = Color.clear;
            return;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = mousePos;
        
        PlaceBool();
        
        if (!canPlace) { return; }

        if (Input.GetMouseButtonDown(0)) {
            Instantiate(spawn, mousePos, Quaternion.identity);
            
            PlayerInventory inv = PlayerListLists.chosenPlayer.playerInventory;
            inv.inventory[inv.numSelect] = null;
            inv.selectedItem = null;
        }
    }

    private void PlaceBool()
    {
        if (WallCollision()) {
            spriteRend.color = Color.red;
            canPlace = false;
        } else {
            spriteRend.color = Color.green;
            canPlace = true;
        }
    }

    private bool WallCollision() {
        Collider2D collider = Physics2D.OverlapCircle(mousePos, radius);
        return collider;
    }

    public void ChangePlaceable(Sprite sprite, GameObject spawn, float radius) {
        spriteRend.sprite = sprite;
        this.spawn = spawn;
        this.radius = radius;
    }
}
