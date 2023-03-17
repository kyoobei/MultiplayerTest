using UnityEngine;
using Gelo.UI;
using Gelo.UI.Controller;

namespace Gelo.Game
{
    public class MainGameManager : MonoBehaviour
    {
        [SerializeField]
        private MainMenuManager m_mainMenuManager;
        [SerializeField]
        private NetworkManager m_networkManager;
        [SerializeField]
        private DialogController m_dialogController;

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
            m_networkManager.StartGame();
            // m_gameUI.SetActive(true);
        }
    }
}
