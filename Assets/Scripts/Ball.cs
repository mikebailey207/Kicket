using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    // Script to handle the ball, deals with scoring and sends to GameManager. A few magic numbers here to clean up before continuing game after jam
    private Foot foot;

    public bool bowling = false;
    public bool scored = false;
    public bool boundary = false;

    private bool canSwing = true;
    private bool canAddScore = true;

    private static int boundaryLength = 225;
    private static int threeLength = 175;
    private static int twoLength = 125;
    private static int oneLength = 75;

    [SerializeField]
    private Animator bowlerAnim;
    [SerializeField]
    private Collider2D bowlerCollider;
    [SerializeField]
    private GameObject bowlerBall;

    private AudioSource clappingSound;

    private SpriteRenderer spriteRenderer;

    private float bowlingSpeed = 10;
    [SerializeField]
    private float waitToSeeFieldTime = 5;
    [SerializeField]
    private float waitForBowlTime = 3;


    private Rigidbody2D rb;

    private int i; // swing chooser
    private float j; // swing force
    private float angleOffset;

    private void Awake()
    {
        foot = FindObjectOfType<Foot>();
        rb = GetComponent<Rigidbody2D>();
        clappingSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        StartCoroutine(TurnOnBowl());

        GameManager.Instance.TryShowNewOver();

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
        bowlerAnim.enabled = true;
        CameraManager.Instance.CutToBowlCam();
        yield return new WaitForSeconds(waitForBowlTime);
        bowling = true;

    }

    private void Bowl()
    {
        bowlerAnim.enabled = false;
        bowlerBall.SetActive(false);
        spriteRenderer.enabled = true;

        rb.velocity = Vector2.zero;

        if (GameManager.Instance.isSwingOver)
        {
            if (GameManager.Instance.level == 1) bowlingSpeed = Random.Range(40, 60);
            else if (GameManager.Instance.level == 2) bowlingSpeed = Random.Range(40, 65);
            else bowlingSpeed = Random.Range(40, 70);
        }
        else
        {
            bowlingSpeed = Random.Range(25, 35);
        }

        angleOffset = Random.Range(-10f, 10f); // Store angle for later
        Vector2 direction = Quaternion.Euler(0, 0, angleOffset) * Vector2.down;

        rb.AddForce(direction.normalized * bowlingSpeed, ForceMode2D.Impulse);

        if (GameManager.Instance.isSwingOver)
        {
            StartCoroutine(SwingInAir());
        }
        else
        {
            StartCoroutine(ApplySpinForce());
        }

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
            bowlerCollider.enabled = true;
            yield return null;
        }
    }

   private IEnumerator ApplySpinForce()
{
    yield return new WaitForSeconds(Random.Range(0.5f, 0.8f));

    // Spin back toward the stumps
    Vector2 spinDir = angleOffset > 0 ? Vector2.left : Vector2.right;

    rb.AddForce(spinDir * j, ForceMode2D.Impulse);
        bowlerCollider.enabled = true;
    }

    private void CheckBoundary()
    {
        Vector2 middlePoint = Vector2.zero;
        float distance = Vector2.Distance(transform.position, middlePoint);

        int runs = 0;

        if (distance > boundaryLength)
        {
            // Beyond boundary – check if it was in the air
            runs = (foot.lofting && !foot.landed) ? 6 : 4;
            scored = true;
            if (canAddScore)
            {
                GameManager.Instance.ShowRunsThisBall(runs);
                GameManager.Instance.AddRuns(runs);
                clappingSound.pitch = Random.Range(0.8f, 1.2f);
                clappingSound.Play();
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
    
        int runs = 0;

        if (distance > threeLength)
        {
            runs = 3;
        }
        else if (distance > twoLength)
        {
            runs = 2;
        }
        else if (distance > oneLength)
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
            GameManager.Instance.ShowRunsThisBall(runs);
            canAddScore = false;
        }
        Invoke("NextBall", 3);
        Debug.Log("Runs scored: " + runs);
    }

    private void NextBall()
    {
        GameManager.Instance.NextBall();
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
            GameManager.Instance.ShowOutText("Bowled!");

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            bowlingSpeed *= 0.3f;
            rb.AddForce(Vector2.down * 20, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
            clappingSound.Play();
            Invoke("Out", 3);
           
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
                GameManager.Instance.gameOver = true;
                GameManager.Instance.ShowOutText("Caught!");
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                clappingSound.Play();
                Invoke("Out", 3);
                return;
            }

            // BALL is not lofted or has bounced — fielder stops it - score is added
            if (transform.localScale.x <= 2f && (!foot.lofting || foot.landed))
            {
                
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                bowling = false;
                CheckScore();
               
                return;
            }
        }
    }
 
}
