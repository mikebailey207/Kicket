using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetField : MonoBehaviour
{
   
    [SerializeField] List<Transform> fielderTransforms; // e.g. 9 fielders
   
    [SerializeField] float minRadius = 60f;
    [SerializeField] float maxRadius = 300f;

    private Vector2 fieldCenter;

    private void Start()
    {
        fieldCenter = Vector2.zero;
        PositionFielders();
    }

    public void PositionFielders()
    {
        //Cut field into 9 slices, put a fielder in each slice and move them backwards or forwards randomly and sideways slightly. 
        //Called every new ball from GameManager, ensures random fielder positions for each new ball
        int count = fielderTransforms.Count;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float baseAngle = i * angleStep;
            float angleOffset = Random.Range(-angleStep / 3f, angleStep / 3f); // Add variation
            float angle = baseAngle + angleOffset;

            float radius = Random.Range(minRadius, maxRadius);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(
                fieldCenter.x + Mathf.Cos(rad) * radius,
                fieldCenter.y + Mathf.Sin(rad) * radius
            );

            fielderTransforms[i].position = pos;
        }
    }
}