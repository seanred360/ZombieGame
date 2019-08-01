using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    public Camera m_cam;

    private void Awake()
    {
        m_cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + m_cam.transform.rotation * Vector3.back, m_cam.transform.rotation * Vector3.up);
    }
}
