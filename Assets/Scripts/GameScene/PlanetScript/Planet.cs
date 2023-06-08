using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass;
    public int planetRadius { get; private set; }
    public int gravityRadius { get; private set; }
    private System.Random random = new System.Random();
    public long orbitRadius { get; private set; } // destroyで使う

    private void VisualSetActive()
    {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(GameManager.visualGravityExist);
        this.gameObject.transform.GetChild(1).gameObject.SetActive(GameManager.visualOrbitExist);
        this.gameObject.GetComponent<TrailRenderer>().enabled = GameManager.planetTrajectoryExist;
    }

    // インスタンス化したときに呼び出される
    public int Initialize(int orbitRadius)
    {
        planetRadius = orbitRadius > 5000 && random.Next(100) <= 2 ? random.Next(300, 400) : random.Next(50, 100);
        gravityRadius = planetRadius * (int)this.gameObject.GetComponent<SphereCollider>().radius;
        this.orbitRadius = orbitRadius + planetRadius / 2;
        int randomDeg = random.Next(360);
        this.transform.localScale = new Vector3(planetRadius, planetRadius, planetRadius);
        this.transform.position = new Vector3(orbitRadius * (float)Math.Cos(randomDeg * Math.PI / 180), 0, this.orbitRadius * (float)Math.Sin(randomDeg * Math.PI / 180));
        VisualSetActive();
        return planetRadius;
    }
    public int Initialize(int orbitRadius, int randomDeg, bool large)
    {
        planetRadius = orbitRadius > 5000 && large ? random.Next(300, 400) : random.Next(50, 100);
        gravityRadius = planetRadius * (int)this.gameObject.GetComponent<SphereCollider>().radius;
        this.orbitRadius = orbitRadius + planetRadius / 2;
        this.transform.localScale = new Vector3(planetRadius, planetRadius, planetRadius);
        this.transform.position = new Vector3(orbitRadius * (float)Math.Cos(randomDeg * Math.PI / 180), 0, this.orbitRadius * (float)Math.Sin(randomDeg * Math.PI / 180));
        VisualSetActive();
        return planetRadius;
    }

    // updateから呼び出される
    // 位置を変える
    public void Move()
    {
        float rotateAngle = 1f / (10 * (orbitRadius / 1000f + 1f));
        this.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, rotateAngle);
    }

    public void Destrroy()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
}