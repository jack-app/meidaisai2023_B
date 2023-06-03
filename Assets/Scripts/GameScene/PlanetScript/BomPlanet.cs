using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomPlanet : MonoBehaviour
{
    [SerializeField] private float bomTimer;
    [SerializeField] GameObject explosionPrefab;
    private GameObject rocket;
    private Planet planet;
    private RocketControl rocketControl;
    private bool timerOn = false;
    // Start is called before the first frame update
    void Start()
    {
        rocket = GameObject.Find("Rocket");
        rocketControl = rocket.GetComponent<RocketControl>();
        planet = GetComponent<Planet>();
    }
    // Update is called once per frame
    void Update()
    {
        float distance = (this.transform.position - rocket.transform.position).magnitude;           
        if(distance <= planet.orbitLevel1)
        {
            timerOn = true;
        }
        if (timerOn)
        {
            bomTimer -= Time.deltaTime;
        }
        if(bomTimer <= 0)
        {
            Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
            if (distance <= planet.orbitLevel1)
            {
                rocketControl.RocketDestroy();
            }
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(this);
    }
}
