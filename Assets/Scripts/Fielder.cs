using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielder : MonoBehaviour
{
    private Foot foot;

    private Ball ball;

    private float checkDistance = 100;
    private bool chasing = false;

 

    private float speed = 30;

    void Start()
    {
        foot = FindObjectOfType<Foot>();
        ball = FindObjectOfType<Ball>();
      
    }

    void Update()
    {
        if(foot.ballPlayed)
        {
            CheckIfBallInRange();
            Chase();
        }     
    }

    void CheckIfBallInRange()
    {
        float distance = Vector2.Distance(ball.transform.position, transform.position);
     
        if(distance <= checkDistance)
        {
            chasing = true;
        }
        else
        {
            chasing = false;
        }
    }

    void Chase()
    {
        if (chasing)
        {
            transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, speed * Time.deltaTime);
            //Flip
            Vector3 scale = transform.localScale;

            if (ball.transform.position.x > transform.position.x)
                scale.x = -Mathf.Abs(scale.x); // face right
            else
                scale.x = Mathf.Abs(scale.x); // face left

            transform.localScale = scale;
        }
    }
}
