using Discord;
using UnityEngine;
using RKS.DD.Core;

namespace RKS.DD.Core.Managers
{
    public class DiscordController : RKSBehaviour
    {
        [Header("Discord Application Settings")]
        [Space(10)]
        public long applicationID;

        [Header("Rich Presence Details")]
        [Space(10)]
        public string details;
        public string state;
        public string largeImage;
        public string largeText;

        private long time;

        private static bool instanceExists;
        public Discord.Discord discord;

        protected override void OnReady()
        {
            discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

            time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

            UpdateStatus();
        }

        protected override void Update()
        {
            UpdateStatus();

            try
            {
                discord.RunCallbacks();
            }
            catch
            {
                Destroy(gameObject);
            }
        }

        void UpdateStatus()
        {
            try
            {
                var activityManager = discord.GetActivityManager();
                var activity = new Discord.Activity
                {
                    Details = details,
                    State = state,
                    Assets =
                {
                    LargeImage = largeImage,
                    LargeText = largeText
                },
                    Timestamps =
                {
                    Start = time
                }
                };

                activityManager.UpdateActivity(activity, (res) =>
                {
                    if (res != Discord.Result.Ok)
                        Debug.LogWarning("Failed connecting to Discord!");
                });
            }
            catch
            {
                Destroy(gameObject);
            }
        }

        private void OnDispose()
        {
            if (discord != null)
            {
                discord.Dispose();
                discord = null;
            }
        }
    }
}