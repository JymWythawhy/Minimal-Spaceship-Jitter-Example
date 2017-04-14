using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimalSpaceship : MonoBehaviour
{
    Vector2 force = new Vector2(3.0f, 0f); //10.0f);
    float rotationSpeed = 200.0f;
    float maxVelocity = 3.0f;

    public Vector2 TargetPosition = Vector2.zero;

    Rigidbody2D myRigidBody2D;

    Vector2 velocity
    {
        get { return myRigidBody2D.velocity; }
    }

    float currentFacing
    {
        get { return transform.rotation.eulerAngles.z; }
    }

    float currentHeading
    {
        get
        {
            if (velocity.magnitude < 0.01)
            {
                return currentFacing;
            }
            Vector2 velocityVector = velocity.normalized;
            float angle = Mathf.Atan2(velocityVector.y, velocityVector.x) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle = 360 - (-angle);
            }
            if (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }
    }

    void Awake()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        ApproachPosition();

    }


    void Thrust()
    {
        myRigidBody2D.AddRelativeForce(force, ForceMode2D.Force);

        if (velocity.magnitude > maxVelocity)
        {
            myRigidBody2D.velocity = velocity.normalized * maxVelocity;
        }
    }

    void Rotate(float angle)
    {
        Debug.Log("Frame: " + Time.frameCount + " Rotate angle: " + angle);
        float direction = 0;
        if (angle < 0)
        {
            direction = -1;
        }
        if (angle > 0)
        {
            direction = 1;
        }

        float rotation = direction * rotationSpeed;
        rotation *= Time.deltaTime;

        if (Mathf.Abs(rotation) > Mathf.Abs(angle))
        {
            transform.Rotate(0, 0, -angle);
        }
        else
        {
            transform.Rotate(0, 0, -rotation);
        }
    }

    #region Behaviour
    /// <summary>
    /// Approach a position in space. When it is past the braking distance, begin braking to stop near the position.
    /// </summary>
    void ApproachPosition()
    {
        float steeringAngle = AngleToSeek(TargetPosition);
        Debug.Log("Frame: " + Time.frameCount + " Approach Position. currentFacing: " + currentFacing + " currentHeading: " + currentHeading + " steeringAngle: " + steeringAngle);
        Rotate(GetAngleTowardsTargetAngle(steeringAngle));

        if (Mathf.Abs(currentFacing - steeringAngle) <= 0.1f)
        {
            Thrust();
        }
    }
    #endregion

    #region Steering Methods
    /// <summary>
    /// Gets the angle the spaceship should thrust at to most quickly approach the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected float AngleToSeek(Vector2 target)
    {
        Vector2 desiredVelocity = (target - (Vector2)transform.position).normalized * maxVelocity;
        Vector2 steeringVector;
        if (velocity.magnitude > maxVelocity)
        {
            steeringVector = desiredVelocity - (velocity.normalized * maxVelocity * 0.9f);
        }
        else
        {
            steeringVector = desiredVelocity - velocity;
        }

        return AngleToVector(steeringVector);
    }
    #endregion




    #region Angle methods


    /// <summary>
    /// Find the angle to turn towards a target angle from the current ship facing
    /// </summary>
    /// <param name="angle">The angle to turn towards</param>
    /// <returns>The angle needed to turn, negative indicating left</returns>
    protected float GetAngleTowardsTargetAngle(float angle)
    {
        float turnAngle = currentFacing - angle;

        if (turnAngle < -180)
        {
            turnAngle += 360;
        }
        if (turnAngle > 180)
        {
            turnAngle -= 360;
        }

        //Debug.Log(Time.frameCount + " Current Facing : " + currentFacing + " Angle: " + angle + " Turn Angle: " + turnAngle);
        return turnAngle;
    }

    public float AngleToVector(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.y, vector.x);
        angle = angle * Mathf.Rad2Deg;// - 90;
        if (angle < 0)
        {
            angle = 360 - (-angle);
        }
        return angle;
    }
    #endregion



}
