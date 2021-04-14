using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 3;
    float distanceTravelled;
    private bool facingLeft=true;
    void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        float transformX = transform.position.x;
        float pathX=pathCreator.path.GetPointAtDistance(distanceTravelled).x;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        
        if (facingLeft && transformX< pathX)
        {
            Flip();
        }

        if (!facingLeft && transformX >pathX)
        {
            Flip();
        }
    }
    
    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
