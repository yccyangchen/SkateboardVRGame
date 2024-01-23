using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocomotionTechnique : MonoBehaviour
{
    // Please implement your locomotion technique in this script. 
    public OVRInput.Controller leftController;
    public OVRInput.Controller rightController;
    public float movementSpeed = 10;
    public float rotationSpeed;
    public GameObject hmd;
    [SerializeField] private float leftTriggerValue;    
    [SerializeField] private float rightTriggerValue;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool isIndexTriggerDown;
    [SerializeField] private Transform skateboard;
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private float friction = 0.1f;
    [SerializeField] private float steerScale = 2f;
    [SerializeField] private Rigidbody movementRb;
    [SerializeField] private LayerMask streetLayer;
    [SerializeField] private float skateboardHeightOffset;


    /////////////////////////////////////////////////////////
    // These are for the game mechanism.
    public ParkourCounter parkourCounter;
    public string stage;
    public SelectionTaskMeasure selectionTaskMeasure;
    private float currentSkateboardAngle;
    
    void Start()
    {
        movementRb = GetComponent<Rigidbody>();
    }

// Add one button (alternative gesture) to increase the friction, so when I press it, the speed is going down but not 0(Breaking function, emergency break)

    void Update()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Please implement your LOCOMOTION TECHNIQUE in this script :D.
        float slopeAngle = SlopeAlighment();

        leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, leftController); 
        rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, rightController);

        float steerAngle = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        currentSkateboardAngle += steerAngle * Time.deltaTime * rotationSpeed;
        skateboard.rotation = Quaternion.identity;
        skateboard.Rotate(skateboard.right, slopeAngle);

        skateboard.Rotate(skateboard.up, currentSkateboardAngle);

        if (rightTriggerValue > 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = (OVRInput.GetLocalControllerPosition(rightController));
            }
            
            offset = skateboard.forward.normalized *(OVRInput.GetLocalControllerPosition(rightController) - startPos).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);

        // Use the slide velocity to adjust my game (skateboard movement)
        float slideVelocity = offset.magnitude / Time.deltaTime;
        Debug.Log("Slide Velocity" + slideVelocity);

        //UpdateSkateboardMovement(slideVelocity);
        }
        else
        {
            if (isIndexTriggerDown)
            {
                isIndexTriggerDown = false;
                offset = Vector3.zero;
            }
        }
        //this.transform.position = this.transform.position + (offset) * translationGain;
        float currentVelocity = movementRb.velocity.magnitude;
        movementRb.velocity = skateboard.forward * currentVelocity * (1 - friction * Time.deltaTime); // inscrease friction, speed down
        movementRb.AddForce(offset * movementSpeed); // Don't go through the ground anymore, using physics

 


        ////////////////////////////////////////////////////////////////////////////////
        // These are for the game mechanism.
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter.parkourStart)
            {
                this.transform.position = parkourCounter.currentRespawnPos;
            }
        }
    }

    float SlopeAlighment()
    {
        RaycastHit hit;
        // The skateboard slides on only streetLayer, other objects will be ignored.
        if (Physics.Raycast(hmd.transform.position, Vector3.down, out hit, Mathf.Infinity, streetLayer))
        {
            //skateboard.up = hit.normal; // Aligns the skateboard with the street 

            // change height of skateboard
            Vector3 position = skateboard.position;
            position.y = hit.point.y + this.skateboardHeightOffset;
            skateboard.position = position;

            return Vector3.SignedAngle(Vector3.up, hit.normal, Vector3.right);
        }

        return 0;
    }

    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
            stage = other.gameObject.name;
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("objectInteractionTask"))
        {
            selectionTaskMeasure.isTaskStart = true;
            selectionTaskMeasure.scoreText.text = "";
            selectionTaskMeasure.partSumErr = 0f;
            selectionTaskMeasure.partSumTime = 0f;
            // rotation: facing the user's entering direction
            float tempValueY = other.transform.position.y > 0 ? 12 : 0;
            Vector3 tmpTarget = new Vector3(hmd.transform.position.x, tempValueY, hmd.transform.position.z);
            selectionTaskMeasure.taskUI.transform.LookAt(tmpTarget);
            selectionTaskMeasure.taskUI.transform.Rotate(new Vector3(0, 180f, 0));
            selectionTaskMeasure.taskStartPanel.SetActive(true);
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount += 1;
            this.GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        // These are for the game mechanism.
    }

    void UpdateSkateboardMovement(float speed)
    {
        // Assuming your skateboard has a Rigidbody component for realistic physics-based movement
        Rigidbody skateboardRigidbody = GetComponent<Rigidbody>();

        // Apply force to the skateboard based on the calculated speed
        skateboardRigidbody.AddForce(transform.forward * speed, ForceMode.Impulse);
    }
}