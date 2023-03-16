using UnityEngine;
namespace Gelo.UI.View
{
    public abstract class View : MonoBehaviour
    {
        public abstract void Activate();
        public abstract void Deactivate();
    }
}
