using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Planet planetPrefab = null!;
    [SerializeField] private GameObject planetParent = null!;
    [SerializeField] private RocketControl rocketControl = null!;
    [SerializeField] private PlanetManager planetManager = null!;

    void Start()
    {
        Application.targetFrameRate = 60;
        planetManager.FirstSpawn();
    }

    void Update()
    {
        float rocketDistance = rocketControl.NowPosition.magnitude;
        bool _inOrbit = rocketControl.inOrbit;
        if(!_inOrbit) planetManager.PlanetDestroy(rocketDistance);
        planetManager.PlanetSpawn(rocketDistance);
        planetManager.PlanetMove();
    }
}
