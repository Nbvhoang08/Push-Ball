using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class PlayerMove : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float precision = 0.01f;
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private LayerMask obstacleLayer;

        [Header("Debug Settings")]
        [SerializeField] private bool showDebugInfo = false;

        private Vector3 targetPosition;
        private Vector3 startPosition;
        [SerializeField] private bool isMoving = false;
        private float journeyLength;
        private float startTime;
        public int StepCount;
        public Transform StarPos;
        public bool canMove = false;
        public bool isDone = true;
        public GameObject OutLine;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 1f); // Màu sáng mặc định là trắng
        public bool FirstMove;
        
        void Start()
        {
            transform.position = StarPos.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            OutLine.SetActive(false);
            FirstMove  = false;

    }

    private void Update()
        {
            HandleInput();
            if (isMoving)
            {
                PreciseMove();
                
            }

         
            
        }
        private void HandleInput()
        {
            float maxDistance = 200.0f;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Lấy tất cả các hit objects tại điểm click
                RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, maxDistance);
                bool playerClicked = false;
                Vector2 nodePosition = Vector2.zero;
                bool validNodeClicked = false;

                // Kiểm tra từng object được click
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null)
                    {
                        // Kiểm tra nếu hit vào player
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            playerClicked = true;
                            HighlightPlayer(true);
                            SoundManager.Instance.PlayClickSound();
                        }
                        // Kiểm tra nếu hit vào node và node không cùng vị trí với player
                        else if (((1 << hit.collider.gameObject.layer) & nodeLayer) != 0 &&
                                 (Vector2)hit.collider.transform.position != (Vector2)this.transform.position)
                        {
                            nodePosition = hit.collider.transform.position;
                            validNodeClicked = true;
                            SoundManager.Instance.PlayClickSound();
                        }
                    }
                }

                // Xử lý logic sau khi đã kiểm tra tất cả các hit objects
                if (playerClicked)
                {
                    canMove = true;
                }
                else if (canMove && validNodeClicked)
                {
                    Collider2D obstacleHit = Physics2D.OverlapPoint(nodePosition, obstacleLayer);

                    bool hasObstacle = obstacleHit != null;

                    // Chỉ di chuyển nếu không có obstacle
                    if (!hasObstacle)
                    {
                        InitializeMovement(nodePosition);
                        StepCount += 1;
                        StartCoroutine(ResetCanMove());
                    }
                }
            }
        }

        // Hàm xử lý highlight player
        private void HighlightPlayer(bool highlight)
        {
            if (OutLine!= null)
            {
                OutLine.SetActive(highlight);
               
            }
        }

// Thêm hàm này để reset màu player khi script bị disable hoặc game object bị destroy
        private void OnDisable()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
        private void InitializeMovement(Vector3 newTarget)
        {
            startPosition = transform.position;
            targetPosition = new Vector3(newTarget.x, newTarget.y, transform.position.z);
            journeyLength = Vector3.Distance(startPosition, targetPosition);
            startTime = Time.time;
            isMoving = true;
            isDone = false;
            FirstMove = true;
            //SoundManager.Instance.PlayMoveSound();

        }

        private void PreciseMove()
        {
            Vector3 newPosition = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            transform.position = newPosition;

            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget <= precision)
            {
                

         
                FinalizeMovement();
                StartCoroutine(ResetCanMove());
            }

            UpdateRotation();

            
        }

        private void UpdateRotation()
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        private void FinalizeMovement()
        {
            transform.position = targetPosition;
            isMoving = false;
            isDone = true;
           
        }

        private IEnumerator ResetCanMove()
        {
            yield return new WaitForSeconds(1f); // Thời gian chờ trước khi reset canMove
            canMove = false;
            HighlightPlayer(false);
            yield return new WaitForSeconds(0.3f);
            isDone = false;
        }

        // Public methods để kiểm soát movement từ bên ngoài
        public void StopMovement()
        {
            isMoving = false;
        }

        public bool IsMoving()
        {
            return isMoving;
        }

        public Vector3 GetTargetPosition()
        {
            return targetPosition;
        }

        public float GetRemainingDistance()
        {
            if (!isMoving) return 0f;
            return Vector3.Distance(transform.position, targetPosition);
        }

        private bool IsValidDestination(Vector3 position)
        {
            return !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsInfinity(position.x) &&
                   !float.IsInfinity(position.y);
        }

        private void OnDrawGizmos()
        {
            if (showDebugInfo && isMoving)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(startPosition, targetPosition);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(targetPosition, precision);
            }
        }
    }
}