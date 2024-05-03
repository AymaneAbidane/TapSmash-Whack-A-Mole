using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeDataHolder : MonoBehaviour {
    public static ChallengeDataHolder Instance { get; private set; }
    [SerializeField] LevelDifficultySO levelDifficultySO;

    //public static ChallengeDataHolder Instance {
    //    get {
    //        if (instance == null) {
    //            instance = FindObjectOfType<ChallengeDataHolder>();
    //            if (instance == null) {
    //                GameObject obj = new GameObject();
    //                obj.name = typeof(ChallengeDataHolder).Name;
    //                instance = obj.AddComponent<ChallengeDataHolder>();
    //            }
    //        }
    //        return instance;
    //    }
    //}

    public LevelDifficultySO GetLevelDifficultySO() { return levelDifficultySO; }

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
