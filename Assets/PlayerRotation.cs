using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    // Reference to the proxy object for rotation
    public Transform rotationProxy;

    // Update is called once per frame
    void Update()
    {
        RotatePlayerTowardsMouse();
    }

    void RotatePlayerTowardsMouse()
    {
        // Pobierz pozycję kursora na ekranie
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Oblicz kierunek, w którym powinien być obrócony gracz
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );

        // Oblicz kąt obrotu w radianach
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 0f;

        // Ustaw obrotu proxy gracza natychmiastowo w kierunku kursora
        rotationProxy.rotation = Quaternion.Euler(0, 0, angle);
    }
}
