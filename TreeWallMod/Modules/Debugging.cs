using MiraAPI.Hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Buttons;
using UnityEngine;
using static Rewired.ComponentControls.TouchButton;

namespace TreeWallMod.Modules
{
    public static class Debugging
    {
        public static string DebugCamera()
        {
            var cam = Camera.main;
            var z = -89f;
            var bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.WorldToScreenPoint(new Vector3(0, 0, z)).z));
            var topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.WorldToScreenPoint(new Vector3(0, 0, z)).z));

            return ($"Orthographic: {cam.orthographic}, Size: {cam.orthographicSize}, Aspect: {cam.aspect}, " +
                $"Position: {cam.transform.position}, Bottom Left: {bottomLeft}, Top Right: {topRight}");
        }

        public static string DebugKillButton()
        {
            StringBuilder sb = new();

            if (CustomButtonManager.Buttons.Count == 0 || CustomButtonManager.Buttons == null)
            {
                return "No Buttons";
            }

            foreach (var button in CustomButtonManager.Buttons)
            {
                if (button != null && button.Button != null && button is IKillButton && button.Button.isActiveAndEnabled)
                {
                    sb.Append($"{button.Name} is Kill type and present\n");
                }
            }

            return sb.ToString();
        }
    }
}
