using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    public GameObject baseExplosion;

    public void SpawnExplosion(Transform caller)
    {

        //Instantiate(explosionBase, explosionHolder.transform, false);


        /*GameObject copy = Instantiate(baseEnemy, baseEnemy.transform.position, baseEnemy.transform.rotation, transform);
        copy.SetActive(true);

        EnemyController ec = copy.GetComponentInChildren<EnemyController>();
        ec.SetTarget(nexusRadius, scale, angle);

        GameObject goblin = ec.gameObject;
        goblin.transform.Rotate(0f, angle, 0f);
        goblin.transform.Translate(Vector3.forward * distance * scale);
        goblin.transform.Rotate(0f, 180, 0f);

        return ec;

        
        GameObject proj = Instantiate(projectileBase, projectileHolder.transform, true);
        proj.SetActive(true);

        proj.transform.position = shooter.position;
        proj.transform.localScale = projectileBase.transform.localScale;

        if (target == null) // player shot
        {
            proj.transform.rotation = cam.transform.rotation;
        }
        else // turret shot
        {
            Vector3 pos = proj.transform.position;
            pos += proj.transform.up * proj.transform.localScale.y;
            proj.transform.position = pos;
        }

        ProjectileController pc = proj.GetComponent<ProjectileController>();
        pc.SetAttributes(this, projectileSpeed, target, d);
        */
    }
}
