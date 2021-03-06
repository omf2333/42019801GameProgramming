﻿using UnityEngine;
using System.Collections;
public enum PatrolDir
{
    Go,

    Return
}

public class PatrolEnemyAi:Enemy
{
    [SerializeField] protected Transform[] destinationList;
    protected int currDestinationIndex;
    protected int destinationNum;
    protected PatrolDir patrolDir = PatrolDir.Go;
    protected bool isHurtPlayerInPatrol;
    protected float patrolHurtingCoolTime;
    protected bool isSlowDown = false;
    protected float slowDownSpeed;

    // Use this for initialization
    void Start()
    {
        base.Init();
        currDestinationIndex = 1;
        //destinationNum = 2;
        patrolHurtingCoolTime = 1f;
    }


    private void Update()
    {
        if (destinationList != null && destinationList.Length > 0)
        {
            Move();
            if (IsPlayerInRange(attackRange))
            {
                Attack();
            }
        }
    }

    public void SetPatrolPos(Transform[] desList)
    {
        destinationList = desList;
        destinationNum = destinationList.Length;
    }

    protected void Move()
    {
        if (Vector2.Distance(transform.position, destinationList[currDestinationIndex].position) <= 0.1f)
        {
            if (patrolDir == PatrolDir.Go)
            {
                currDestinationIndex += 1;
            }
            else
            {
                currDestinationIndex -= 1;
            }
        }
        if(currDestinationIndex >= destinationNum)
        {
            if (destinationList[0].position == destinationList[destinationNum - 1].position)
            {
                currDestinationIndex = 1;
                patrolDir = PatrolDir.Go;
            }
            else
            {
                currDestinationIndex -= 2;
                patrolDir = PatrolDir.Return;
            }
        }
        else if (currDestinationIndex < 0)
        {
            currDestinationIndex += 2;
            patrolDir = PatrolDir.Go;
        }
        if (isSlowDown)
        {
            transform.position = Vector2.MoveTowards(transform.position, destinationList[currDestinationIndex].position, slowDownSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, destinationList[currDestinationIndex].position, speed * Time.deltaTime);
        }
    }

    protected Vector3 GetPreviousDestination()
    {
        if (patrolDir == PatrolDir.Go)
        {
            return destinationList[currDestinationIndex - 1].position;
        }
        else
        {
            return destinationList[currDestinationIndex + 1].position;
        }
    }

    protected IEnumerator CooldownHurtingPlayer()
    {
        isHurtPlayerInPatrol = true;
        yield return new WaitForSeconds(patrolHurtingCoolTime);
        isHurtPlayerInPatrol = false;
    }

    public override void SlowDownMovingSpeed(float slowDownRate)
    {
        if (!isSlowDown)
        {
            isSlowDown = true;
            slowDownSpeed = speed * slowDownRate;
        }
    }

    public override void FrozenMovement()
    {
        speed = 0;
    }

    public override void RecoverMovingSpeed()
    {
        isSlowDown = false;
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHurtPlayerInPatrol && collision.gameObject.tag == "Player")
        {
            Debug.Log("patrol collision");
            GameController.DamagePlayer(1);
               
        }
    }
}
