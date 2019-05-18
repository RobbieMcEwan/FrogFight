using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosDefenseFinder : MonoBehaviour
{

    public void StartGame()
    {
        FindObjectOfType<ChaosDefense>()?.Activate();
    }
}
