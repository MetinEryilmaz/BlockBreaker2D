using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { set; get; }
    
    public float moveSpeed;
    public float movementLimitX;
    public bool autoMovementLimitX;
    public float ballMaxAngleLimit;

    [Space(20)]
    [Header("ScriptReferances")]
    [SerializeField] GameManager gameManager;

    private void Awake()
    {
        Instance = this;

        if (autoMovementLimitX) //Adjust the movement limit X value based on the screen width
        {
            movementLimitX = (Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f) - (GetComponent<BoxCollider2D>().bounds.size.x / 2);
        }
    }

    private void Update()
    {
        if (gameManager.gamePhase == GamePhase.InGame)
        {
            float horizontalMovement = Input.GetAxis("Horizontal");

            if ((horizontalMovement > 0 && transform.position.x < movementLimitX) || (horizontalMovement < 0 && transform.position.x > -movementLimitX)) //if player is inside movement limits
            {
                transform.position += Vector3.right * horizontalMovement * moveSpeed * Time.deltaTime;
            }
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //When ball collides with the player, this will find where the ball collided and calculates a new rotation angle bethween the current angle and an angle based on where the collision occured relative to the player
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball collidedBall = collision.gameObject.GetComponent<Ball>();

            Vector2 playerPosition = transform.position;
            Vector2 contactPoint = collision.GetContact(0).point;
            float distance = playerPosition.x - contactPoint.x;
            float playerWidth = GetComponent<BoxCollider2D>().bounds.size.x / 2;

            float currentBallAngle = Vector2.SignedAngle(Vector2.up, collidedBall.rb.velocity);
            float bounceAngle = (distance / playerWidth) * ballMaxAngleLimit;

            float newAngle = Mathf.Clamp(currentBallAngle + bounceAngle, -ballMaxAngleLimit, ballMaxAngleLimit);

            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            collidedBall.rb.velocity = rotation * Vector2.up * collidedBall.rb.velocity.magnitude;
        }
    }
}
