/**
 * CharacterAnimator Class
 * ------------------------
 * This class controls the character animation based on the movement speed of a GameObject with a NavMeshAgent component.
 * It utilizes an Animator component to adjust the animation based on the agent's velocity and speed.
 * 
 * Public Variables:
 * - speedDampener: A coefficient to adjust the animation speed. Lower values result in smoother transitions.
 * 
 * Private Variables:
 * - animator: The Animator component attached to the GameObject.
 * - agent: The NavMeshAgent component attached to the GameObject.
 * 
 * Methods:
 * - Start(): Called when the script is first initialized. Initializes the NavMeshAgent and Animator components.
 * - Update(): Called every frame. Adjusts the character animation based on the movement speed.
 * 
 * Usage:
 * Attach this script to a GameObject with a NavMeshAgent component and an Animator component in a Unity scene.
 * Customize the public variables in the Unity Editor for specific animation properties.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    // A coefficient to adjust the animation speed. Lower values result in smoother transitions.
    public float speedDampener;

    // The Animator component attached to the GameObject.
    private Animator animator;

    // The NavMeshAgent component attached to the GameObject.
    private NavMeshAgent agent;

    /**
     * Start Method
     * ------------
     * Called when the script is first initialized. Initializes the NavMeshAgent and Animator components.
     */
    void Start()
    {
        // Get the NavMeshAgent component.
        agent = GetComponent<NavMeshAgent>();

        // Get the Animator component attached to the child GameObject.
        animator = GetComponentInChildren<Animator>();

        // Set a default value for the speed dampener.
        speedDampener = 0.1f;
    }

    /**
     * Update Method
     * --------------
     * Called every frame. Adjusts the character animation based on the movement speed.
     */
    void Update()
    {
        // Calculate the animation threshold based on the agent's velocity and speed.
        float animationThreshold = agent.velocity.magnitude / agent.speed;

        // Set the animation parameter in the Animator component.
        animator.SetFloat("animationThreshold", animationThreshold, speedDampener, Time.deltaTime);
    }
}
