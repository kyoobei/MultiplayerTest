using Gelo.UI.Model;
using UnityEngine;
namespace Gelo.UI.View
{
    public class PleaseWaitView : View
    {
        [SerializeField]
        private PleaseWaitModel m_pleaseWaitModel;

        public override void Activate() => m_pleaseWaitModel.gameObject.SetActive(true);
        public override void Deactivate() => m_pleaseWaitModel.gameObject.SetActive(false);
    }
}
