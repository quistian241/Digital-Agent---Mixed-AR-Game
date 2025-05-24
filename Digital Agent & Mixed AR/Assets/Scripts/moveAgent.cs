using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class moveAgent : MonoBehaviour
{
    public GameObject agent;
    private Rigidbody agentRigid;
    private float moveSpeed = 2f;
    private string rightActiveGesture = "";
    private string leftActiveGesture = "";

    // Start is called before the first frame update
    void Awake()
    {
        agentRigid = agent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement activated
        if (leftActiveGesture == "Left Hand Thumbs Up")
        {
            if (rightActiveGesture == "Right Hand Point Fowards")
            {
                agentRigid.velocity = Vector3.forward * moveSpeed;
            } else if (rightActiveGesture == "Right Hand Point Sideways")
            {
                agentRigid.velocity = -Vector3.right * moveSpeed;
            } else if (rightActiveGesture == "Right Hand Thumbs Up")
            {
                agentRigid.velocity = -Vector3.forward * moveSpeed;
            } else if (rightActiveGesture == "Right Hand Thumbs Side")
            {
                agentRigid.velocity = Vector3.right * moveSpeed;
            } else
            {
                agentRigid.velocity = Vector3.zero;
            }
        } else
        {
            agentRigid.velocity = Vector3.zero;
        }

    }

    public void MovePlayer(string gestureName)
    {
        Debug.Log("function activated " + gestureName);

        rightActiveGesture = gestureName;
    }
    
    public void MoveStateSet(string gestureName)
    {
        Debug.Log("function activated " + gestureName);

        leftActiveGesture = gestureName;
    }
}


