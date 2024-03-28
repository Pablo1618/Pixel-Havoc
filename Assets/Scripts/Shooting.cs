using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab; 

    void Update()
    {
        if(player == null)
        {
            Debug.LogError("Player object is not assigned!");
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; 

        Vector2 shootDirection = (mousePosition - player.transform.position).normalized;

        if(Input.GetMouseButtonDown(0))
        {
            Shoot(shootDirection);
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, player.transform.position, Quaternion.identity);
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        bulletRigidbody.velocity = direction * bulletSpeed;

        // Zniszczenie pocisku po 3 sekundach
        Destroy(bullet, 3f);
    }

}
