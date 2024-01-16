 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;

public class MoveCube : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private bool pressYRaw;
    [SerializeField] private Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 axis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick,controller);
        transform.Translate(new Vector3(axis.x, 0, axis.y) * speed * Time.deltaTime);

        if(OVRInput.Get(OVRInput.Button.One)) transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if(OVRInput.Get(OVRInput.Button.Two)) transform.Translate(Vector3.back * speed * Time.deltaTime);

        pressYRaw = OVRInput.Get(OVRInput.RawButton.Y);
         
    }
}
