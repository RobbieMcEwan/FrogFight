using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    // When a target is found
    // What to show next?
    public GenerateImageAnchor ImageAnchorGenerator;

    private void Awake()
    {
        ImageAnchorGenerator.OnImageFound += OnImageFound;
    }

    public void OnImageFound() 
    { 
        // Show Play Panel
        // Hide Intro pnale
    }
}
