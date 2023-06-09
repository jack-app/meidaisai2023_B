using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomPlanet : MonoBehaviour
{
    [SerializeField] private float bomTimer;
    [SerializeField] GameObject explosionPrefab;
    private PlanetManager planetManager;
    private Planet planet;
    private RocketControl rocketControl;
    private GameObject rocket;
    private bool timerOn = false;

    private void PlanetDestroy()
    {
        planetManager.RemovePlanetForList(planet);
        rocketControl.RemovePlanetFromColList(this.gameObject);
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        rocket = GameObject.Find("Rocket");
        planetManager = GameObject.Find("PlanetManager").GetComponent<PlanetManager>();
        rocketControl = rocket.GetComponent<RocketControl>();
        planet = GetComponent<Planet>();
        if(this.transform.lossyScale.x >=299)
        {
            bomTimer = bomTimer * 1.5f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rocketControl.crash) return;
        float distance = (this.transform.position - rocket.transform.position).magnitude;
        if(distance <= planet.planetRadius * 2)
        {
            timerOn = true;
        }
        if (timerOn)
        {
            bomTimer -= Time.deltaTime;
        }
        if(bomTimer <= 0)
        {
            if (distance <= planet.planetRadius * 2)
            {
                rocketControl.resultCauseOfDeath = 4;//死因：星の爆発に巻き込まれた。
                rocketControl.RocketDestroy();
            }
            PlanetDestroy();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        PlanetDestroy();
    }
}
