using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public struct PlayerPos
{
    public float x;
    public float y;
    public float rotation;
}

public class PlayerInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerPos pos;
    private int kills = 0;
    private int deaths = 0;
    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void addKill()
    {
        kills += 1;
    }

    public void addDeaths()
    {
        deaths += 1;
    }

    public int getKills() { return kills; }

    public int getDeaths() { return deaths; }

    // Update is called once per frame
    void FixedUpdate()
    {
        pos.x = rb.transform.position.x;
        pos.y = rb.transform.position.y;
        pos.rotation = rb.transform.rotation.z;
    }
}
