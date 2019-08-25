using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerManager : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    bool m_isMoving;
    public bool IsMoving { get { return m_isMoving; } set { m_isMoving = value; } }
    public bool m_canMove = false;
    public ThirdPersonCharacter character;

    private void Awake()
    {
        cam = Camera.main;
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0)) && m_canMove)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag == "House")
                {
                    m_isMoving = true;
                    agent.SetDestination(hit.point);
                }
            }
        }
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
        if(agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        } else
        {
            character.Move(Vector3.zero, false, false);
        }
    }
}
