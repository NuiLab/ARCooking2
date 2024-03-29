using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenManager : MonoBehaviour
{
    [SerializeField] GameObject pizzaPos;
    [SerializeField] float cookingSpeed;
    [SerializeField] GameObject progressText_GO;
    [SerializeField] GameObject globalRecords_GO;

    bool cooking = false;
    float cookingProgress = 0;
    int prevQuotient = 0;
    GameObject food;
    GameObject notification_GO;


    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
    }

    // Update is called once per frame
    void Update()
    {
        if (cooking && food.GetComponent<IngredientProperties>().GetCookingStatus() != "Burnt")
        {
            cookingProgress += Time.deltaTime;
            if (cookingProgress / cookingSpeed > prevQuotient + 1)
            {
                prevQuotient = (int)Math.Floor(cookingProgress / cookingSpeed);
                food.GetComponent<IngredientProperties>().SetCookingStatus(prevQuotient, "Pizza");
                switch (prevQuotient)
                {
                    case 0:
                        progressText_GO.GetComponent<TextMesh>().text = "Uncooked";
                        break;
                    case 1:
                        progressText_GO.GetComponent<TextMesh>().text = "Cooked";
                        break;
                    case 2:
                        progressText_GO.GetComponent<TextMesh>().text = "Burnt";
                        cooking = false;
                        break;
                }
                /*
                switch (prevQuotient)
                {
                    case 0:
                        progressText_GO.GetComponent<TextMesh>().text = "Uncooked";
                        break;
                    case 1:
                        progressText_GO.GetComponent<TextMesh>().text = "Partially Cooked";
                        break;
                    case 2:
                        progressText_GO.GetComponent<TextMesh>().text = "Cooked";
                        break;
                    case 3:
                        progressText_GO.GetComponent<TextMesh>().text = "Over Cooked";
                        break;
                    case 4:
                        progressText_GO.GetComponent<TextMesh>().text = "Burnt";
                        cooking = false;
                        break;
                }*/
                if (globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().GetShowNotification())
                {
                    int notificationNumber = globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().GetNumber();
                    switch (globalRecords_GO.GetComponent<Records>().GetNotificationType())
                    {
                        case 0:
                            if (notification_GO != null)
                                Destroy(notification_GO);
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.gameObject, new Vector3(-0.2f, 0.5f, 0), new Quaternion(0, 0.707106829f, 0, 0.707106829f), new Vector3(4, 4, 1.33333337f));
                            break;
                        case 1:
                            globalRecords_GO.GetComponent<Records>().AddNotificationOnDock(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.GetInstanceID());
                            break;
                        case 2:
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnViewport(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().GetNotificationBillboard(), new Vector3(0, 0.1f, 0), Quaternion.identity);
                            break;
                        case 3:
                            if (notification_GO != null)
                                Destroy(notification_GO);
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Pizza", progressText_GO.GetComponent<TextMesh>().text, transform.gameObject, new Vector3(-0.2f, 0.5f, 0), new Quaternion(0, 0.707106829f, 0, 0.707106829f), new Vector3(4, 4, 1.33333337f));
                            break;
                    }
                }
                if (globalRecords_GO.GetComponent<Records>().GetNotificationType() == 4 && PersistentGOManager.instance.GetNotificationSound())
                {
                    PersistentGOManager.instance.GetComponent<PersistentGOManager>().AddData("Notification", "Pizza:" + progressText_GO.GetComponent<TextMesh>().text + ":" + transform.GetInstanceID().ToString(), 1);
                    Camera.main.transform.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ingredient_Base") && !other.gameObject.GetComponent<ObjectManager>().isGrabbed && !cooking)
        {
            food = other.gameObject;
            other.transform.position = pizzaPos.transform.position;
            other.transform.rotation = Quaternion.identity;
            cooking = true;
            progressText_GO.GetComponent<TextMesh>().text = other.GetComponent<IngredientProperties>().GetCookingStatus();
            switch (other.GetComponent<IngredientProperties>().GetCookingStatus())
            {
                case "Uncooked":
                    cookingProgress = 0;
                    prevQuotient = 0;
                    break;
                case "Cooked":
                    cookingProgress = cookingSpeed * 1;
                    break;
                case "Burnt":
                    cookingProgress = cookingSpeed * 2;
                    break;
            }
            /*
            switch (other.GetComponent<IngredientProperties>().GetCookingStatus())
            {
                case "Uncooked":
                    cookingProgress = 0;
                    break;
                case "Partially Cooked":
                    cookingProgress = cookingSpeed * 1;
                    break;
                case "Cooked":
                    cookingProgress = cookingSpeed * 2;
                    break;
                case "Over Cooked":
                    cookingProgress = cookingSpeed * 3;
                    break;
                case "Burnt":
                    cookingProgress = cookingSpeed * 4;
                    break;
            }*/
        }
        else if (other.CompareTag("Ingredient_Base") && other.gameObject.GetComponent<ObjectManager>().isGrabbed)
        {
            cooking = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ingredient_Base") && other.gameObject.GetComponent<ObjectManager>().isGrabbed)
        {
            other.transform.GetChild(1).gameObject.SetActive(false);
            progressText_GO.GetComponent<TextMesh>().text = "Place Food";
            if (notification_GO != null)
                Destroy(notification_GO);
        }
    }
}
