using UnityEngine.UI;
using UnityEngine;
namespace Gelo.UI.Model
{
    public class NotifyModel : Model
    {
        [SerializeField]
        private Text m_headerText;
        [SerializeField]
        private Text m_bodyText;

        public void UpdateNotification(string header, string body)
        {
            m_headerText.text = header;
            m_bodyText.text = body;
        }
        public override void Clear()
        {
            m_headerText.text = string.Empty;
            m_bodyText.text = string.Empty;
        }
    }
}
