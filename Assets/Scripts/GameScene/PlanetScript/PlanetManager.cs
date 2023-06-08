using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] private Planet planetPrefab = null!;
    [SerializeField] private Planet bomPlanetPrefab = null!;
    [SerializeField] private Planet blackholePrefab = null!;
    [SerializeField] private GameObject planetParent = null!;
    private List<Planet> planetList = new List<Planet>();
    private int minSpawnableRange = 700;
    private System.Random random = new System.Random();

    private void Spawn(int spawnCount)
    {
        int deg = 360 / spawnCount;
        int firstDeg = random.Next(0, 360);
        int maxPlanet = 0;
        bool large = random.Next(100) < 4;
        for (int count = 0; count < spawnCount; count++)
        {
            Planet prefab;
            bool blackHole = false;
            if(minSpawnableRange >= 5000 && random.Next(400) < 2)
            {
                prefab = blackholePrefab;
                blackHole = true;
            }
            else prefab = random.Next(100) <= 30 ? bomPlanetPrefab : planetPrefab;
            Planet planet = Instantiate(prefab, planetParent.transform);
            planetList.Add(planet);
            int planetSize = planet.Initialize(minSpawnableRange, firstDeg + deg * count, large, blackHole);
            maxPlanet = maxPlanet < planetSize ? planetSize : maxPlanet;
        }
        minSpawnableRange += maxPlanet;
    }
    private void Spawn()
    {
        Planet prefab;
        bool blackhole = false;
        if (minSpawnableRange >= 15000 && random.Next(100) < 2)
        {
            prefab = blackholePrefab;
            blackhole = true;
        }
        else prefab = random.Next(100) <= 30 ? bomPlanetPrefab : planetPrefab;
        Planet planet = Instantiate(prefab, planetParent.transform);
        planetList.Add(planet);
        int planetSize = planet.Initialize(minSpawnableRange, blackhole);
        minSpawnableRange += planetSize;
    }


    public void PlanetSpawn(float rocketDistance)
    {
        while (minSpawnableRange <= rocketDistance + 30000)
        {
            if (random.Next(100) <= 3 * (1 + Mathf.Pow(rocketDistance, 4) ) && random.Next(100) < Mathf.Pow(minSpawnableRange, 1f/2f)) //¶¬‚·‚é‚©”»’è
            {
                int spawnCount = 1;
                int randomInt = random.Next(1000);
                if (random.Next(100) <= Mathf.Pow(minSpawnableRange, 1f / 3f)) spawnCount++;
                else Spawn();
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 2f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 3f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 3.5f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 4f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 4.5f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 5f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 5.4f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 5.7f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 6f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 6.3f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 6.5f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 6.7f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 6.9f)) spawnCount++;
                if (randomInt <= Mathf.Pow(minSpawnableRange, 1f / 7f)) spawnCount++;
                Spawn(spawnCount);
            }
            else minSpawnableRange++;
        }
    }

    public void PlanetMove()
    {
        foreach (Planet planet in planetList)
        {
            planet.Move();
            planet.VisualSetActive();
        }
    }

    public void RemovePlanetForList(Planet planet)
    {
        planetList.Remove(planet);
    }

    public void PlanetDestroy(float rocketDistance)
    {
        List<int> destroyIndex = new List<int>();
        foreach (Planet planet in planetList)
        {
            if (planet.orbitRadius + planet.gravityRadius >= rocketDistance) break;
            destroyIndex.Insert(0, planetList.IndexOf(planet));
        }
        foreach (int num in destroyIndex)
        {
            Planet planet = planetList[num];
            planetList.RemoveAt(num);
            planet.Destrroy();
        }
    }
}
