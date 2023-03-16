using System;
using UnityEngine;
using Gelo.UI.View;

namespace Gelo.UI.Controller
{
    public abstract class Controller : MonoBehaviour
    {
        public Action<string, string> OnGeneralPopup;
        public Action OnWaitStart;
        public Action OnWaitEnd;
    }
}
