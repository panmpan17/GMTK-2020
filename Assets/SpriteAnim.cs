using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnim : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    private int index;
    [SerializeField]
    private float speed;
    private float timer;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= speed) {
            timer = 0;
            if (++index >= sprites.Length)
                index = 0;
            spriteRenderer.sprite = sprites[index];
        }
    }
}
