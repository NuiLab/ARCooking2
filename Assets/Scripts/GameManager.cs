using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState state;

    public static event Action<GameState> OnGameStateChanged;

    IMixedRealitySceneSystem sceneSystem;
    GameObject persistentGO;
    string currSceneName;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        persistentGO = GameObject.FindGameObjectsWithTag("PersistentGO")[0];
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        UpdateGameState(GameState.Setup);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Setup:
                HandleSetup();
                break;
            case GameState.Welcome:
                HandleWelcome();
                break;
            case GameState.Scene:
                HandleScene();
                break;
            case GameState.End:
                HandleEnd();
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleSetup()
    {
        
    }

    private void HandleWelcome()
    {
        
    }

    private void HandleScene()
    {
        // persistentGO.GetComponent<PersistentGOManager>().SetSceneNamesAndLoad("NoO_WA Scene");
        persistentGO.GetComponent<PersistentGOManager>().SetSceneNamesAndLoad(persistentGO.GetComponent<PersistentGOManager>().GetNextScene());
        /*
        if (persistentGO.GetComponent<PersistentGOManager>().GetSceneIndex() < 6)
        {
            persistentGO.GetComponent<PersistentGOManager>().SetSceneNamesAndLoad(persistentGO.GetComponent<PersistentGOManager>().GetNextScene());

        }
        else
        {
            UpdateGameState(GameState.End);
        }
        */
    }

    private void HandleEnd()
    {
        
    }

    public void SetSceneName(string sceneName)
    {
        currSceneName = sceneName;
    }

    public string GetSceneName()
    {
        return currSceneName;
    }
}


public enum GameState
{
    Setup,
    Welcome,
    Scene,
    End
}

public enum Conditions
{
    Tutorial,
    NoO_WS,
    NoO_WOS,
    NoO_WA,
    NoD_WS,
    NoD_WOS,
    NoV_WS,
    NoV_WOS,
    Control_WS,
    Control_WOS
}
