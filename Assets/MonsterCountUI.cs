using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCountUI : MonoBehaviour
{
    Text monsterCountText;
    void Start()
    {
        monsterCountText = transform.Find("Text").GetComponent<Text>();
    }
    int monsterCount;
    void Update()
    {
        monsterCount = NewMonster.totalMonster.Count;
        monsterCountText.text = $"몬스터 {monsterCount} 마리";
    }
}
