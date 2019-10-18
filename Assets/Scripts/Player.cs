using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;


public class Player : TurnManager
{
    public Camera cam;
    public NavMeshAgent agent;

    public ThirdPersonCharacter character;

    public List<Follower> followers;
    public int playerNumber;
    public int health = 3;
    public bool isDead;

    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (InputEnabled)
        {
            if ((Input.GetMouseButtonDown(0)) && InputEnabled)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.tag == "House")
                    {
                        agent.SetDestination(hit.point);
                    }
                }
            }
        }
        if (!agent.pathPending && isDead == false)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    this.GetComponentInChildren<Animator>().SetBool("isWalking", false);
                }
            }
        }
        if (agent.remainingDistance > agent.stoppingDistance && isDead == false)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }

        switch (health)
        {
            case 0:
                Debug.Log("player is dead" + gameObject.name);
                m_gameManager.m_players.Remove(this);
                isDead = true;
                gameObject.SetActive(false);
                //Destroy(gameObject);
                break;
            case 1:
                followers[0].isDead = true;
                //followers.Remove(followers[0]);
                break;
            case 2:
                followers[1].isDead = true;
                //followers.Remove(followers[1]);
                break;
        }
    }

    public void ReactivateFollowers()
    {
        foreach (Follower follower in followers)
        {
            if(follower.isDead == false)
            follower.gameObject.SetActive(true);
        }     
    }
    void DeactivateFollowers()
    {
        foreach (Follower follower in followers)
        {
            follower.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "House")
            FinishTurn();
    }

    public void PlayTurn()
    {
        InputEnabled = true;
        Debug.Log(gameObject.name + "'s turn");
    }

    // override the TurnManager's FinishTurn
    public override void FinishTurn()
    {
        // tell the GameManager the PlayerTurn is complete
        base.FinishTurn();
        Debug.Log(gameObject.name + "finished turn");
        DeactivateFollowers();
        InputEnabled = false;
        gameObject.SetActive(false);
    }
}
