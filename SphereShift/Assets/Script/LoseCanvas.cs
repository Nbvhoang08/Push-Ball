using UnityEngine.SceneManagement;
using UnityEngine;
namespace Script
{
    public class LoseCanvas : UICanvas
    {
        public void RetryBtn()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            UIManager.Instance.CloseUI<LoseCanvas>(0.1f);
        }

        public void HomeBtn()
        {
            Time.timeScale = 1;
            
            SceneManager.LoadScene("Home");
            UIManager.Instance.CloseAll();
            UIManager.Instance.OpenUI<HomeCanvas>();

        }
    }
}