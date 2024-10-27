namespace Script
{
    public class HomeCanvas : UICanvas
    {
        public void PlayBtn()
        {
            UIManager.Instance.CloseAll();
            UIManager.Instance.OpenUI<LevelCanvas>();
            SoundManager.Instance.PlayPopUpSound();
        }
    }
}