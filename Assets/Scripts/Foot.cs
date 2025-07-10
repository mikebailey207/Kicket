using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Foot : MonoBehaviour
{
    public bool dragging = false;
    public bool lofting = false;
    public bool landed = false;
    
    private bool kicking = false;
    private LineRenderer lr;
    Vector3 startPos;

    Vector3 mouseWorldPos;
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



    public bool ballPlayed = false;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        startPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (dragging)
        {
           
            
            Drag();
        }
        else if (kicking)
        {
            Kick();
        }
    }
    private void Drag()
    {
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
        transform.position = Vector3.MoveTowards(transform.position, startPos, kickSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, startPos) < 0.01f)
        {
            kicking = false;
        }
    }

    private void KickConnect(Rigidbody2D ballRB)
    {
        ballPlayed = true;
        // Calculate direction opposite to the drag direction
        Vector2 dragDirection = (transform.position - startPos).normalized;
        Vector2 forceDirection = dragDirection;

        float forceMagnitude = kickSpeed * forceStrengthAdjuster; // Adjust force strength as needed

        ballRB.AddForce(-forceDirection * forceMagnitude);

        CameraManager.Instance.CutToBallCam();
    }

    private IEnumerator LoftBallEffect(Transform ball)
    {
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
    }

    private float CalculateKickSpeed()
    {
        float distance = Vector3.Distance(transform.position, startPos);
        float kickSpeed = distance * speedMultiplier;
        return Mathf.Clamp(kickSpeed, minKickSpeed, maxKickSpeed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
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
}
