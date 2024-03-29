using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationSetManager : MonoBehaviour
{
    [SerializeField] GameObject notificationDock;
    [SerializeField] GameObject notificationBillboard;
    [SerializeField] int notificationType;
    /*
     * 0: Notification on Object
     * 1: Notification on Dock
     * 2: Notification on Viewport
     * 3: Notification on Object with Arrow
     */
    [SerializeField] GameObject[] notificationPrefabs;

    GameObject persistentGO;
    GameObject globalRecordsGO;
    GameObject tempNotification;
    List<string> notifications = new List<string>();
    List<string> stations = new List<string>();
    List<int> gameObjectId = new List<int>();
    bool waitTimeStarted = false;
    List<int> pool;
    int poolCount = 0;
    HashSet<int> inUse;


    // Start is called before the first frame update
    void Start()
    {
        globalRecordsGO = GameObject.FindWithTag("Global Records");
        persistentGO = GameObject.FindGameObjectsWithTag("PersistentGO")[0];
        bool nSound = persistentGO.GetComponent<PersistentGOManager>().GetNotificationSound();
        foreach (var notifications in globalRecordsGO.GetComponent<Records>().GetAllNotificationPrefabs())
            notifications.GetComponent<AudioSource>().mute = !nSound;
        if (notificationType == 1)
            notificationDock.GetComponent<AudioSource>().mute = !nSound;

        // Initialize the pool with numbers from minNumber to maxNumber
        pool = new List<int>();
        inUse = new HashSet<int>();

        for (int i = 0; i < 15; i++)
        {
            pool.Add(i);
            poolCount++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tempNotification == null && notifications.Count > 0 && !waitTimeStarted)
        {
            waitTimeStarted = true;
            StartCoroutine(CreateNotification(0.5f));
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public int GetNotificationType()
    {
        return notificationType;
    }

    public void SetNotificationType(int notifiType)
    {
        notificationType = notifiType;
    }

    public GameObject GetNotificationPrefab()
    {
        return notificationPrefabs[notificationType];
    }


    public GameObject AddNotificationOnObject(int objectId)
    {
        gameObjectId.Add(objectId);
        return Instantiate(notificationPrefabs[notificationType]);
    }

    public void AddNotificationOnDock(string stationTxt, string notificationTxt, int objectId)
    {
        notificationDock.GetComponent<NotificationDockManager>().AddNotification(GetNumber(), stationTxt, notificationTxt, objectId);
    }

    public void RemoveNotificationOnDock(GameObject cutletGO)
    {
        notificationDock.GetComponent<NotificationDockManager>().RemoveNotification(cutletGO);
    }

    public GameObject AddNotificationOnViewport(string stationTxt, string notificationTxt, int objectId, int notifiNum)
    {
        if (tempNotification != null)
        {
            StartCoroutine(CreateNotification(0.5f));
            Destroy(tempNotification);
            StartCoroutine(CreateNotification(0.5f));
        }
        gameObjectId.Add(objectId);
        // return Instantiate(notificationPrefabs[notificationType]);
        tempNotification = Instantiate(notificationPrefabs[notificationType]);
        // tempNotification.GetComponent<NotificationManager>().SetNotificationProperties(notifiNum, "Customer", "New Customer", notificationBillboard, new Vector3(0, 0.1f, 0), Quaternion.identity);
        return tempNotification;
        
        /*
        if (gameObjectId.Contains(objectId))
        {
            int tempIndex = gameObjectId.IndexOf(objectId);
            gameObjectId.Remove(objectId);
            notifications.RemoveAt(tempIndex);
            stations.RemoveAt(tempIndex);
        }
        gameObjectId.Add(objectId);
        notifications.Add(notificationTxt);
        stations.Add(stationTxt);
        */
    }

    public int GetNumber()
    {
        if (pool.Count == 0)
        {
            AddNumbers(1);
        }

        int number = pool[Random.Range(0, pool.Count)];
        pool.Remove(number);

        inUse.Add(number);

        return number;
    }

    public void ReturnNumber(int number)
    {
        // if (!inUse.Contains(number))
        // {
        //     throw new Exception("Number is not in use.");
        // }

        inUse.Remove(number);

        pool.Add(number);
    }

    public void AddNumbers(int count)
    {
        int startNumber = poolCount + 1;

        for (int i = 0; i < count; i++)
        {
            int newNumber = startNumber + i;
            pool.Add(newNumber);
        }
    }

    IEnumerator CreateNotification(float duration)
    {
        yield return new WaitForSeconds(duration);

        /*
        tempNotification = Instantiate(notificationPrefabs[notificationType]);
        tempNotification.transform.parent = notificationBillboard.transform;
        tempNotification.transform.localPosition = new Vector3(0, 0.1f, 0);
        tempNotification.transform.localRotation = Quaternion.identity;
        tempNotification.transform.Find("IconAndText").Find("Station Text").GetComponent<TextMeshPro>().text = stations[0];
        tempNotification.transform.Find("IconAndText").Find("Notification Text").GetComponent<TextMeshPro>().text = notifications[0];
        notifications.RemoveAt(0);
        stations.RemoveAt(0);
        waitTimeStarted = false;
        */
    }

    public GameObject GetNotificationBillboard()
    {
        return notificationBillboard;
    }
}
