using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

namespace Script
{
    public class GamePlayCanvas : UICanvas
    { 
        [SerializeField] private Text _levelText;
        [SerializeField] private Transform stepCountContainer;
        [SerializeField] private Transform SpawnPos;
        [SerializeField] private GameObject stepCountPrefab;
        [SerializeField] private Sprite fullSprite;
        [SerializeField] private Sprite emptySprite;
        [SerializeField] private float spacing = 20f; // Khoảng cách giữa các UI
        [SerializeField] private GameObject WinMess;
        private GameManager _gameManager;
        private List<Image> stepCountImages = new List<Image>();

        private void Awake()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }
        }

        private void OnEnable()
        {
            spawnStepCountManual();
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log(scene.name);
            spawnStepCountManual();
        }


        // Cách 1: Sử dụng HorizontalLayoutGroup
        public void spawnStepCount()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }

            // Xóa các UI Count hiện có
            foreach (Transform child in stepCountContainer)
            {
                Destroy(child.gameObject);
            }
            stepCountImages.Clear();

            // Đảm bảo có HorizontalLayoutGroup
            HorizontalLayoutGroup layoutGroup = stepCountContainer.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = stepCountContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            // Thiết lập các thuộc tính của HorizontalLayoutGroup
            layoutGroup.spacing = spacing;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            // Tạo các UI Count mới
            for (int i = 0; i < _gameManager.numStep; i++)
            {
                GameObject stepCount = Instantiate(stepCountPrefab, stepCountContainer);
                Image stepCountImage = stepCount.GetComponent<Image>();
                stepCountImage.sprite = fullSprite;
                stepCountImages.Add(stepCountImage);
            }

            // Cập nhật layout
            Canvas.ForceUpdateCanvases();
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        // Cách 2: Thiết lập vị trí thủ công
        public void spawnStepCountManual()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }

            // Xóa các UI Count hiện có
            foreach (Transform child in stepCountContainer)
            {
                Destroy(child.gameObject);
            }
            stepCountImages.Clear();

            // Lấy kích thước của prefab để tính toán vị trí
            RectTransform prefabRect = stepCountPrefab.GetComponent<RectTransform>();
            float elementWidth = prefabRect.sizeDelta.x;

            // Tạo các UI Count mới
            for (int i = 0; i < _gameManager.numStep; i++)
            {
                GameObject stepCount = Instantiate(stepCountPrefab, stepCountContainer);
                RectTransform rectTransform = stepCount.GetComponent<RectTransform>();
                
                // Tính toán vị trí X cho mỗi element
                float xPosition = i * (elementWidth + spacing);
                
                // Thiết lập vị trí
                rectTransform.anchoredPosition = new Vector2(xPosition, 0);
                
                // Thêm vào danh sách
                Image stepCountImage = stepCount.GetComponent<Image>();
                stepCountImage.sprite = fullSprite;
                stepCountImages.Add(stepCountImage);
            }

            // Cập nhật kích thước của container nếu cần
            RectTransform containerRect = stepCountContainer.GetComponent<RectTransform>();
            float totalWidth = _gameManager.numStep * (elementWidth + spacing) - spacing;
            containerRect.sizeDelta = new Vector2(totalWidth, containerRect.sizeDelta.y);
        }

        public void PauseBtn()
        {
            UIManager.Instance.OpenUI<PauseCanvas>();
            SoundManager.Instance.PlayPopUpSound();
        }

        private void Update()
        {
            UpdateLevelText();
            UpdateStepCount();
            if (_gameManager.hasWon)
            {
                WinMess.SetActive(true);
                
            }
            else
            {
                WinMess.SetActive(false);
            }
        }

        private void UpdateLevelText()
        {
            if (_levelText != null)
            {
                _levelText.text = SceneManager.GetActiveScene().name;
            }
        }

        public void UpdateStepCount()
        {
            if (_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }

            int remainingSteps = _gameManager.remainingSteps;

            for (int i = 0; i < stepCountImages.Count; i++)
            {
                if (i < remainingSteps)
                {
                    stepCountImages[i].sprite = fullSprite;
                }
                else
                {
                    stepCountImages[i].sprite = emptySprite;
                }
            }
        }
    }
}