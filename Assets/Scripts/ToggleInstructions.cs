using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInstructions : MonoBehaviour
{
    [SerializeField] private GameObject instructionsPanel;

    private bool isShowing = true; // start true as they turn on at start of first match

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

