using System.Collections.Generic;
using UnityEngine;

public class MoleManager : MonoBehaviour
{
    public static MoleManager Instance { get; private set; }

    private List<Mole> moles = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void RegisterMole(Mole mole)
    {
        moles.Add(mole);
    }

    public void UnregisterMole(Mole mole)
    {
        moles.Remove(mole);
    }

    public IReadOnlyList<Mole> GetMoles()
    {
        return moles.AsReadOnly();
    }
}
