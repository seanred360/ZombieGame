using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class EnemyManager : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public bool m_canMove = false;
    bool m_isMoving;
    public ThirdPersonCharacter character;
    public GameManager m_gameManager;

    void Awake()
    {
        cam = Camera.main;
        agent.updateRotation = false;
        m_gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    private void Update()
    {
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }
    }

    // play the Enemy's turn routine
    public void PlayTurn()
    {
        StartCoroutine(PlayTurnRoutine());
    }

    IEnumerator PlayTurnRoutine()
    {
        Debug.Log("zombie turn");
        yield return new WaitForEndOfFrame();

        m_canMove = true;
        int i = Random.Range(0, m_gameManager.m_houses.Count);
        agent.SetDestination(m_gameManager.m_houses[i].transform.position + new Vector3(0, 0, 2));
        //agent.SetDestination(m_gameManager.m_houses[i].transform.position);

        m_isMoving = true;
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    m_isMoving = false;
                    this.GetComponentInChildren<Animator>().SetBool("isWalking", false);
                }
            }
        }
    }
}
