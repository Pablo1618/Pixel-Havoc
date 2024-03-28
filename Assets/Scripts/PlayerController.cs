using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float smoothness = 0.5f; // Dodajemy nową zmienną do określenia płynności ruchu
    float speedX, speedY;
    Vector2 currentVelocity;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        speedX = Input.GetAxisRaw("Horizontal") * movementSpeed;
        speedY = Input.GetAxisRaw("Vertical") * movementSpeed;
        Vector2 targetVelocity = new Vector2(speedX, speedY);
        // Używamy Vector2.SmoothDamp do płynnej zmiany prędkości
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothness);
    }
}
