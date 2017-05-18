using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private GameController gc;

    // Stats
    private GameObject target;
    public float damage;
    public float attackSpeed;
    public float range;

    private float attackTimer;
    

	void Start ()
    {
        target = null;
        attackTimer = 0f;
    }
	
	void Update ()
    {
        if (gc == null || gc.GetState() != GameState.PLAY)
            return;

        if (target == null)
        {
            target = gc.GetClosestTarget(transform, range);
        }
        else
        {
            float distance = Vector3.Distance(transform.position, target.transform.position) * gc.GetScale();
            target = (distance <= range) ? target : null; // check target still in range
        }

        if (target == null)
            return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= (1f / attackSpeed))
        {
            EnemyController ec = target.GetComponentInChildren<EnemyController>();
            if (ec.Targetable())
            {
                ec.incomingDamage += damage;
                attackTimer -= (1f / attackSpeed);
                gc.SpawnProjectile(transform, target, damage);
            }
            else
            {
                target = null;
            }
        }
    }

    public void SetAllAttributes(GameController g, float d, float s, float r)
    {
        gc = g;
        damage = d;
        attackSpeed = s;
        range = r;
    }

    public void UpdateDamage(float d)
    {
        damage = d;
    }
    public void UpdateAttackSpeed(float s)
    {
        attackSpeed = s;
    }
    public void UpdateRange(float r)
    {
        range = r;
    }
}
