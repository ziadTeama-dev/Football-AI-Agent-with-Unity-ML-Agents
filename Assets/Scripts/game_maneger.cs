using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class game_maneger : MonoBehaviour
{
    [SerializeField] private Goal_Is_hit goal_Is_Hit;
    [SerializeField] private TMP_Text text;

    void Update()
    {
        text.text=$"{goal_Is_Hit.total_blue_goals}:{goal_Is_Hit.total_red_goals}";
    }
}
