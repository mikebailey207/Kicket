using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Foot foot;
    bool bowling = false;
    private float bowlingSpeed = 10;

    private void Awake()
    {
        foot = FindObjectOfType<Foot>();     
    }

    private void Start()
    {
        Invoke("TurnOnBowl", 3);
    }

    void Update()
    {
        if(bowling)
        {
            Bowl();
        }
    }

    private void TurnOnBowl()
    {
        bowling = true;
    }

    private void Bowl()
    {
        bowlingSpeed = Random.Range(10, 20);
        transform.Translate(Vector2.down * Time.deltaTime * bowlingSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bowling = false;
    }
}
