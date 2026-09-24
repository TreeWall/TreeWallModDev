using System;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using static TreeWallMod.Modules.Debugging;

namespace TreeWallMod.Modules
{
    public class DebuggingWindow : MonoBehaviour
    {
        public DebuggingWindow(IntPtr ptr) : base(ptr) { }

        private bool _showWindow = true;
        private Rect _windowRect = new Rect(20, 20, 300, 200);

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _showWindow = !_showWindow;
            }
        }

        public void OnGUI()
        {
            if (!_showWindow) return;

            // Draw an immediate mode GUI window
            _windowRect = GUI.Window(
                0,
                _windowRect,
                (GUI.WindowFunction)DrawWindowContents,
                "Mod Testing Debugger"
            );
        }

        public void DrawWindowContents(int windowID)
        {
            GUILayout.Label("Mod Status: Active");

            if (GUILayout.Button("Camera"))
            {
                Message(DebugCamera());
            }
            if (GUILayout.Button("KIllButton"))
            {
                Message(DebugKillButton());
            }
            if (GUILayout.Button("SpawnHeadless"))
            {
                if (TutorialManager.InstanceExists)
                {
                    HeadlessPlayer.Spawn(PlayerControl.LocalPlayer, idleAnim: Assets.Assets.HeadlessIdleAnim.LoadAsset(), walkAnim: Assets.Assets.HeadlessWalkAnim.LoadAsset());
                }
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}
