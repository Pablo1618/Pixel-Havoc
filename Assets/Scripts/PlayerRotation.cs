using System.Collections;
using UnityEngine;

public class FaceMouse : MonoBehaviour
{
    void FixedUpdate()
    {
        faceMouse();        
    }
 
    void faceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
 
        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
        );
 
        direction = Quaternion.Euler(0, 0, 90) * direction; // +90 stopni
 
        transform.up = direction;
    }
}
