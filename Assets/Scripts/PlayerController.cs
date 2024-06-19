using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Net;
using Unity.VisualScripting;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float smoothness = 0.5f; // Dodajemy nową zmienną do określenia płynności ruchu
    [SerializeField] GameObject[] respawnPoints;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    public static UDPClientInfo clientInfo;
    public static PlayerController instance;
    public bool shouldRespawn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.transform.position = respawnPoints[GameClient.id%respawnPoints.Length].transform.position;
        clientInfo = new UDPClientInfo(GameClient.id);
        instance = this;
    }

    public void Respawn()
    {
        rb.velocity = Vector2.zero;
        int randomPoint = UnityEngine.Random.Range(0, respawnPoints.Length);
        rb.transform.position = respawnPoints[randomPoint].transform.position;
        shouldRespawn = false;
    }

    void FixedUpdate()
    {
        float speedX = Input.GetAxisRaw("Horizontal") * movementSpeed;
        float speedY = Input.GetAxisRaw("Vertical") * movementSpeed;
        Vector2 targetVelocity = new Vector2(speedX, speedY);
        // Używamy Vector2.SmoothDamp do płynnej zmiany prędkości
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothness);
        clientInfo.playerInfo.x = rb.transform.position.x;
        clientInfo.playerInfo.y = rb.transform.position.y;
        clientInfo.playerInfo.rotation = rb.transform.eulerAngles.z;
        if (shouldRespawn)
            Respawn();
    }
}
