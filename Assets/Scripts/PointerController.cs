using UnityEngine;
using UnityEngine.InputSystem;

public class PointerController : MonoBehaviour
{
    public bool canMove;

    private Vector2 movement;

    public float accel;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float decelSpeed;

    public void MovePointer(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (movement.x > 0 && accel < 1)
            accel += Time.deltaTime;
        else if (movement.x < 0 && accel > -1)
            accel -= Time.deltaTime;
        else if (accel < 0)
            accel += Time.deltaTime * decelSpeed;
        else if (accel > 0)
            accel -= Time.deltaTime * decelSpeed;

        if (accel < 0.05f && accel > 0 && movement.x == 0 || accel > -0.05f && accel < 0 && movement.x == 0)
            accel = 0;
            Debug.Log("Accel 0");
        
        if (canMove)
            transform.Rotate(0f, 0f, accel * moveSpeed);

        if (movement != Vector2.zero)
            Debug.Log("movement found");
    }
}
