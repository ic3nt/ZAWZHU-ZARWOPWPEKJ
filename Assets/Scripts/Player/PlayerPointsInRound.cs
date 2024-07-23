using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPointsInRound : MonoBehaviour
{
    public HealthManager healthManager;
    public int points;

    public int currfloor;
    public TextMeshProUGUI textPoint;

    public TextMeshProUGUI textflr;

    private void Start()
    {
        currfloor = 0;
        points = 0;
    }

    void FixedUpdate()
    {
        textPoint.text = points.ToString();

        textflr.text = currfloor.ToString();
    }
    private void Awake()
    {
        if (healthManager.healthAmount <= 0)
        {
            PointsCounter.instance.AddScore(points);
            Debug.Log("AddScore");

        }
    }

}
