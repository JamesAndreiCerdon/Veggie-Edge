using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Spawner : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)]
    public float bombChance = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    private bool isSpawning = true;  // Flag to control spawning

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (isSpawning)  // Only start spawning if it's allowed
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled && isSpawning)  // Check if spawning is allowed
        {
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }

    // Stop spawning fruits when called
    public void StopSpawning()
    {
        isSpawning = false;  // Set flag to false to stop spawning
        StopAllCoroutines(); // Stop any ongoing spawn coroutines
        Debug.Log("Spawning stopped.");
    }

    // Start spawning fruits again when called
    public void StartSpawning()
    {
        isSpawning = true;  // Set flag to true to allow spawning
        StartCoroutine(Spawn());  // Restart the spawn coroutine
        Debug.Log("Spawning started.");
    }

    // This method adjusts the spawn rate (min and max spawn delays) as the level increases
    public void IncreaseSpawnRate(int level)
    {
        // Example: Decrease the spawn delay to make the game more difficult as the level increases
        float newMinDelay = Mathf.Max(0.1f, minSpawnDelay - 0.05f * level);  // Prevent going below 0.1 seconds
        float newMaxDelay = Mathf.Max(0.25f, maxSpawnDelay - 0.05f * level); // Prevent going below 0.25 seconds

        minSpawnDelay = newMinDelay;
        maxSpawnDelay = newMaxDelay;

        Debug.Log("Spawn rate increased. Min delay: " + minSpawnDelay + ", Max delay: " + maxSpawnDelay);
    }
}
