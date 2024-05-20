using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] GameObject notificationTxt_GO;
    [SerializeField] GameObject stationTxt_GO;
    [SerializeField] GameObject notificationNumber_GO;
    [SerializeField] GameObject indicator_GO;
    [SerializeField] float duration;
    public GameObject globalRecords_GO;     // Reference to global records
    int gameObjectId = 0;
    bool burgerNotification = false;
    GameObject cutletGO;
    Vector3 relativePos = new Vector3(0,0,0);
    IMixedRealitySceneSystem sceneSystem;
    int notificationNumber = -1;

    private void Awake()
    {
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        if (duration > 0)
            StartCoroutine(NotificationDuration(duration));
        globalRecords_GO = GameObject.FindWithTag("Global Records");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(0) || globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(3))
        {
            if (burgerNotification)
            {
                if (cutletGO == null)
                    Destroy(transform.gameObject);
                else
                    transform.localPosition = cutletGO.transform.localPosition + relativePos;
            }
        }
    }

    private void OnDestroy()
    {
        if (!globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(2))
        {
            globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().ReturnNumber(notificationNumber);
        }
        Destroy(indicator_GO);
        StopAllCoroutines();
    }

    public void setText(string stationTxt, string notificationTxt)
    {
        notificationNumber_GO.GetComponent<TextMeshPro>().text = notificationNumber.ToString();
        notificationTxt_GO.GetComponent<TextMeshPro>().text = notificationTxt;
        stationTxt_GO.GetComponent<TextMeshPro>().text = stationTxt;
    }

    public string getText(string txtType)
    {
        if (txtType == "Station")
            return stationTxt_GO.GetComponent<TextMeshPro>().text;
        else
            return notificationTxt_GO.GetComponent<TextMeshPro>().text;
    }

    IEnumerator PauseForSound()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(transform.gameObject);
    }

    public void OnNotificationClick()
    {
        if (globalRecords_GO.GetComponent<Records>().GetNotificationType() == 1)
            transform.parent.parent.GetComponent<NotificationDockManager>().ManageNotificationLayout(gameObject);
        if (transform.parent.name.Equals("Customer(Clone)"))
            globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().AddData("Notification", stationTxt_GO.GetComponent<TextMeshPro>().text + ":" + notificationTxt_GO.GetComponent<TextMeshPro>().text + ":" + transform.parent.GetInstanceID().ToString(), 2);
        else
            globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().AddData("Notification", stationTxt_GO.GetComponent<TextMeshPro>().text + ":" + notificationTxt_GO.GetComponent<TextMeshPro>().text + ":" + transform.parent.GetInstanceID().ToString(), 2);
        StartCoroutine(PauseForSound());
    }

    public void SetNotificationProperties(int notifiNum, string stationTxt, string notificationTxt, GameObject parent, Vector3? pos = null, Quaternion? rot = null, Vector3? scale = null, GameObject cutletGameObject = null)
    {
        if (pos == null)
            pos = Vector3.zero;
        if (rot == null)
            rot = Quaternion.identity;
        if (scale == null)
            scale = transform.localScale;
        notificationNumber = notifiNum;
        setText(stationTxt, notificationTxt);
        transform.SetParent(parent.transform);
        if (stationTxt == "Burger")
        {
            relativePos = pos.Value;
            cutletGO = cutletGameObject;
            burgerNotification = true;
        }
        transform.localPosition = pos.Value;
        transform.localRotation = rot.Value;
        transform.localScale = scale.Value;
        // gameObjectId = objectId;
        if (globalRecords_GO.GetComponent<Records>().GetNotificationType().Equals(3))
            indicator_GO.transform.SetParent(globalRecords_GO.transform);
    }

    IEnumerator NotificationDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(transform.gameObject);
    }

    public void SetObjectId(int objectId)
    {
        gameObjectId = objectId;
    }

    public int GetObjectId()
    {
        return gameObjectId;
    }

    public void OnNotificationCalled()
    {
        globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().AddData("Notification Noticed", "Called:0", 2);
        OnNotificationClick();
    }
}
