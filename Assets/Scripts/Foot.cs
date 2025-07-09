using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour
{
    public bool dragging = false;

    bool kicking = false;

    Vector3 startPos;
    Vector3 endPos;
    Vector3 mouseWorldPos;

    float kickSpeed = 40;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        
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
        kicking = true;
    }
}
