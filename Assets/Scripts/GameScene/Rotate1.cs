using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate1 : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 0.01f;

    // Update is called once per frame
    void Update()
    {
        Transform myTransform = this.transform;
        myTransform.Rotate(0, rotateSpeed, 0);
    }
}
