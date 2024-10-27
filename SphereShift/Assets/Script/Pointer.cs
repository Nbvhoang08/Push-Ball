using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public Transform position1;
    public Transform position2;
    public Vector3 offset;
    public float speed = 5.0f; // Tốc độ di chuyển

    private bool isAtPosition1 = true;
    private bool isMoving = false;
    private Vector3 targetPosition;


    void Start()
    {
        // Đặt con trỏ tại vị trí Position1 với offset
        transform.position = position1.position + offset;
    }

    void Update()
    {
        if (isAtPosition1 && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(mousePos, position1.position) < 0.5f)
            {
                // Thiết lập vị trí đích và bắt đầu di chuyển
                targetPosition = position2.position + offset;
                isMoving = true;
                isAtPosition1 = false;
            }
        }
        else if (!isAtPosition1 && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(mousePos, position2.position) < 0.5f)
            {
                // Tắt object này
                gameObject.SetActive(false);
            }
        }

        // Di chuyển mượt mà đến vị trí đích
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}
