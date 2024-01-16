using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applyForce : MonoBehaviour
{
    Rigidbody rig;
    float magnitude = 5;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        rig.AddForce(Vector3.forward*magnitude, ForceMode.Impulse);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
