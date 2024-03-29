using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject impactParticlePrefab; // Prefabrykat efektu cząsteczkowego uderzenia
    [SerializeField] GameObject graphics; // elementy wizualne pocisku
    [SerializeField] LayerMask collisionMask;
    private Vector2 velocity;
    private float range;
    [SerializeField] float invisibleRange; // Pocisk na początku jest niewidzialny aby ukryć to że wylatuje z głowy gracza
    private float distanceTravelled = 0f;

    public void Initialize(float speed, float range)
    {
        this.range = range;
        velocity = transform.up * speed; // Używamy transform.up jako wektora kierunku
        graphics.SetActive(false);
    }

    private void Update()
    {
        // Sprawdzamy, czy doszło do kolizji z jakimś obiektem
        float speed = velocity.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, Time.deltaTime * speed, collisionMask);
        if (hit.collider)
        {
            // Tworzymy efekt cząsteczkowy uderzenia na pozycji kolizji
            if (impactParticlePrefab)
            {
                Instantiate(impactParticlePrefab, hit.point, Quaternion.identity);
            }

            Destroy(gameObject); // Niszczymy pocisk
        }
        else
        {
            // Jeśli nie doszło do kolizji, przesuwamy pocisk do przodu
            transform.position += (Vector3)velocity * Time.deltaTime;
        }

        distanceTravelled += speed * Time.deltaTime;
        // Aktywujemy elementy wizualne dopiero po chwili
        if (distanceTravelled > invisibleRange)
        {
            graphics.SetActive(true);
        }
        // Niszczymy pocisk jeśli osiągnął maksymalny zasięg
        if (distanceTravelled > range)
        {
            Destroy(gameObject);
        }
    }
}
