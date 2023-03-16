using UnityEngine;
using Gelo.UI;
namespace Gelo.Game
{
    public class MainGameManager : MonoBehaviour
    {
        [SerializeField]
        private MainMenuManager m_mainMenuManager;
        [SerializeField]
        private GameObject m_gameUI;

        private void OnEnable()
        {
            m_mainMenuManager.OnStartGame += RecievedStartGame;
        }
        private void OnDisable()
        {
            m_mainMenuManager.OnStartGame -= RecievedStartGame;
        }
        private void RecievedStartGame()
        {
            m_gameUI.SetActive(true);
        }
    }
}
