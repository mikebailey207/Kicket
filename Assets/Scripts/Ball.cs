using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    private Foot foot;

    public bool bowling = false;
    public bool scored = false;
    public bool boundary = false;

    private bool canSwing = true;
    private bool canAddScore = true;


    private float bowlingSpeed = 10;
    [SerializeField]
    private float waitToSeeFieldTime = 5;
    [SerializeField]
    private float waitForBowlTime = 3;

    private Rigidbody2D rb;

    private int i; // swing chooser
    private float j; // swing force
    private void Awake()
    {
        foot = FindObjectOfType<Foot>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(TurnOnBowl());
    }

    void Update()
    {
        if (bowling)
        {
            if (canSwing)
            {
                canSwing = false;
                bowlingSpeed = Random.Range(40, 70);
                i = Random.Range(0, 2);
                j = Random.Range(5, 10);
            }
            Bowl();
        }
        if(!scored)
        {
            CheckBoundary();
        }
    }

    private IEnumerator TurnOnBowl()
    {
        yield return new WaitForSeconds(waitToSeeFieldTime);

        CameraManager.Instance.CutToBowlCam();
        yield return new WaitForSeconds(waitForBowlTime);
        bowling = true;

    }

    private void Bowl()
    {
        rb.velocity = Vector2.zero;

        // Add strong initial downward impulse
        rb.AddForce(Vector2.down * bowlingSpeed, ForceMode2D.Impulse);

        // Start swing over time
        StartCoroutine(SwingInAir());

        bowling = false;
    }

    private IEnumerator SwingInAir()
    {
        float swingTime = 2f;         // Total time to apply swing
        float elapsed = 0f;
        float swingForce = j;           // From earlier: Random.Range(5, 10)
        Vector2 swingDir = (i == 0) ? Vector2.left : Vector2.right;

        while (elapsed < swingTime)
        {
            rb.AddForce(swingDir * swingForce * 40 * Time.deltaTime, ForceMode2D.Force);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ApplySpinForce() // use this later for spinners
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 0.8f));
        // delay before spinning, hitting ground

        Vector2 swingDir = (i == 0) ? Vector2.left : Vector2.right;
        rb.AddForce(swingDir * j, ForceMode2D.Impulse);
    }

    private void CheckBoundary()
    {
        Vector2 middlePoint = Vector2.zero;
        float distance = Vector2.Distance(transform.position, middlePoint);

        int runs = 0;

        if (distance > 225f)
        {
            // Beyond boundary – check if it was in the air
            runs = (foot.lofting && !foot.landed) ? 6 : 4;
            scored = true;
            if (canAddScore)
            {
                GameManager.Instance.AddRuns(runs);
                canAddScore = false;
            }
            Invoke("NextBall", 3);
            Debug.Log("Runs scored: " + runs);
        }
    }
    private void CheckScore()
    {
        Vector2 middlePoint = Vector2.zero;
        float distance = Vector2.Distance(transform.position, middlePoint);
        //Debug.Log(distance);
        int runs = 0;

        if (distance > 175f)
        {
            runs = 3;
        }
        else if (distance > 125f)
        {
            runs = 2;
        }
        else if (distance > 75f)
        {
            runs = 1;
        }
        else
        {
            runs = 0;
        }
        scored = true;
        if (canAddScore)
        {
            GameManager.Instance.AddRuns(runs);
            canAddScore = false;
        }
        Invoke("NextBall", 3);
        Debug.Log("Runs scored: " + runs);
    }

    private void NextBall()
    {
        SceneManager.LoadScene(0);
    }

    private void Out()
    {
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
      {
          if (collision.gameObject.CompareTag("Foot"))
          {
              rb.velocity = Vector2.zero;
              rb.angularVelocity = 0;
              bowling = false;
          }

          if(collision.gameObject.CompareTag("Stump"))
        {
            Out();
        }
      }
  
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fielder"))
        {
     
            // BALL is above the fielder – it's still lofted and cannot be interacted with
            if (transform.localScale.x > 2f)
            {
                return; // ball too high, fielder ignores it
            }

            // BALL is falling from loft but hasn't bounced yet – caught out
            if (transform.localScale.x <= 2f && foot.lofting && !foot.landed)
            {
                // Debug.Log("Caught out!");
                Out();
                return;
            }

            // BALL is not lofted or has bounced — fielder stops it
            if (transform.localScale.x <= 2f && (!foot.lofting || foot.landed))
            {
              //  Debug.Log("Fielder stops the ball");
                
              
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                bowling = false;
                CheckScore();
                return;
            }
        }
    }
 
}
