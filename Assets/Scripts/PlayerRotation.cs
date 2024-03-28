using System.Collections;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    void FixedUpdate()
    {
        FaceMouse();
    }
 
    void FaceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = mousePosition - transform.position;

        transform.up = direction;
    }
}
