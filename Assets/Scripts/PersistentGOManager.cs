using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class PersistentGOManager : MonoBehaviour
{
    public static PersistentGOManager instance;

    #region Consts to modify
    private const int FlushAfter = 40;
    #endregion

    [SerializeField] bool showNotification = false;
    [SerializeField] GameObject StudyBillboard;

    Vector3 position = new Vector3(1000, 1000, 1000);
    GameObject currGlobalRecordsGO;
    IMixedRealitySceneSystem sceneSystem;
    string unloadSceneName;
    bool notificationSound = false;
    bool sceneChanged = false;
    List<string> sceneNames;
    int sceneIndex = 0;
    string typedNotificationNumberTracker = "";

    int participantNumber = 0;
    string filePath;
    StreamWriter writer;
    float time_s = 0;
    List<string> independentCSVData = new List<string>();
    private StringBuilder csvData;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneNames = new List<string>() { "NoO_WS Scene", "NoO_WA Scene", "NoV_WOS Scene", "Control_WOS Scene" };      // Change this for different levels
        /*
        var rnd = new System.Random();
        sceneNames = sceneNames.OrderBy(item => rnd.Next()).ToList();
        */
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        filePath = Application.persistentDataPath + "/Records";
        Debug.Log(filePath);
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
        DontDestroyOnLoad(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        time_s += Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0))
            SetSceneNamesAndLoad("Instructions Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
            SetSceneNamesAndLoad("NoO_WS Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
            SetSceneNamesAndLoad("NoO_WA Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
            SetSceneNamesAndLoad("NoV_WOS Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
            SetSceneNamesAndLoad("Control_WOS Scene");
        /*
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
            SetSceneNamesAndLoad("NoO_WOS Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
            SetSceneNamesAndLoad("NoV_WS Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6))
            SetSceneNamesAndLoad("NoV_WOS Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha7))
            SetSceneNamesAndLoad("Control_WS Scene");
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha8))
            SetSceneNamesAndLoad("Control_WOS Scene");
        */

        if (Input.GetKeyDown(KeyCode.N))
        {
            writer.Close();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.I))
        {
            ShowInstructions(0);
            StudyInstructionsManager.instance.SetInstructionsNumer(1);
            // StudyInstructionsManager.instance.ResetInstructionNumber();
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                typedNotificationNumberTracker += 0;
                Debug.Log("0: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                typedNotificationNumberTracker += 1;
                Debug.Log("1: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                typedNotificationNumberTracker += 2;
                Debug.Log("2: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                typedNotificationNumberTracker += 3;
                Debug.Log("3: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                typedNotificationNumberTracker += 4;
                Debug.Log("4: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                typedNotificationNumberTracker += 5;
                Debug.Log("5: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                typedNotificationNumberTracker += 6;
                Debug.Log("6: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                typedNotificationNumberTracker += 7;
                Debug.Log("7: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                typedNotificationNumberTracker += 8;
                Debug.Log("8: " + typedNotificationNumberTracker);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                typedNotificationNumberTracker += 9;
                Debug.Log("9: " + typedNotificationNumberTracker);
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            AddData("Notification Number", "Called:" + typedNotificationNumberTracker, 2);
            typedNotificationNumberTracker = "";
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            AddData("Notification Noticed", "Called:0", 2);
        }
    }

    public void SetSceneNamesAndLoad(string newSceneName)
    {
        sceneChanged = true;
        if (sceneSystem.IsContentLoaded("Instructions Scene"))
            unloadSceneName = "Instructions Scene";
        else if (sceneSystem.IsContentLoaded("NoO_WS Scene"))
            unloadSceneName = "NoO_WS Scene";
        else if (sceneSystem.IsContentLoaded("NoO_WA Scene"))
            unloadSceneName = "NoO_WA Scene";
        else if (sceneSystem.IsContentLoaded("NoV_WOS Scene"))
            unloadSceneName = "NoV_WOS Scene";
        else if (sceneSystem.IsContentLoaded("Control_WOS Scene"))
            unloadSceneName = "Control_WOS Scene";
        /*
        else if (sceneSystem.IsContentLoaded("NoO_WOS Scene"))
            unloadSceneName = "NoO_WOS Scene";
        else if (sceneSystem.IsContentLoaded("NoV_WS Scene"))
            unloadSceneName = "NoV_WS Scene";
        else if (sceneSystem.IsContentLoaded("NoV_WOS Scene"))
            unloadSceneName = "NoV_WOS Scene";
        else if (sceneSystem.IsContentLoaded("Control_WS Scene"))
            unloadSceneName = "Control_WS Scene";
        */

        switch (newSceneName)
        {
            case "Instructions Scene":
                showNotification = false;
                notificationSound = false;
                StudyInstructionsManager.instance.ResetInstructionNumber();
                break;
            case "NoO_WS Scene":
                showNotification = true;
                notificationSound = true;
                break;
            case "NoO_WA Scene":
                showNotification = true;
                notificationSound = false;
                break;
            case "NoV_WOS Scene":
                showNotification = true;
                notificationSound = false;
                break;
            case "Control_WOS Scene":
                showNotification = false;
                notificationSound = false;
                break;
            /*
            case "NoO_WOS Scene":
                showNotification = true;
                notificationSound = false;
                break;
            case "NoV_WS Scene":
                showNotification = true;
                notificationSound = true;
                break;
            case "NoV_WOS Scene":
                showNotification = true;
                notificationSound = false;
                break;
            case "Control_WS Scene":
                showNotification = false;
                notificationSound = true;
                break;

            */
        }
        GameManager.instance.SetSceneName(newSceneName);
        StudyInstructionsManager.instance.DisplayInstructionsScreen(GameState.Scene);
        var task = LoadNextLevel(newSceneName);
    }

    public async Task LoadNextLevel(string sceneName)
    {
        await sceneSystem.UnloadContent(unloadSceneName);
        await sceneSystem.LoadContent(sceneName);
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }

    public bool GetShowNotification()
    {
        return showNotification;
    }

    public void SetShowNotification(bool sNotifi)
    {
        showNotification = sNotifi;
    }

    public void SetCurrGlobalRecordsGO(GameObject currObject)
    {
        currGlobalRecordsGO = currObject;
        sceneChanged = false;
        foreach (var independentData in independentCSVData)
        {
            csvData.AppendLine(participantNumber + "," + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "," + time_s + "," + currGlobalRecordsGO.GetComponent<Records>().GetNotificationType() + "," + notificationSound + "," + independentData);
        }
        independentCSVData.Clear();
    }

    public bool GetNotificationSound()
    {
        return notificationSound;
    }

    public void SetNotificationSound(bool nSound)
    {
        notificationSound = nSound;
    }

    //============================== Study ==============================//

    public void SetParticipantNumber(int pNum)
    {
        participantNumber = pNum;
        filePath = filePath + "/Participant" + participantNumber.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".csv";
        using (writer = File.CreateText(filePath))
        {
            writer.WriteLine("Participant_Number,Timestamp,Time_s,Notification_Type,Notification_Sound,Category,Action,Status,Ingredients");
        }
        csvData = new StringBuilder();
    }

    public int GetParticipantNumber()
    {
        return participantNumber;
    }

    public void AddData(string category="n/a", string action="n/a", int status=0, string ingredients="n/a")
    {
        /*
         * status (0=n/a; 1=start; 2=end)
         */
        if (sceneChanged)
            independentCSVData.Add(category + "," + action + "," + status + "," + ingredients);
        else
            csvData.AppendLine(participantNumber + "," + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "," + time_s + "," + currGlobalRecordsGO.GetComponent<Records>().GetNotificationType() + "," + notificationSound + "," + category + "," + action + "," + status + "," + ingredients);
        if (csvData.Length >= FlushAfter)
        {
            FlushData();
        }
    }

    void FlushData()
    {
        using (var csvWriter = new StreamWriter(filePath, true))
        {
            csvWriter.Write(csvData.ToString());
        }
        csvData.Clear();
    }

    public void EndCSV()
    {
        if (csvData == null)
        {
            return;
        }
        using (var csvWriter = new StreamWriter(filePath, true))
        {
            csvWriter.Write(csvData.ToString());
        }
        csvData = null;
    }

    private void OnDestroy()
    {
        EndCSV();
    }

    void ShowInstructions(int instructionNum)
    {
        if (!StudyBillboard.activeSelf)
        {
            StudyBillboard.SetActive(true);
        }
        StudyBillboard.GetComponentInParent<StudyInstructionsManager>().SetNextInstruction(instructionNum);
    }

    public GameObject GetStudyBillboard()
    {
        return StudyBillboard;
    }

    public int GetSceneIndex()
    {
        return sceneIndex;
    }

    public string GetNextScene()
    {
        return sceneNames[sceneIndex++];
    }

    public void SetSceneOrder()
    {
        List<string> tempSceneList = new List<string>(sceneNames.Count);
        sceneNames.ForEach((item) =>
        {
            tempSceneList.Add((string)item.Clone());
        });

        switch (participantNumber % 24)
        {
            case 0:
                sceneNames[0] = tempSceneList[0];
                sceneNames[1] = tempSceneList[1];
                sceneNames[2] = tempSceneList[2];
                sceneNames[3] = tempSceneList[3];
                break;
            case 10:
                sceneNames[0] = tempSceneList[0];
                sceneNames[1] = tempSceneList[1];
                sceneNames[2] = tempSceneList[3];
                sceneNames[3] = tempSceneList[2];
                break;
            case 21:
                sceneNames[0] = tempSceneList[0];
                sceneNames[1] = tempSceneList[2];
                sceneNames[2] = tempSceneList[1];
                sceneNames[3] = tempSceneList[3];
                break;
            case 3:
                sceneNames[0] = tempSceneList[0];
                sceneNames[1] = tempSceneList[2];
                sceneNames[2] = tempSceneList[3];
                sceneNames[3] = tempSceneList[1];
                break;
            case 14:
                sceneNames[0] = tempSceneList[0];
                sceneNames[1] = tempSceneList[3];
                sceneNames[2] = tempSceneList[1];
                sceneNames[3] = tempSceneList[2];
                break;
            case 17:
                sceneNames[0] = tempSceneList[0];
                sceneNames[1] = tempSceneList[3];
                sceneNames[2] = tempSceneList[2];
                sceneNames[3] = tempSceneList[1];
                break;
            case 6:
                sceneNames[0] = tempSceneList[1];
                sceneNames[1] = tempSceneList[0];
                sceneNames[2] = tempSceneList[2];
                sceneNames[3] = tempSceneList[3];
                break;
            case 7:
                sceneNames[0] = tempSceneList[1];
                sceneNames[1] = tempSceneList[0];
                sceneNames[2] = tempSceneList[3];
                sceneNames[3] = tempSceneList[2];
                break;
            case 18:
                sceneNames[0] = tempSceneList[1];
                sceneNames[1] = tempSceneList[2];
                sceneNames[2] = tempSceneList[0];
                sceneNames[3] = tempSceneList[3];
                break;
            case 9:
                sceneNames[0] = tempSceneList[1];
                sceneNames[1] = tempSceneList[2];
                sceneNames[2] = tempSceneList[3];
                sceneNames[3] = tempSceneList[0];
                break;
            case 1:
                sceneNames[0] = tempSceneList[1];
                sceneNames[1] = tempSceneList[3];
                sceneNames[2] = tempSceneList[0];
                sceneNames[3] = tempSceneList[2];
                break;
            case 15:
                sceneNames[0] = tempSceneList[1];
                sceneNames[1] = tempSceneList[3];
                sceneNames[2] = tempSceneList[2];
                sceneNames[3] = tempSceneList[0];
                break;
            case 12:
                sceneNames[0] = tempSceneList[2];
                sceneNames[1] = tempSceneList[1];
                sceneNames[2] = tempSceneList[0];
                sceneNames[3] = tempSceneList[3];
                break;
            case 13:
                sceneNames[0] = tempSceneList[2];
                sceneNames[1] = tempSceneList[1];
                sceneNames[2] = tempSceneList[3];
                sceneNames[3] = tempSceneList[0];
                break;
            case 4:
                sceneNames[0] = tempSceneList[2];
                sceneNames[1] = tempSceneList[0];
                sceneNames[2] = tempSceneList[1];
                sceneNames[3] = tempSceneList[3];
                break;
            case 11:
                sceneNames[0] = tempSceneList[2];
                sceneNames[1] = tempSceneList[0];
                sceneNames[2] = tempSceneList[3];
                sceneNames[3] = tempSceneList[1];
                break;
            case 16:
                sceneNames[0] = tempSceneList[2];
                sceneNames[1] = tempSceneList[3];
                sceneNames[2] = tempSceneList[1];
                sceneNames[3] = tempSceneList[0];
                break;
            case 5:
                sceneNames[0] = tempSceneList[2];
                sceneNames[1] = tempSceneList[3];
                sceneNames[2] = tempSceneList[0];
                sceneNames[3] = tempSceneList[1];
                break;
            case 8:
                sceneNames[0] = tempSceneList[3];
                sceneNames[1] = tempSceneList[1];
                sceneNames[2] = tempSceneList[2];
                sceneNames[3] = tempSceneList[0];
                break;
            case 19:
                sceneNames[0] = tempSceneList[3];
                sceneNames[1] = tempSceneList[1];
                sceneNames[2] = tempSceneList[0];
                sceneNames[3] = tempSceneList[2];
                break;
            case 20:
                sceneNames[0] = tempSceneList[3];
                sceneNames[1] = tempSceneList[2];
                sceneNames[2] = tempSceneList[1];
                sceneNames[3] = tempSceneList[0];
                break;
            case 2:
                sceneNames[0] = tempSceneList[3];
                sceneNames[1] = tempSceneList[2];
                sceneNames[2] = tempSceneList[0];
                sceneNames[3] = tempSceneList[1];
                break;
            case 22:
                sceneNames[0] = tempSceneList[3];
                sceneNames[1] = tempSceneList[0];
                sceneNames[2] = tempSceneList[1];
                sceneNames[3] = tempSceneList[2];
                break;
            case 23:
                sceneNames[0] = tempSceneList[3];
                sceneNames[1] = tempSceneList[0];
                sceneNames[2] = tempSceneList[2];
                sceneNames[3] = tempSceneList[1];
                break;
        }
    }

}
