using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Game Controller reference
    public GameController gc = null;

    // Animation Component
    private Animation anim;

    // Health
    private float maxHealth = 5f;
    private float currentHealth = 5f;
    private float corpeDuration = 10f;
    public GameObject healthBarGreen;
    public GameObject healthBarRed;
    public GameObject healthBarCenter;
    public GameObject cam;


    // Movement
    public GameObject target;
    public GameObject floor;
    private float speed = 5f;
    private float reachingDist = 5f;
    private float attackSpeed = 1f;
    private float attackTimer = 0f;
    private bool moving;
    private Vector3 currentPos;
    private Vector3 destination;
    private Vector3 localDestination;

    void Start ()
    {
        moving = false;

        anim = GetComponent<Animation>();
        anim.Play("idle", PlayMode.StopAll);

        //destination = target.transform.position;
        localDestination = target.transform.localPosition;
        GoTo(target);

        healthBarCenter.transform.parent.GetComponent<LookAtTarget>().SetTarget(cam);
    }
	
	void Update ()
    {
        if (gc == null)
            return;

        switch (gc.GetState())
        {
            case GameState.TRANSITION_TO_PAUSE:
                {
                    anim.PlayQueued("idle", QueueMode.PlayNow, PlayMode.StopAll);
                    moving = false;
                    break;
                }
            case GameState.TRANSITION_TO_PLAY:
                {
                    moving = true;
                    attackTimer = 0f;
                    break;
                }
            case GameState.PLAY:
                {
                    HandleUnit();
                    break;
                }
            default: return;
        }
	}


    private void HandleUnit()
    {
        if (currentHealth <= 0f) // dead
            return;

        UpdateHealthBars();

        if (localDestination != target.transform.localPosition)
        {
            localDestination = target.transform.localPosition;
            //destination = target.transform.position;
            moving = true;
        }

        float distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance > reachingDist * gc.GetScale())
        {
            if (moving) // going to target
            {
                Move();
                anim.PlayQueued("run", QueueMode.CompleteOthers, PlayMode.StopAll);
            }
            else // target is far, but wasnt earlier
            {
                GoTo(target);
            }
        }
        else
        {
            if (moving) // target just reached
            {
                Attack();
            }
            else // target reached earlier
            {
                attackTimer += Time.deltaTime / attackSpeed;
                if (attackTimer > 1f)
                {
                    // trigger damage nexus =====================================================================
                    Debug.Log("Nexus Hit");
                    attackTimer = 0f;
                }

                anim.PlayQueued("attack3", QueueMode.CompleteOthers);
            }
        }
    }


    public void Hit(float damage)
    {
        currentHealth -= Time.deltaTime;
        if (currentHealth <= 0f)
        {
            anim.PlayQueued("death", QueueMode.PlayNow, PlayMode.StopAll);
            healthBarGreen.SetActive(false);
            healthBarRed.SetActive(false);
            transform.parent.GetComponent<TimedDestroy>().DestroyIn(corpeDuration);
        }
    }

    public void SetTarget(Vector3 position)
    {
        target.transform.position = position;
    }

    private void UpdateHealthBars()
    {
        float healthPercentage = currentHealth / maxHealth;
        Vector3 tmp;

        // Reset global positions to center
        healthBarGreen.transform.position = healthBarRed.transform.position = healthBarCenter.transform.position;

        // Translate front bar
        tmp = healthBarCenter.transform.localPosition;
        tmp += Vector3.right * 0.5f * (1f - healthPercentage);
        healthBarGreen.transform.localPosition = tmp;

        // Translate back bar
        tmp = healthBarCenter.transform.localPosition;
        tmp += Vector3.right * -0.5f *  (healthPercentage);
        healthBarRed.transform.localPosition = tmp;

        // Scale front bar
        tmp = healthBarGreen.transform.localScale;
        tmp.x = healthPercentage;
        healthBarGreen.transform.localScale = tmp;

        // Scale back bar
        tmp = healthBarRed.transform.localScale;
        tmp.x = 1f - healthPercentage;
        healthBarRed.transform.localScale = tmp;
    }

    private void Move()
    {
        // rotate
        LookAtTarget();

        // move
        currentPos = transform.position;
        currentPos += transform.forward * speed * Time.deltaTime * gc.GetScale();
        transform.position = currentPos;

        //Debug.Log("Moved" + (speed * Time.deltaTime * gc.GetScale()).ToString());
    }

    private void LookAtTarget()
    {
        // rotate unit on floor
        Vector3 lookPos = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(lookPos, floor.transform.up);
    }


    private void Attack()
    {
        moving = false;
        transform.rotation = target.transform.rotation;
        anim.PlayQueued("attack1", QueueMode.PlayNow, PlayMode.StopAll);
    }

    private void GoTo(GameObject destTarget)
    {
        if(moving)
            anim.PlayQueued("run", QueueMode.CompleteOthers, PlayMode.StopAll);
        else
            anim.PlayQueued("run", QueueMode.PlayNow, PlayMode.StopAll);

        moving = true;
        localDestination = target.transform.localPosition;
        //destination = target.transform.position;
        attackTimer = 0f;
    }

    private string GetCurrentAnim()
    {
        float bestWeight = -1f;
        string ret = "None";

        foreach (AnimationState state in anim)
        {
            if (state.enabled && state.weight > bestWeight)
            {
                ret = state.name;
                bestWeight = state.weight;
            }
        }

        return ret;
    }
}

