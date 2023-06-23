using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int damage;
    public float speed;
    public Rigidbody2D rb;

    Vector2 previousDirection;
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        InvokeRepeating("CheckVelocity", 2, 1);
    }
    private void CheckVelocity()
    {
        if (gameManager.gamePhase == GamePhase.InGame)
        {
            //if ball gots stuck in a straight line 
            if (rb.velocity.x == 0 || rb.velocity.y == 0)
            {
                rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * (speed / 2));
            }

            //if speed goes down, adds speed with the current direction
            if (rb.velocity.magnitude < (rb.velocity.normalized * speed).magnitude)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BottomWall"))
        {
            GameManager.Instance.BallHitFloor(this);
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            collision.gameObject.GetComponent<Block>().GetHit(damage, 1);
        }
    }

    public void SendRandomDirection()
    {
        transform.parent = null;
        rb.simulated = true;
        rb.velocity = new Vector2(Random.Range(-1f, 1f), 1).normalized * speed;
    }

    /// <summary>Places the ball on the player and resets the rigidbody</summary>
    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        transform.parent = Player.Instance.transform;
        transform.localPosition = new Vector3(0, 0.4f, 0);
    }
}
