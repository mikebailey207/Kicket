using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielder : MonoBehaviour
{
    //AI for fielders to chase and face the ball while not bumping into each other
    private Foot foot;
    private Ball ball;

    private float checkDistance = 100f;
    private bool chasing = false;

    [SerializeField] private float speed = 30f;
    [SerializeField] private float avoidanceRadius = 10f;      // How close before they repel each other
    [SerializeField] private float avoidanceStrength = 15f;    // How strong the repulsion is

    void Start()
    {
        foot = FindObjectOfType<Foot>();
        ball = FindObjectOfType<Ball>();
    }

    void Update()
    {
        FaceBall();

        if (foot.ballPlayed)
        {
            CheckIfBallInRange();
            if (!ball.scored)
            {
                Chase();
                AvoidOtherFielders();
            }
        }
    }

    void FaceBall()
    {
        float xDiff = ball.transform.position.x - transform.position.x;

        if (Mathf.Abs(xDiff) > 0.1f) // only flip if difference is significant to stop jittering
        {
            Vector3 scale = transform.localScale;
            scale.x = xDiff > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x); // right or left
            transform.localScale = scale;
        }
    }

    void CheckIfBallInRange()
    {
        float distance = Vector2.Distance(ball.transform.position, transform.position);
        chasing = distance <= checkDistance;
    }

    void Chase()
    {
        if (chasing)
        {
            transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, speed * Time.deltaTime);
        }
    }

    void AvoidOtherFielders()
    {
        //stop fielders bunching on the same position
        Fielder[] allFielders = FindObjectsOfType<Fielder>();
        foreach (Fielder other in allFielders)
        {
            if (other == this) continue;

            float dist = Vector2.Distance(transform.position, other.transform.position);
            if (dist < avoidanceRadius && dist > 0)
            {
                Vector2 away = (transform.position - other.transform.position).normalized;
                transform.position += (Vector3)(away * avoidanceStrength * Time.deltaTime);
            }
        }
    }
}
