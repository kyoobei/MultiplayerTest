using Fusion;
using UnityEngine;

namespace Gelo.Game
{
    public enum InputButtons
    {
        Forward = 0,
        Backward = 1,
        Left = 2,
        Right = 3,
    }
    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons Buttons;

        public bool IsDown(int button)
        {
            return Buttons.IsSet(button);
        }
    }
}