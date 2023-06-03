using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    private List<Planet> planetList = new List<Planet>();
    [SerializeField] private Planet planetPrefab = null!;
    [SerializeField] private GameObject planetParent = null!;
    private int minSpawnableRange = 300;
    private System.Random random = new System.Random();

    public void FirstSpawn()
    {
        while (minSpawnableRange <= 10000)
        {
            if (random.Next(100) <= 2)
            {
                Planet planet = Instantiate(planetPrefab, planetParent.transform);
                planetList.Add(planet);
                int planetSize = planet.Initialize(minSpawnableRange);
                minSpawnableRange += planetSize;
            }
            minSpawnableRange++;
        }
    }

    public void PlanetSpawn(float rocketDistance)
    {
        while (minSpawnableRange <= rocketDistance + 10000)
        {
            if (random.Next(100) <= 2)
            {
                Planet planet = Instantiate(planetPrefab, planetParent.transform);
                planetList.Add(planet);
                int planetSize = planet.Initialize(minSpawnableRange);
                minSpawnableRange += planetSize;
            }
            minSpawnableRange++;
        }
    }

    public void PlanetMove()
    {
        foreach (Planet planet in planetList)
        {
            planet.Move();
        }
    }

    public void PlanetDestroy(float rocketDistance)
    {
        List<int> destroyIndex = new List<int>();
        foreach (Planet planet in planetList)
        {
            if (planet.orbitRadius >= rocketDistance) break;
            //destroyIndex.Insert(0, planetList.IndexOf(planet));
        }
        foreach (int num in destroyIndex)
        {
            Planet planet = planetList[num];
            planetList.RemoveAt(num);
            planet.Destrroy();
        }
    }
}
