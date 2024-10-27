using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Script
{
    public class Obstacle : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRange = 5f; // Khoảng cách phát hiện Player
        [SerializeField] private LayerMask playerLayer; // Layer của Player
        [SerializeField] private LayerMask nodeLayer; // Layer của các Node
        [SerializeField] private bool showDebugRays = true;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float precision = 0.01f;
        [SerializeField] private LayerMask obstacleLayer;
        private Vector3 targetPosition;
        private bool isMoving = false;
        public Transform StartPos;

        private void Start()
        {
            transform.position = StartPos.position;
        }

        private void Update()
        {
            if (!isMoving)
            {
                CheckPlayerAndMove();
            }
            else
            {
                MoveToTarget();
            }

            // Vẽ ray để debug
            if (showDebugRays)
            {
                DrawDebugRays();
            }
        }

        private void CheckPlayerAndMove()
        {
            RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, detectionRange, playerLayer);
            RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, playerLayer);
            RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, playerLayer);
            RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, playerLayer);

            if (upHit.collider != null)
            {
                HandlePlayerDetection(upHit, Vector2.down);
                
            }
            else if (rightHit.collider != null)
            {
                HandlePlayerDetection(rightHit, Vector2.left);
                
            }
            else if (downHit.collider != null)
            {
                HandlePlayerDetection(downHit, Vector2.up);
              
            }
            else if (leftHit.collider != null)
            {
                HandlePlayerDetection(leftHit, Vector2.right);
                

            }
        }

        private void HandlePlayerDetection(RaycastHit2D playerHit, Vector2 escapeDirection)
        {
            PlayerMove playerMoveScript = playerHit.collider.GetComponent<PlayerMove>();
   
            if (playerMoveScript != null )
            {
                if (!playerMoveScript.IsMoving() && playerMoveScript.canMove && playerMoveScript.isDone)
                {
                    FindAndMoveToNearestNode(escapeDirection);
                    
                }
            }
            

            
        }

        private  void FindAndMoveToNearestNode(Vector2 escapeDirection)
        {
            RaycastHit2D[] nodeHits = Physics2D.RaycastAll(transform.position, escapeDirection, detectionRange, nodeLayer);

            Transform nearestNode = null;
            float nearestDistance = float.MaxValue;

            foreach (RaycastHit2D hit in nodeHits)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < nearestDistance && hit.transform.position != transform.position)
                {
                    nearestNode = hit.transform;
                    nearestDistance = distance;
                }
            }

            if (nearestNode != null)
            {
                // Kiểm tra obstacle tại vị trí nearestNode
                Collider2D obstacle = Physics2D.OverlapCircle(nearestNode.position, 0.1f, obstacleLayer);

                if (obstacle == null)
                {
                    // Không có obstacle, di chuyển ngay
                    InitializeMovement(nearestNode.position);
                    if (showDebugRays)
                    {
                        Debug.Log($"Moving to node at {nearestNode.position} to escape from direction {escapeDirection}");
                    }
                }
                else
                {
                    // Có obstacle, đợi 0.5s và kiểm tra lại
                    StartCoroutine(CheckObstacleAfterDelay(nearestNode, escapeDirection));
                }
            }
            else
            {
                if (showDebugRays)
                {
                    Debug.Log($"No escape node found in direction {escapeDirection}");
                }
            }
        }

        private IEnumerator CheckObstacleAfterDelay(Transform node, Vector2 escapeDirection)
        {
            yield return new WaitForSeconds(0.01f);

            // Kiểm tra lại sau 0.5s
            Collider2D obstacle = Physics2D.OverlapCircle(node.position, 0.1f, obstacleLayer);

            if (obstacle == null)
            {
                // Không còn obstacle, thực hiện di chuyển
                InitializeMovement(node.position);
                if (showDebugRays)
                {
                    Debug.Log($"Moving to node at {node.position} after obstacle cleared");
                }
            }
            else
            {
                if (showDebugRays)
                {
                    Debug.Log($"Cannot move to node at {node.position} - obstacle still present");
                }
            }
        }

        private void InitializeMovement(Vector3 newTarget)
        {
            targetPosition = new Vector3(newTarget.x, newTarget.y, transform.position.z);
            isMoving = true;
            SoundManager.Instance.PlayMoveSound();
        }

        private void MoveToTarget()
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
            
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distanceToTarget <= precision)
            {
                FinalizeMovement();
            }

             
        }

      
        private void FinalizeMovement()
        {
            transform.position = targetPosition;
            isMoving = false;
        }

        private void DrawDebugRays()
        {
            Debug.DrawRay(transform.position, Vector2.up * detectionRange, Color.red);
            Debug.DrawRay(transform.position, Vector2.right * detectionRange, Color.red);
            Debug.DrawRay(transform.position, Vector2.down * detectionRange, Color.red);
            Debug.DrawRay(transform.position, Vector2.left * detectionRange, Color.red);

            if (isMoving)
            {
                Debug.DrawLine(transform.position, targetPosition, Color.green);
            }
        }

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
    }
}