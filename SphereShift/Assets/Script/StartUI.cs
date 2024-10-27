using UnityEngine;

namespace Script
{
    public class StartUI : MonoBehaviour
    {
        void Start()
        {
            UIManager.Instance.OpenUI<HomeCanvas>();
        }
    }
}