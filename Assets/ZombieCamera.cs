using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCamera : MonoBehaviour
{
    public Transform target;
    
    // Update is called once per frame
    void Update()
    {
        float step = 5f * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
