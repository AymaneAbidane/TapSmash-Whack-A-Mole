using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeDataHolder : MonoBehaviour {
    private static ChallengeDataHolder instance;
    [SerializeField] LevelDifficultySO levelDifficultySO;

    public static ChallengeDataHolder Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<ChallengeDataHolder>();
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = typeof(ChallengeDataHolder).Name;
                    instance = obj.AddComponent<ChallengeDataHolder>();
                }
            }
            return instance;
        }
    }

    public LevelDifficultySO GetLevelDifficultySO() { return levelDifficultySO; }

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            if (instance != this) {
                Destroy(gameObject);
            }
        }
    }
}
