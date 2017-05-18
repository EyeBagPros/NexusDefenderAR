using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private GameController gc;
    private EnemyController target;
    private float speed;
    private float timer;
    private float reachingDistance = 0.5f;
    public float damage;


	// Update is called once per frame
	void Update ()
    {

        if (gc.GetState() == GameState.MENU)
            Destroy(gameObject);
        else if (gc.GetState() != GameState.PLAY)
            return;


        timer += Time.deltaTime;
        if(timer > 5f) // destroy projectile after 15 seconds
        {
            Destroy(gameObject);
        }

        if(target != null) // turret shot
            transform.LookAt(target.transform);

        Vector3 currentPos = transform.position;
        currentPos += transform.forward * speed * Time.deltaTime * gc.GetScale();
        transform.position = currentPos;

        if (target != null) // turret shot
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if(distance < reachingDistance * gc.GetScale())
            {
                target.GetComponent<EnemyController>().Hit(damage);
                gc.SpawnExplosion(target.transform);
                Destroy(gameObject);
            }
        }
    }

    public void SetAttributes(GameController g, float s, EnemyController t, float d)
    {
        gc = g;
        speed = s;
        target = t;
        damage = d;
    }
}
