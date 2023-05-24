using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetInfo : MonoBehaviour
{
    public float mass;
    public float orbitLevel1;
    private Transform myTransform;
    private Vector3 prePosition;
    private Vector3 nowPosition;
    public Vector3 plaDelta;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = this.transform;
        prePosition = myTransform.position;
    }
    // Update is called once per frame
    void Update()
    {
        nowPosition = myTransform.position;
        plaDelta = nowPosition - prePosition;
        prePosition = nowPosition;
        //Debug.Log(plaDelta);
    }
}
