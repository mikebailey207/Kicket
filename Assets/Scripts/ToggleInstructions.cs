using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInstructions : MonoBehaviour
{
    [SerializeField] private GameObject instructionsPanel;

    private bool isShowing = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isShowing = !isShowing;
            instructionsPanel.SetActive(isShowing);
            Time.timeScale = isShowing ? 0f : 1f;
        }
    }
    
}

