using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnforceConstraints : MonoBehaviour
{
    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}

