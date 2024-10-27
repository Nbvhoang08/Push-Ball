using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Script
{
    public class PauseCanvas : UICanvas
    {
        public void Resume()
        {
            UIManager.Instance.CloseUI<PauseCanvas>(0.2f);
            
        }

        public void HomeBtn()
        {
            UIManager.Instance.CloseAll();
            Time.timeScale = 1; 
            SceneManager.LoadScene("Home");
            
            UIManager.Instance.OpenUI<HomeCanvas>();
            
        }
    }
}