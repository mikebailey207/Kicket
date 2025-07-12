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
        if (foot.ballPlayed)
        {
            GetComponent<Collider2D>().enabled = false;
        }
    } 
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.CompareTag("Ball") && !GameManager.Instance.gameOver && !foot.ballPlayed)
        {
        
            GameManager.Instance.ballsRemaining--;
            GameManager.Instance.NextBall();
          
            SceneManager.LoadScene(0);
        }
    }
}
