using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Foot foot;
    public bool bowling = false;
    private float bowlingSpeed = 10;
    [SerializeField]
    private float waitToSeeFieldTime = 5;
    [SerializeField]
    private float waitForBowlTime = 3;

    private void Awake()
    {
        foot = FindObjectOfType<Foot>();     
    }

    private void Start()
    {
        StartCoroutine(TurnOnBowl());
    }

    void Update()
    {
        if(bowling)
        {
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bowling = false;
    }
}
