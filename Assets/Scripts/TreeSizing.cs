using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSizing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        //Randomize tree sizes each ball. Should really be each match, but this is fine for now. 
        spriteRenderer = GetComponent<SpriteRenderer>();
        float scale = Random.Range(3, 6);
        transform.localScale = new Vector3(scale, scale, scale);
        //make sure ball is behind them
        spriteRenderer.sortingOrder = 2000;

    }
}
