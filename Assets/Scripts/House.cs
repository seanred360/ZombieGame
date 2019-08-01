using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public List<Player> m_ReceivedPlayers;
    public GameObject num1, num2, num3, num4;
    int whatPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            ReceivePlayer(other.gameObject);
    }

    void ReceivePlayer(GameObject player)
    {
        m_ReceivedPlayers.Add(player.GetComponent<Player>());
        whatPlayer = player.GetComponent<Player>().playerNumber;
        switch (whatPlayer)
        {
            case 1:
                num1.SetActive(true);
                break;
            case 2:
                num2.SetActive(true);
                break;
            case 3:
                num3.SetActive(true);
                break;
            case 4:
                num4.SetActive(true);
                break;
        }
    }
}
