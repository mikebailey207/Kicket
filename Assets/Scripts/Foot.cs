using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Foot : MonoBehaviour
{
    //class to control foot dragging and connecting. Acts on the ball's rigid body on connecting. 
    public bool dragging = false;
    public bool lofting = false;
    public bool landed = false;
    public bool canKick = true;

    private bool kicking = false;

    private LineRenderer lr;
    Vector3 startPos;

    Vector3 mouseWorldPos;

    private AudioSource hitSound;

    [SerializeField]
    private float kickSpeed = 2;
    [SerializeField]
    private float speedMultiplier = 10;
    [SerializeField]
    private float maxKickSpeed = 1000;
    [SerializeField]
    private float minKickSpeed = 0;
    [SerializeField]
    private float forceStrengthAdjuster = 10;

    [SerializeField]
    private AudioSource dashSound;

    [SerializeField]
    private TextMeshProUGUI spaceText;

    public bool ballPlayed = false;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        hitSound = GetComponent<AudioSource>();
        startPos = transform.position;
    }

    void Update()
    {
        //whacked this here at the end, not ideal but seemed like an ok place to put it
        if (Input.GetKey(KeyCode.Space))
        {
            spaceText.text = "LOFTING";
            spaceText.color = Color.red;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            spaceText.text = "HOLD SPACE TO LOFT BALL";
            spaceText.color = Color.white;
        }
        if (dragging)
        {                    
            Drag();
        }
        else if (kicking)
        {
            if (canKick)
            {             
                Kick();
            }
        }
    }
    private void Drag()
    {
        //drag the foot
        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;

        Vector2 direction = startPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, mouseWorldPos);
    }

    private void Kick()
    {        
        //release of foot foot shoots off to perform kick
        transform.position = Vector3.MoveTowards(transform.position, startPos, kickSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, startPos) < 0.01f)
        {
            kicking = false;
            canKick = false;
        }
    }

    private void KickConnect(Rigidbody2D ballRB)
    {     
        //actually kick the ball
        ballPlayed = true;
      
        // Calculate direction opposite to the drag direction
        Vector2 dragDirection = (transform.position - startPos).normalized;
        Vector2 forceDirection = dragDirection;

        float forceMagnitude = kickSpeed * forceStrengthAdjuster; 

        ballRB.AddForce(-forceDirection * forceMagnitude);

        hitSound.Play();

        CameraManager.Instance.CutToBallCam();
    }

    private IEnumerator LoftBallEffect(Transform ball)
    {
        //old school ball gets bigger it is higher a la old school cricket games
        Vector3 originalScale = ball.localScale;
        Vector3 loftScale = originalScale * 5f;
        Vector3 bounceScale = originalScale * 1.2f;

        float loftTime = 2f;
        float bounceTime = 0.1f;

        // LOFT up (grow)
        yield return ScaleOverTime(ball, originalScale, loftScale, loftTime);
        // Drop down
        yield return ScaleOverTime(ball, loftScale, originalScale, loftTime);
        // Bounce once (small scale pop)
        landed = true;
        yield return ScaleOverTime(ball, originalScale, bounceScale, bounceTime);
        yield return ScaleOverTime(ball, bounceScale, originalScale, bounceTime);
    }

    private IEnumerator ScaleOverTime(Transform target, Vector3 start, Vector3 end, float duration)
    {
        //actual scaling of ball object
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = end;
    }


    private void OnMouseDown()
    {
        startPos = transform.position;
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
        lr.enabled = false;
        kickSpeed = CalculateKickSpeed();
        kicking = true;
        dashSound.Play();
    }

    private float CalculateKickSpeed()
    {
        //more power in kick the further the foot starts from the start position before kicking
        float distance = Vector3.Distance(transform.position, startPos);
        float kickSpeed = distance * speedMultiplier;
        return Mathf.Clamp(kickSpeed, minKickSpeed, maxKickSpeed);
    }

    private void HitWicket()
    {
        //if player hits their own wicket they are out
        GameManager.Instance.ResetGame();

        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
        
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null)
            {
                KickConnect(rb);

                if (Input.GetKey(KeyCode.Space))
                {
                  
                    lofting = true;
                    StartCoroutine(LoftBallEffect(collision.transform));
                }
            
            }
        }
        else if (collision.gameObject.CompareTag("Stump"))
        {
            GameManager.Instance.ShowOutText("Out hit wicket!");

            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.down * 10f, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
            Invoke("HitWicket", 3);
        }
    }
}
