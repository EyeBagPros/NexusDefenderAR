using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public GameObject baseProjectile;

    public ProjectileController SpawnProjectile(Transform shooter, EnemyController target)
    {
        GameObject copy = Instantiate(baseProjectile, shooter.transform.position, shooter.transform.rotation, transform);
        copy.SetActive(true);

        if (target != null) // turret shot
        {
            Vector3 pos = copy.transform.position;
            pos += shooter.up * 12f;
            copy.transform.position = pos;
        }

        return copy.GetComponentInChildren<ProjectileController>();
    }

    public ProjectileController SpawnProjectile(Transform shooter, Vector3 aim, float scale)
    {
        GameObject copy = Instantiate(baseProjectile, shooter.transform.position, shooter.transform.rotation, transform);
        copy.SetActive(true);

        Vector3 pos = copy.transform.localPosition;
        pos += shooter.forward * scale;
        //pos.x += aim.x * scale;
        //pos.y += aim.y * scale;
        copy.transform.localPosition = pos;
        
        return copy.GetComponentInChildren<ProjectileController>();
    }
}
