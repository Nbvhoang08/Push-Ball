using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        public List<FillNode> nodes = new List<FillNode>();
        public int numStep;
        public PlayerMove player;
        public bool hasWon = false;
        
        public int remainingSteps;
        private float winCheckDuration = 1f; // Thời gian cần duy trì trạng thái fill để win
        private bool isCheckingWin = false; // Flag để kiểm tra xem có đang trong quá trình check win không
        private Coroutine checkWinCoroutine;

        void Awake()
        {
            if (player == null)
            {
                player = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
            }
            remainingSteps = numStep;
        }
        public void Start()
        {
            UIManager.Instance.OpenUI<GamePlayCanvas>();
        }

        // Update is called once per frame
        void Update()
        {
            if (player.FirstMove)
            {
                if (numStep >= player.StepCount )
                {
                    CheckWinCondition();
                }

                CheckLoseCondition();
            }
           
            remainingSteps = Mathf.Max(0, CalculateRemainingSteps());
        
           
        }
        private void CheckWinCondition()
        {
            if (hasWon) return;

            bool allNodesFilled = true;
            foreach (FillNode node in nodes)
            {
                if (!node.isFilled)
                {
                    allNodesFilled = false;
                   
                    break;
                }
                
            }
           
            
            if (allNodesFilled && !isCheckingWin)
            {
                // Bắt đầu đếm thời gian khi tất cả nodes được fill
                if (checkWinCoroutine != null)
                {
                    StopCoroutine(checkWinCoroutine);
                }
                checkWinCoroutine = StartCoroutine(WinCheckCoroutine());
             
            }
            else if (!allNodesFilled && isCheckingWin)
            {
                // Nếu có node không còn filled trong quá trình check, hủy check
                if (checkWinCoroutine != null)
                {
                    StopCoroutine(checkWinCoroutine);
                    isCheckingWin = false;
                }
                
            }
        }

        private IEnumerator WinCheckCoroutine()
        {
            isCheckingWin = true;
            float elapsedTime = 0f;

            while (elapsedTime < winCheckDuration)
            {
                // Kiểm tra lại trạng thái của tất cả các node
                bool stillAllFilled = true;
                foreach (FillNode node in nodes)
                {
                    if (!node.isFilled)
                    {
                        stillAllFilled = false;
                        break;
                    }
                }

                // Nếu có node không còn filled, dừng coroutine
                if (!stillAllFilled)
                {
                    isCheckingWin = false;
                    yield break;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Nếu đã duy trì được trạng thái fill đủ thời gian
            hasWon = true;
            isCheckingWin = false;
            LevelManager.Instance.SaveGame();

            StartCoroutine(nextsence());
            // Có thể thêm hiệu ứng hoặc thông báo win ở đây
            //Debug.Log("Level Completed!");
        }

// Thêm hàm này để reset trạng thái khi cần
        public void ResetWinCheck()
        {
            if (checkWinCoroutine != null)
            {
                StopCoroutine(checkWinCoroutine);
            }
            isCheckingWin = false;
            hasWon = false;
        }


        private void CheckLoseCondition()
        {    
            
            if (remainingSteps == 0)
            {
                //Time.timeScale = 0;
                StartCoroutine(CheckLose());

            }
        }
        IEnumerator CheckLose()
        {
            yield return new WaitForSeconds(3f);
            if (hasWon)
            {
        
                CheckWinCondition();
            }
            else
            {
 
                StartCoroutine(lose());
            }
        }

        IEnumerator nextsence()
        {
            yield return new WaitForSeconds(1);
            UIManager.Instance.CloseUIDirectly<GamePlayCanvas>();
            SwitchToNextScene();
        }
        public void SwitchToNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(nextSceneIndex);
        }
        private int CalculateRemainingSteps()
        {
            int steps = numStep - player.StepCount;
            return Mathf.Max(0, steps);
        }

        IEnumerator lose()
        {
            yield return new WaitForSeconds(0.5f);
            if (!hasWon)
            {
                Time.timeScale = 0f;
                UIManager.Instance.OpenUI<LoseCanvas>();
            }
          
        }
    }
}