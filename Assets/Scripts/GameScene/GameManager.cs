using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RocketControl rocketControl = null!;
    [SerializeField] private PlanetManager planetManager = null!;
    [SerializeField] private FollowCamera followCamera = null!;

    void Start()
    {
        Application.targetFrameRate = 60;
        planetManager.FirstSpawn();
    }

    void FixedUpdate()
    {
        float rocketDistance = rocketControl.NowPosition.magnitude;
        bool _inOrbit = rocketControl.inOrbit;
        if(!_inOrbit) planetManager.PlanetDestroy(rocketDistance);
        planetManager.PlanetSpawn(rocketDistance);
        planetManager.PlanetMove();
        rocketControl.RocketUpdate();
        followCamera.CameraUpdate();
    }
}
