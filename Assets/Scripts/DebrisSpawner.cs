using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Blackhole))]
public class DebrisSpawner : MonoBehaviour
{
    [SerializeField] List<Debris> debris;

    [SerializeField] float defaultMinDist;
    [SerializeField] float defaultMaxDist;

    float minDist;
    float maxDist;

    public bool spawnOnAwake = true;

    Blackhole blackhole;

    private void Awake()
    {
        blackhole = GetComponent<Blackhole>();

        minDist = defaultMinDist;
        maxDist = defaultMaxDist;
    }
    public void SpawnRandomDebris()
    {
        //get random direction
        float spawnDirX = Random.Range(-1.0f, 1.0f);
        float spawnDirY = Random.Range(-1.0f, 1.0f);

        //direction to spawn debris
        Vector2 spawnPos = new Vector2(spawnDirX, spawnDirY).normalized;

        //get random rotation
        float randomRotation = Random.Range(0f, 360f);

        //weighted chance
        float totalWeight = 0;
        for (int i = 0; i < debris.Count; i++)
        {
            totalWeight += debris[i].spawnWeight;
        }

        //get random debris
        float randomItem = Random.Range(0, totalWeight);

        //get debris index
        float checkWeight = 0;
        int randomIndex = 0;
        for (int i = 0; i < debris.Count; i++)
        {
            checkWeight += debris[i].spawnWeight;
            if(randomItem <= checkWeight)
            {
                randomIndex = i;
                break;
            }
        }

        //get debris game object
        GameObject debrisToSpawn = debris[randomIndex].debris[Random.Range(0, debris[randomIndex].debris.Length)];

        //get random spawn distance
        float randomDist = Random.Range(minDist, maxDist);

        //spawn debris
        blackhole.Add(Instantiate(debrisToSpawn, transform.position + new Vector3(spawnPos.x * randomDist, spawnPos.y * randomDist, 0), Quaternion.Euler(new Vector3(0, 0, randomRotation))));
    }

    public void ExtendSpawner(float _blackholeRad)
    {
        minDist = _blackholeRad * 2;
        maxDist = minDist + _blackholeRad;

        minDist = Mathf.Clamp(minDist, defaultMinDist, Mathf.Infinity);
        maxDist = Mathf.Clamp(maxDist, defaultMaxDist, Mathf.Infinity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, minDist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDist);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            SpawnRandomDebris();
        }
    }
}
