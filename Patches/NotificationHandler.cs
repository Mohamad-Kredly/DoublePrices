using HarmonyLib;

namespace DoublePrices.Patches
{

    [HarmonyPatch(typeof(GameCanvas))]
    internal class NotificationHandler
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void NotificationHandler_Postfix(GameCanvas __instance, ref bool ___inCooldown)
        {
            if (DoublePricesPlugin.notify)
            {
                ___inCooldown = false;
                DoublePricesPlugin.notify = false;
                string Notification = "`";
                switch (DoublePricesPlugin.notificationType) 
                {
                    case "priceToggle":
                        Notification += "Double Price: " + (DoublePricesPlugin.doublePrice ? "ON" : "OFF");
                        break;
                    case "roundDownSwitch":
                        Notification += "Roudning to nearest " + (DoublePricesPlugin.NearestTen.Value ? "ten" : "five") + (!DoublePricesPlugin.roundDown.Value ? "\r\n(Currently disabled)" : "");
                        break;
                    case "roundDownToggle":
                        Notification += "Rounding has been " + (DoublePricesPlugin.roundDown.Value ? "enabled" : "disabled");
                        break;
                    default:
                        break;
                }
                __instance.CreateCanvasNotification(Notification);
            }
        }
    }
}