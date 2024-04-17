/**
 * AIController Class
 * ---------------------
 * This class controls the behavior of an AI character in a Unity scene. It includes features such as wandering,
 * chasing the player, taking damage, and updating the health bar UI. The AI uses NavMeshAgent for navigation and
 * vision detection to determine whether the player is within range.
 * 
 * Public Variables:
 * - visionRange: Represents how far the AI can see.
 * - wanderRadius: The radius within the AI will choose a new destination when wandering.
 * - wanderTimer: The time before the AI will choose a new destination to wander to.
 * - maxHP: AI's maximum health value.
 * - eyes: Represents where the AI is looking.
 * - hpBarPrefab: The UI prefab for the HP bar.
 * 
 * Private Variables:
 * - hpBarInstance: Holds the instance of the hpBar prefab.
 * - currentHP: AI's current health value.
 * - navMeshAgent: The NavMeshAgent component for navigation.
 * - player: Reference to the player's location.
 * - isChasingPlayer: Tracks whether the AI has seen the player and abandoned wandering for chasing.
 * - wander: Stores a reference to the wander coroutine to ensure it can be stopped at will.
 * - chase: Stores a reference to the chase coroutine to ensure it can be started at will.
 * 
 * Methods:
 * - Start(): Called when the script is first initialized. Initializes variables and starts the wander coroutine.
 * - Update(): Called every frame. Checks for the player within vision range and triggers chase behavior.
 * - TakeDamage(float damage): Inflicts damage on the AI, updates the health bar, and destroys the AI if health drops to zero.
 * - Wander(): Coroutine that periodically picks a new wander location for the AI.
 * - RandomNavSphere(Vector3 origin, float distance, int layermask): Calculates a random location for wandering.
 * - ChasePlayer(): Coroutine that periodically updates the player's position as the next chase destination.
 * - CanSeePlayer(): Checks if the player is within the AI's vision range.
 * - OnDrawGizmos(): Visualizes where the AI is currently looking and how far it can see in the Scene view.
 * 
 * Usage:
 * Attach this script to a GameObject representing an AI character in a Unity scene.
 * Customize the public variables in the Unity Editor for specific AI properties.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIController : MonoBehaviour
{
    

    // Represents how far the AI can see.
    [SerializeField]
    public float visionRange = 8f;

    // The radius within the AI will choose a new destination when wandering.
    [SerializeField]
    public float wanderRadius = 12f;

    // The time before the AI will choose a new destination to wander to.
    [SerializeField]
    public float wanderTimer = 5f;

    // AI's maximum health value.
    [SerializeField]
    public float maxHP = 100f;

    // Represents where the AI is looking.
    public GameObject eyes;

    // The UI prefab for the HP bar.
    public HPBar hpBarPrefab;

    // Holds the instance of the hpBar prefab.
    private HPBar hpBarInstance;

    // AI's current health value.
    private float currentHP;

    // The NavMeshAgent component for navigation.
    private NavMeshAgent navMeshAgent;

    // Reference to the player's location.
    private Transform player;

    // Tracks whether the AI has seen the player and abandoned wandering for chasing.
    private bool isChasingPlayer = false;

    // Stores a reference to the wander coroutine to ensure it can be stopped at will.
    private Coroutine wander;

    // Stores a reference to the chase coroutine to ensure it can be started at will.
    private Coroutine chase;

    /**
     * Start Method
     * ------------
     * Called when the script is first initialized. Initializes variables and starts the wander coroutine.
     */
    void Start()
    {
        // Set AI's health to maximum.
        currentHP = maxHP;

        // Instantiate the HPBar prefab.
        hpBarInstance = Instantiate(hpBarPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        hpBarInstance.transform.SetParent(transform);

        // Set up the background and hp images in the HP bar UI.
        hpBarInstance.background = hpBarInstance.transform.Find("Background").GetComponent<Image>();
        hpBarInstance.hp = hpBarInstance.transform.Find("HP").GetComponent<Image>();

        // Get the NavMeshAgent component.
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Find the player in the scene and assign its transform.
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Start the wander coroutine and assign a reference to it to the wander variable.
        wander = StartCoroutine(Wander());
    }

    /**
     * Update Method
     * --------------
     * Called every frame. Checks for the player within vision range and triggers chase behavior.
     */
    void Update()
    {

        if (!isChasingPlayer)
        {
            // Check if the player is within vision range.
            if (CanSeePlayer())
            {
                // Player has been seen, stop wandering and begin chasing.
                StopCoroutine(wander);
                isChasingPlayer = true;

                // Start the chase coroutine and assign a reference to it to the chase variable.
                chase = StartCoroutine(ChasePlayer());
            }
        }
    }

    /**
     * TakeDamage Method
     * -------------------
     * Inflicts damage on the AI, updates the health bar, and destroys the AI if health drops to zero.
     * 
     * Parameters:
     * - damage: The amount of damage to be inflicted.
     */
    public void TakeDamage(float damage)
    {
        Debug.Log("Took damage");

        // Apply damage.
        currentHP -= damage;

        // Ensure health never drops below zero or goes above maximum HP.
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        // Update the HP bar.
        hpBarInstance.UpdateHPBar(currentHP, maxHP);

        // Destroy AI if health drops to zero.
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    /**
     * Wander Coroutine
     * -------------------
     * Periodically picks a new wander location for the AI.
     */
    private IEnumerator Wander()
    {
        while (true)
        {
            yield return new WaitForSeconds(wanderTimer);

            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
        }
    }

    /**
     * RandomNavSphere Method
     * ------------------------
     * Calculates a random location which is the next wander destination.
     * 
     * Parameters:
     * - origin: The center of the sphere.
     * - distance: The radius of the sphere.
     * - layermask: A Layer mask to exclude certain areas.
     */
    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layermask);

        return navHit.position;
    }

    /**
     * ChasePlayer Coroutine
     * ----------------------
     * Periodically updates the player's position as the next chase destination.
     */
    private IEnumerator ChasePlayer()
    {
        while (isChasingPlayer)
        {
            navMeshAgent.SetDestination(player.position);
            yield return null;      // Wait for the next frame.
        }
    }

    /**
     * CanSeePlayer Method
     * ---------------------
     * Checks if the player is within the AI's vision range.
     * 
     * Returns:
     * - True if the player is within vision range, otherwise false.
     */
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 direction = player.position - eyes.transform.position;

        if (Physics.Raycast(eyes.transform.position, direction, out hit, visionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("PLAYER SEEN");
                return true;
            }
        }
        return false;
    }

    /**
     * OnDrawGizmos Method
     * ---------------------
     * Visualizes where the AI is currently looking and how far it can see in the Scene view.
     */
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(eyes.transform.position, eyes.transform.forward * visionRange);
    }

}
