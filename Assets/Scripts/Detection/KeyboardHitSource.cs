using UnityEngine;
using UnityEngine.InputSystem;

namespace Vibranite.Detection
{
    /// <summary>
    /// Debug hit source: press a key to hit. Lets you test timing without the headset.
    /// The Game view must have focus for key presses to register.
    /// </summary>
    public class KeyboardHitSource : HitSource
    {
        [SerializeField] private Key hitKey = Key.Space;

        private void Update()
        {
            if (WasHitKeyPressed()) ReportHit();
        }

        private bool WasHitKeyPressed()
            => Keyboard.current != null && Keyboard.current[hitKey].wasPressedThisFrame;
    }
}
