using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private GameObject impactParticlePrefab; // Prefabrykat efektu cząsteczkowego uderzenia

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzamy, czy doszło do kolizji z jakimś obiektem
        if (!collision.gameObject.CompareTag("Player")) // Sprawdzamy, czy obiekt, z którym kolidujemy, nie jest graczem
        {
            Destroy(gameObject); // Niszczymy pocisk

            // Tworzymy efekt cząsteczkowy uderzenia na pozycji kolizji
            if (impactParticlePrefab != null)
            {
                GameObject impactParticle = Instantiate(impactParticlePrefab, collision.contacts[0].point, Quaternion.identity);
                Destroy(impactParticle, 0.05f);
            }
        }
    }
}
