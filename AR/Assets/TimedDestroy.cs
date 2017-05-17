using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public GameController gc = null;

    private float timer;
    private float timeTillDeath;

    void Start ()
    {
        timeTillDeath = timer = 0f;
	}
	
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
