using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float smoothness = 0.5f; // Dodajemy nową zmienną do określenia płynności ruchu
    [SerializeField] GameObject[] respawnPoints;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.transform.position = respawnPoints[GameClient.id%respawnPoints.Length].transform.position;
    }

    void FixedUpdate()
    {
        float speedX = Input.GetAxisRaw("Horizontal") * movementSpeed;
        float speedY = Input.GetAxisRaw("Vertical") * movementSpeed;
        Vector2 targetVelocity = new Vector2(speedX, speedY);
        // Używamy Vector2.SmoothDamp do płynnej zmiany prędkości
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothness);
    }
}
