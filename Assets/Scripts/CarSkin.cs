using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSkin : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer srenderer;

    private void Start() {
        srenderer.sprite = sprites[PlayerPrefs.GetInt("skin")];
    }
}
