using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all the movement of the snake according to the inputs given.
/// </summary>
public class SnakeMovement : MonoBehaviour {

    public float playerVelocity, playerTurnVelocity;
    public float playerHoverOffset = 0.5f;
    public Planet planet;
    public SnakeTail snakeTailPrefab;
    public AnimationCurve steeringCurve;
    public GameObject instantiatedObjects;
    public TouchIndicator touchIndicatorRight, touchIndicatorLeft;

    private Snake snake;
    private Rigidbody thisRigidbody;
    private float steerMultiplier;
    private bool leftDown, rightDown;
    private bool stopped = false;
    private RaycastHit downHit;
    private Vector3 surfaceNorm;
    private float evaluatedInput;
    private const string horizontalAxisKey = "Horizontal";

    public void Init( Snake snake ) {
        this.snake = snake;
        thisRigidbody = GetComponent<Rigidbody>();
        surfaceNorm = Vector3.zero;
    }

    private void Update() {
        if( !stopped ) {
            float distance = Vector3.Distance( planet.transform.position, transform.position );

            if( Physics.Raycast( transform.position, -transform.up, out downHit, distance ) ) {
                surfaceNorm = downHit.normal;
            }

            transform.localRotation = Quaternion.FromToRotation( transform.up, surfaceNorm ) * thisRigidbody.rotation;
            transform.localPosition = surfaceNorm * ( ( planet.transform.localScale.x / 2 ) + playerHoverOffset );
            transform.Translate( transform.forward * Time.deltaTime * playerVelocity, Space.World );

            if( Input.GetAxisRaw( horizontalAxisKey ) != 0 ) {
                if( Input.GetAxis( horizontalAxisKey ) < 0 ) {
                    evaluatedInput = steeringCurve.Evaluate( -Input.GetAxis( horizontalAxisKey ) );
                    transform.Rotate( 0, -evaluatedInput * Time.deltaTime * playerTurnVelocity, 0 );
                    touchIndicatorLeft.SetTouched();
                } else {
                    evaluatedInput = steeringCurve.Evaluate( Input.GetAxis( horizontalAxisKey ) );
                    transform.Rotate( 0, evaluatedInput * Time.deltaTime * playerTurnVelocity, 0 );
                    touchIndicatorRight.SetTouched();
                }
            } else {
                transform.Rotate( Vector3.zero );
            }
        }
    }

    /// <summary>
    /// Stop everything on game over.
    /// </summary>
    public void Stop() {
        stopped = true;
        transform.parent = planet.transform;
        instantiatedObjects.transform.parent = this.transform;
    }

    /// <summary>
    /// Resume after player has watched ad to revive.
    /// </summary>
    public void Resume() {
        stopped = false;
        transform.parent = null;
        instantiatedObjects.transform.parent = null;
    }

    public Transform GetCurrentPosition() {
        return gameObject.transform;
    }
}
