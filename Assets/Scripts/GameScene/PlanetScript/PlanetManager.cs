using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] private Planet planetPrefab = null!;
    [SerializeField] private Planet bomPlanetPrefab = null!;
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
            Planet prefab = random.Next(100) <= 30 ? bomPlanetPrefab : planetPrefab;
            Planet planet = Instantiate(prefab, planetParent.transform);
            planetList.Add(planet);
            int planetSize = planet.Initialize(minSpawnableRange, firstDeg + deg * count, large);
            maxPlanet = maxPlanet < planetSize ? planetSize : maxPlanet;
        }
        minSpawnableRange += maxPlanet;
    }
    private void Spawn()
    {
        Planet prefab = random.Next(100) <= 30 ? bomPlanetPrefab : planetPrefab;
        Planet planet = Instantiate(prefab, planetParent.transform);
        planetList.Add(planet);
        int planetSize = planet.Initialize(minSpawnableRange);
        minSpawnableRange += planetSize;
    }


    public void PlanetSpawn(float rocketDistance)
    {
        while (minSpawnableRange <= rocketDistance + 10000)
        {
            if (random.Next(100) <= 3 * (1 + Mathf.Pow(rocketDistance, 4) ) && random.Next(100) < Mathf.Pow(minSpawnableRange, 1f/2f)) //¶¬‚·‚é‚©”»’è
            {
                int spawnCount = 1;
                if (random.Next(100) <= Mathf.Pow(minSpawnableRange, 1f / 3f)) spawnCount++;
                else Spawn();
                if (random.Next(1000) <= Mathf.Pow(minSpawnableRange, 1f / 3f)) spawnCount++;
                if (random.Next(10000) <= Mathf.Pow(minSpawnableRange, 1f / 3f)) spawnCount++;
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
            if (planet.orbitRadius + planet.planetRadius * 8 >= rocketDistance) break;
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
