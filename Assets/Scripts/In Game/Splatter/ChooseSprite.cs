using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChooseSprite : MonoBehaviour
{
    SpriteRenderer sprite;
    [SerializeField] private SplatterConfigurationSO config;

    void Start()
    {
        if (TryGetComponent(out sprite))
        {
            int randomSprite = Random.Range(0,config.sprites.Length - 1);
            sprite.sprite = config.sprites[randomSprite];
        }
    }
}
