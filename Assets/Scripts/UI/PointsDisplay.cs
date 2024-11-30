using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsDisplay : MonoBehaviour
{
    public TextMeshProUGUI pointsAmount;
    private void Start()
    {
        int score = PointsCounter.instance.GetScore();
        pointsAmount.text = score.ToString();
    }
}
