using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public GameController gc = null;

    private float timer = 0f;
    private float timeTillDeath = 0f;
	
	void Update ()
    {
		if(gc.GetState() == GameState.PLAY && timeTillDeath != 0)
        {
            timer += Time.deltaTime;

            if(timer >= timeTillDeath)
                Destroy(gameObject);
        }
	}

    public void DestroyIn(float seconds)
    {
        timeTillDeath = seconds;
    }
}
