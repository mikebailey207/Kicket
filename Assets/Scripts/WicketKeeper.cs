using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WicketKeeper : MonoBehaviour
{
    private Foot foot;
    private Ball ball;
    private bool chasing = false;

    // Start is called before the first frame update
    void Start()
    {
        ball = FindObjectOfType<Ball>();
        foot = FindObjectOfType<Foot>();
    }

    // Update is called once per frame
    void Update()
    {
     /*   if (!chasing && ball.gameObject.transform.position.y <= 3 && !foot.ballPlayed)
        {
            chasing = true;
            CameraManager.Instance.CutToBallCam(); // triggers once
        }

        if (chasing)
        {
            transform.position = Vector2.MoveTowards(transform.position, ball.gameObject.transform.position, 50 * Time.deltaTime);
        }*/
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            GameManager.Instance.ballsPlayed++;
            SceneManager.LoadScene(0);
        }
    }
}
