using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Blackhole))]
[RequireComponent(typeof(ScoreManager))]
public class DebrisSpawner : MonoBehaviour
{
    [SerializeField] List<Debris> debris;

    [SerializeField] float defaultMinDist;
    [SerializeField] float defaultMaxDist;

    [SerializeField] float defaultMaxMinDist;
    [SerializeField] float defaultMaxMaxDist;

    float minDist;
    float maxDist;

    public bool spawnOnAwake = true;
    public bool isSpawning = true;

    [HideInInspector]
    public Blackhole blackhole;
    [HideInInspector]
    public ScoreManager score;

    [SerializeField] float spawnInterval;
    [SerializeField] float spawnRamp;
    [SerializeField] float minInterval;

    public static DebrisSpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance);
        }

            blackhole = GetComponent<Blackhole>();

        minDist = defaultMinDist;
        maxDist = defaultMaxDist;

        defaultMaxMinDist = blackhole.maxBlackholeSize * blackhole.maxBlackholeSize;
        defaultMaxMaxDist = blackhole.maxBlackholeSize + defaultMaxMinDist;
    }
    private void Start()
    {
        if (spawnOnAwake) StartCoroutine(StartAutoSpawn());
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

        //set points
        debrisToSpawn.GetComponent<Consumable>().SetPoints(debris[randomIndex].points);

        //spawn debris
        blackhole.Add(Instantiate(debrisToSpawn, transform.position + new Vector3(spawnPos.x * randomDist, spawnPos.y * randomDist, 0), Quaternion.Euler(new Vector3(0, 0, randomRotation))));
    }
    public void SpawnDebris(int _index, Vector3 _position, int _amount)
    {
        while(_amount > 0)
        {
            //get random direction
            float spawnDirX = Random.Range(-1.0f, 1.0f);
            float spawnDirY = Random.Range(-1.0f, 1.0f);

            //direction to spawn debris
            Vector2 spawnPos = new Vector2(spawnDirX, spawnDirY).normalized;

            //get random rotation
            float randomRotation = Random.Range(0f, 360f);

            //get debris game object
            GameObject debrisToSpawn = debris[_index].debris[Random.Range(0, debris[_index].debris.Length)];

            //get random spawn distance
            float randomDist = Random.Range(0, 1.5f);

            //spawn debris
            blackhole.Add(Instantiate(debrisToSpawn, _position + new Vector3(spawnPos.x * randomDist, spawnPos.y * randomDist, 0), Quaternion.Euler(new Vector3(0, 0, randomRotation))));

            //increment
            _amount--;
        }
    }

    public void ExtendSpawner(float _blackholeRad)
    {
        minDist = _blackholeRad * _blackholeRad;
        maxDist = minDist + _blackholeRad;

        minDist = Mathf.Clamp(minDist, defaultMinDist, defaultMaxMinDist);
        maxDist = Mathf.Clamp(maxDist, defaultMaxDist, defaultMaxMaxDist);
    }
    public void SetDefaultDist(float _maxSize)
    {
        defaultMinDist = _maxSize * _maxSize;
        defaultMaxDist = defaultMinDist + _maxSize;
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

    IEnumerator StartAutoSpawn()
    {
        while (isSpawning)
        {
            SpawnRandomDebris();

            if (spawnInterval <= minInterval)
            {
                spawnInterval = minInterval;
            }

            yield return new WaitForSeconds(spawnInterval);

            spawnInterval -= spawnRamp * Time.deltaTime;
        }
    }
}
