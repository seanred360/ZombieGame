﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : PlayerManager
{
    public Transform followTarget;
    public bool isDead;

    private void Update()
    {
        if (isDead)
        {
            gameObject.SetActive(false);
            Debug.Log(gameObject.name + " is dead");
        }

        if (followTarget != null && agent != null)
        {
            IsMoving = true;
            agent.SetDestination(followTarget.position);

            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        IsMoving = false;
                        this.GetComponentInChildren<Animator>().SetBool("isWalking", false);
                    }
                }
            }
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
                character.Move(Vector3.zero, false, false);
            }
        }
    }
}
        
 
