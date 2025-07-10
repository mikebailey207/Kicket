using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    private Foot foot;
    public bool bowling = false;
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
        if(bowling)
        {
            if (canSwing)
            {
                canSwing = false;
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
        bowlingSpeed = Random.Range(10, 40);
        transform.Translate(Vector2.down * Time.deltaTime * bowlingSpeed);

        if (i == 0) rb.AddForce(Vector2.left * j);
        else rb.AddForce(Vector2.right * j);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Foot"))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            bowling = false;
        }
        if (collision.gameObject.CompareTag("Fielder"))
        {
            // BALL is above the fielder – it's still lofted and cannot be interacted with
            if (transform.localScale.x > 3f)
            {
                return; // ball too high, fielder ignores it
            }

            // BALL is falling from loft but hasn't bounced yet – caught out
            if (transform.localScale.x <= 3f && foot.lofting && !foot.landed)
            {
                Debug.Log("Caught out!");
                SceneManager.LoadScene(0);
                return;
            }

            // BALL is not lofted or has bounced — fielder stops it
            if (transform.localScale.x <= 3f && (!foot.lofting || foot.landed))
            {
                Debug.Log("Fielder stops the ball");
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
                bowling = false;
                return;
            }
        }
    }
}
