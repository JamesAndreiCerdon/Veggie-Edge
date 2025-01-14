using UnityEngine;

public class Blade : MonoBehaviour
{
    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;

    private Camera mainCamera;
    private Collider sliceCollider;  // Use Collider for 3D
    private TrailRenderer sliceTrail;

    public Vector3 direction { get; private set; }
    public bool slicing { get; private set; }

    // Reference to the GameOverManager (drag this in the Inspector)
    public GameOverManager gameOverManager;

    // Reference to the Spawner (drag the Spawner script in the Inspector)
    public Spawner fruitSpawner;  // Make sure the Spawner is referenced here

    private void Awake()
    {
        mainCamera = Camera.main;
        sliceCollider = GetComponent<Collider>();  // Use Collider for 3D
        sliceTrail = GetComponentInChildren<TrailRenderer>();

        // Null checks for components
        if (sliceCollider == null)
        {
            Debug.LogError("No Collider found on the Blade GameObject.");
        }
        if (sliceTrail == null)
        {
            Debug.LogError("No TrailRenderer found on the Blade GameObject or its children.");
        }
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSlice();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }
        else if (slicing)
        {
            ContinueSlice();
        }
    }

    private void StartSlice()
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;  // Keep the z-position flat for 2D-like game in 3D space
        transform.position = position;

        slicing = true;

        if (sliceCollider != null) sliceCollider.enabled = true;
        if (sliceTrail != null)
        {
            sliceTrail.enabled = true;
            sliceTrail.Clear();
        }
    }

    private void StopSlice()
    {
        slicing = false;

        if (sliceCollider != null) sliceCollider.enabled = false;
        if (sliceTrail != null) sliceTrail.enabled = false;
    }

    private void ContinueSlice()
    {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0f;  // Ensure the z-position stays flat
        direction = newPosition - transform.position;

        float velocity = direction.magnitude / Time.deltaTime;

        if (sliceCollider != null)
        {
            sliceCollider.enabled = velocity > minSliceVelocity;
        }

        transform.position = newPosition;
    }

    // Handle collision with bombs
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomb"))  // Ensure that your bomb prefab has the "Bomb" tag
        {
            if (gameOverManager != null)
            {
                gameOverManager.GameOver();  // Call GameOver from GameOverManager
            }

            // Stop the fruit spawner when a bomb is hit
            if (fruitSpawner != null)
            {
                fruitSpawner.StopSpawning();  // Call StopSpawning method from Spawner
            }
            else
            {
                Debug.LogError("FruitSpawner is not assigned.");
            }
        }
    }
}
