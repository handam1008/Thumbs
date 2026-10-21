using System;
using AJH.Scripts;
using UnityEngine;

public class Customers : MonoBehaviour
{
    CustomerStateMachine  stateMachine;

    private Rigidbody rb;
    

    private void Awake()
    {
        stateMachine = new CustomerStateMachine(this);
    }
}
