using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletRange = 10f;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] ParticleSystem muzzleFlash;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.Initialize(bulletSpeed, bulletRange);

        muzzleFlash.Play();
    }

}
