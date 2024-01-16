using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject skateboard;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(skateboard.transform);
    }
}
