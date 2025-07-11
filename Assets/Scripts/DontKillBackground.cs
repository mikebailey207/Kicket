using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontKillBackground : MonoBehaviour
{
    private static DontKillBackground dkb;
    // Start is called before the first frame update
    void Awake()
    {
        if (dkb == null)
        {
            dkb = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
