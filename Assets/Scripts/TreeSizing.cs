using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSizing : MonoBehaviour
{
    void Start()
    {
        float scale = Random.Range(3, 6);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
