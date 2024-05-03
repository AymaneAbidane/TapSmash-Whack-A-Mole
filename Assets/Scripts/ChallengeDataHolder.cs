using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeDataHolder : MonoBehaviour
{
    [SerializeField] LevelDifficultySO levelDifficultySO;

    public LevelDifficultySO GetLevelDifficultySO() {  return levelDifficultySO; }

    private void Awake() {
        DontDestroyOnLoad(this);
    }
}
