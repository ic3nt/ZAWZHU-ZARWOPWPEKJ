using Discord;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    public long applicationID;
    [Space]
    public string details = "";
    public string state = "";
    [Space]
    public string largeImage = "";
    public string largeText = "";

    private long time;

    private static bool instanceExists;
    public Discord.Discord discord;

    // а тут значит DD работает с дискордом

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
        // при старте начинаем отображать статус в дс

        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        UpdateStatus();
    }

    void Update()
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
        // все че надо отображаем в статусе дс, обновляя его, если DD не может подключится к сервисам дискорда то выводим ошибку в консоль

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
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord!");
            });
        }
        catch
        {
            Destroy(gameObject);
        }
    }
  //  private void OnDestroy()
 //   {
  //      if (discord != null)
  //      {
  //          discord.Dispose();
  //          discord = null;
   //      }
  //  }
}