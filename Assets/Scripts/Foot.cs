using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Foot : MonoBehaviour
{
    public bool dragging = false;

    bool kicking = false;

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

    void Start()
    {
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
    }
    private void Kick()
    {        
        transform.position = Vector3.MoveTowards(transform.position, startPos, kickSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, startPos) < 0.01f)
        {
            kicking = false;
        }
    }
    private void OnMouseDown()
    {
        startPos = transform.position;
        dragging = true;
    }
    private void OnMouseUp()
    {
        dragging = false;
   
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
            // Calculate direction opposite to the drag direction
            Vector2 dragDirection = (transform.position - startPos).normalized;
            Vector2 forceDirection = dragDirection;

            float forceMagnitude = kickSpeed/2; // Adjust force strength as needed

            rb.AddForce(-forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
    }
}
