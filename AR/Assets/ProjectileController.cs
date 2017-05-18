using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private GameController gc;
    private GameObject target;
    private float speed;
    private float timer;
    public float damage;

	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if(timer > 5f) // destroy projectile after 15 seconds
        {
            Destroy(gameObject);
        }

        if(target == null) // player shot
        {
            Vector3 currentPos = transform.position;
            currentPos += transform.forward * speed * Time.deltaTime * gc.GetScale();
            transform.position = currentPos;
        }
        else // turret shot
        {
            transform.LookAt(target.transform);

            Vector3 currentPos = transform.position;
            currentPos += transform.forward * speed * Time.deltaTime * gc.GetScale();
            transform.position = currentPos;
        }
	}

    public void SetAttributes(GameController g, float s, GameObject t, float d)
    {
        gc = g;
        speed = s;
        target = t;
        damage = d;
    }
}
