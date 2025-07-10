using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    private Foot foot;
    public bool bowling = false;
    public bool stoppedByFielder = false;

    private bool canSwing = true;

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
            rb.AddForce(swingDir * swingForce * 20 * Time.deltaTime, ForceMode2D.Force);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ApplySpinForce()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 0.8f));
        // delay before spinning, hitting ground

        Vector2 swingDir = (i == 0) ? Vector2.left : Vector2.right;
        rb.AddForce(swingDir * j, ForceMode2D.Impulse);
    }
    private void OnTriggerEnter2D(Collider2D collision)
      {
          if (collision.gameObject.CompareTag("Foot"))
          {
              rb.velocity = Vector2.zero;
              rb.angularVelocity = 0;
              bowling = false;
          }
      }
  /*  private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Foot"))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            bowling = false;
        }
    }*/
    /*private void OnCollisionStay2D(Collision2D collision)
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
                Debug.Log("Caught out!");
                SceneManager.LoadScene(0);
                return;
            }

            // BALL is not lofted or has bounced — fielder stops it
            if (transform.localScale.x <= 2f && (!foot.lofting || foot.landed))
            {
                Debug.Log("Fielder stops the ball");
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                bowling = false;
                return;
            }
        }
    }*/
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
                Debug.Log("Caught out!");
                SceneManager.LoadScene(0);
                return;
            }

            // BALL is not lofted or has bounced — fielder stops it
            if (transform.localScale.x <= 2f && (!foot.lofting || foot.landed))
            {
                Debug.Log("Fielder stops the ball");
                stoppedByFielder = true;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                bowling = false;
                return;
            }
        }
    }
 
}
