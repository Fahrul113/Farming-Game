using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{

    public void Interact()
    {
        TimeManager time = GameManager.instance.timeManager;

        if (time.currentState != TimeManager.TimeState.Normal)
        {
            return;
        }   

        time.Sleep();
        // After sleep Fadeout Animation
        time.WakeUp();
    }
}
