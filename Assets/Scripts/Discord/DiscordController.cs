using Discord;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    [Header("Discord Application Settings")]
    [Space(10)]
    public long applicationID;

    [Header("Rich Presence Details")]
    [Space(10)]
    public string details = "";

    public string state = "";

    public string largeImage = "";

    public string largeText = "";


    private long time;

    private static bool instanceExists;
    public Discord.Discord discord;

    void Awake()
    {
        if (!instanceExists)
        {
            instanceExists = true;
        }
        else if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize Discord connection
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // Update the Discord Rich Presence status
        UpdateStatus();
    }

    void Update()
    {
        // Regularly update Discord Rich Presence status
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
        // Attempt to update the Discord status
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

     private void OnDestroy()
     {
         if (discord != null)
         {
             discord.Dispose();
             discord = null;
         }
     }
}
