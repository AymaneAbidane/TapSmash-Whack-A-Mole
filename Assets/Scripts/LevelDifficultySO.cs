using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelDifficultySO : ScriptableObject
{

    [SerializeField] private float timeBetweenMoleSpawns = .5f;
    [SerializeField] private int levelDifficulty = 10;

    public float GetTimeBetweenMoleSpawns() {
        return timeBetweenMoleSpawns;
    }
    public int GetLevelDifficulty() {
        return levelDifficulty;
    }
}
