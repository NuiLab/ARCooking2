using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class StartExperiment : MonoBehaviour
{
    [SerializeField] GameObject textField;
    [SerializeField] GameObject placeholder;

    IMixedRealitySceneSystem sceneSystem;
    GameObject persistentGO;

    // Start is called before the first frame update
    void Start()
    {
        persistentGO = GameObject.FindGameObjectsWithTag("PersistentGO")[0];
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
    }

    public void ButtonPress()
    {
        int pNum = 0;
        if (int.TryParse(textField.GetComponentInChildren<TMP_InputField>().text, out pNum))
        {
            persistentGO.GetComponent<PersistentGOManager>().SetParticipantNumber(pNum);
            persistentGO.GetComponent<PersistentGOManager>().SetSceneOrder();
            var task = LoadNextLevel();
            Destroy(textField);
            Destroy(transform.gameObject);
        }
        else
        {
            textField.GetComponentInChildren<TMP_InputField>().text = "";
            placeholder.GetComponent<TextMeshProUGUI>().text = "Enter correct number...";
        }
    }

    public async Task LoadNextLevel()
    {
        await sceneSystem.LoadContent("Instructions Scene");
    }
}
