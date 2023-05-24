using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketControl : MonoBehaviour
{
    [SerializeField] private float maxAngularSpeed = Mathf.Infinity;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private Vector3 _forward = Vector3.forward;
    [SerializeField] private Vector3 up = Vector3.up;
    [SerializeField] private Vector3 axis = Vector3.up;
    private Transform myTransform;
    private Vector3 prePosition;
    private float currentAngularVelocity;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform;
        prePosition = myTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var nowPosition = myTransform.position;
        var delta = nowPosition - prePosition;
        prePosition = nowPosition;
        if (delta == Vector3.zero)
            return;
        var offsetRot = Quaternion.Inverse(Quaternion.LookRotation(_forward, up));
        var forward = myTransform.TransformDirection(_forward);
        var projectFrom = Vector3.ProjectOnPlane(forward, axis);
        var projectTo = Vector3.ProjectOnPlane(delta, axis);
        var diffAngle = Vector3.Angle(projectFrom, projectTo);
        var rotAngle = Mathf.SmoothDampAngle(
            0,
            diffAngle,
            ref currentAngularVelocity,
            smoothTime,
            maxAngularSpeed
            );
        var lookFrom = Quaternion.LookRotation(projectFrom);
        var lookTo = Quaternion.LookRotation(projectTo);
        var nextRot = Quaternion.RotateTowards(lookFrom, lookTo, rotAngle) * offsetRot;
        myTransform.rotation = nextRot;
    }
}
