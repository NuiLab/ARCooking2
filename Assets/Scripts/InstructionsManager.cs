using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField] GameObject[] instuctionPos;
    [SerializeField] string[] instuctionText;
    [SerializeField] GameObject textGO;

    int instructionStage = 0;
    IMixedRealitySceneSystem sceneSystem;
    GameObject globalRecords_GO;

    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        transform.GetChild(0).GetComponent<DirectionalIndicator>().DirectionalTarget = instuctionPos[instructionStage].transform;
    }


    public void NextInstruction()
    {
        if (instructionStage != instuctionPos.Length - 1)
        {
            instructionStage++;
            transform.GetChild(0).GetComponent<DirectionalIndicator>().DirectionalTarget = instuctionPos[instructionStage].transform;
            transform.GetChild(1).transform.position = instuctionPos[instructionStage].transform.position;
            transform.GetChild(1).transform.rotation = instuctionPos[instructionStage].transform.rotation;
            textGO.gameObject.GetComponent<TextMeshPro>().text = instuctionText[instructionStage];

        }
        else
        {
            globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().SetShowNotification(true);
            var task = LoadNextLevel();
        }
    }

    public async Task LoadNextLevel()
    {
        await sceneSystem.UnloadContent("Instructions Scene");
        await sceneSystem.LoadContent("NoO Scene");
    }
}
