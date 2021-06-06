using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class Follower : MonoBehaviour
{
    [SerializeField] public Path[] paths;
    [SerializeField] public EndOfPathInstruction endOfPathInstruction;

    [Header("Debug Variables")] [Space(20)]
    [SerializeField] [ReadOnly] private int currentPath = 0;
    [SerializeField] [ReadOnly] private float currentSpeed = 0.0f;
    [SerializeField] [ReadOnly] private float distanceTravelled;
    [SerializeField] [ReadOnly] private bool alreadyCollided = false;
    [SerializeField] [ReadOnly] private CharacterController2D characterToGuide;
    [SerializeField] [ReadOnly] private CircleCollider2D butterflyCollider;

    private bool facingLeft = true;


    void Start()
    {
        butterflyCollider = gameObject.GetComponent<CircleCollider2D>();
        currentSpeed = paths[currentPath].minSpeed;

        if (paths[currentPath] != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            paths[currentPath].pathCreator.pathUpdated += OnPathChanged;
        }
    }

    void Update()
    {
        if (distanceTravelled < paths[currentPath].distanceWithMaxSpeed)
        {
            if (currentSpeed < paths[currentPath].maxSpeed)
            {
                currentSpeed += paths[currentPath].acceleration;
            }
        }
        else
        {
            if (currentSpeed > paths[currentPath].minSpeed)
            {
                currentSpeed -= paths[currentPath].acceleration;
            }
        }
        
        distanceTravelled += currentSpeed * Time.deltaTime;
        float transformX = transform.position.x;
        float pathX = paths[currentPath].pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction).x;
        transform.position = paths[currentPath].pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);

        if (facingLeft && transformX < pathX)
        {
            Flip();
        }

        if (!facingLeft && transformX > pathX)
        {
            Flip();
        }
    }
    
    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = paths[currentPath].pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !alreadyCollided)
        {
            if (currentPath < paths.Length - 1)
            {
                alreadyCollided = true;
                distanceTravelled = 0.0f;
                currentSpeed = paths[currentPath].minSpeed;
                currentPath++;
                butterflyCollider.offset = paths[currentPath].colliderOffset;
                butterflyCollider.enabled = false;

                StartCoroutine(WaitCoroutine(paths[currentPath].delay));
            }
        }
    }
    IEnumerator WaitCoroutine(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        alreadyCollided = false;
        butterflyCollider.enabled = true;
    }
}

[System.Serializable]
public class Path 
{
    [SerializeField] public PathCreator pathCreator;
    [SerializeField] public float delay = 0.0f;
    [SerializeField] public Vector2 colliderOffset;
    [SerializeField] public float maxSpeed = 4.0f;
    [SerializeField] public float minSpeed = 2.5f;
    [SerializeField] public float acceleration = 0.005f;
    [SerializeField] public float distanceWithMaxSpeed = 0.0f;
}