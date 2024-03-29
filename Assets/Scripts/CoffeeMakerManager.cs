using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMakerManager : MonoBehaviour
{

    [SerializeField] Vector3 coffeePotPos;
    [SerializeField] GameObject playArea;
    [SerializeField] GameObject coffeePotGlass;
    [SerializeField] GameObject coffeeMakerBody;
    [SerializeField] GameObject coffeeLevel_GO;
    [SerializeField] GameObject coffeeLevelMeter;
    [SerializeField] float coffeeFillRate = 2;
    [SerializeField] float maxCoffeeLevel = 0.05f;
    [SerializeField] GameObject globalRecords_GO;

    bool coffeeMakerOn = true;
    Renderer rend;
    float coffeeLevel = -0.05f;
    int coffeeCupCnt = 0;
    GameObject notification_GO;

    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        Physics.IgnoreCollision(coffeePotGlass.GetComponent<MeshCollider>(), coffeeMakerBody.GetComponent<MeshCollider>(), true);
        rend = coffeeLevel_GO.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (StudyInstructionsManager.instance.GetStartTask())
        {
            if (coffeeMakerOn && coffeeLevel < maxCoffeeLevel && transform.GetChild(1).GetComponent<CoffeePotManager>().GetPlaced())
            {
                coffeeLevel += coffeeFillRate * 0.001f * Time.deltaTime;
                rend.material.SetFloat("FillLevel", coffeeLevel);
            }
            else if (coffeeMakerOn && coffeeLevel >= maxCoffeeLevel && transform.GetChild(1).GetComponent<CoffeePotManager>().GetPlaced())
            {
                TurnOnCoffeeMaker();
            }

            if (((coffeeLevel + maxCoffeeLevel) * 3) / (maxCoffeeLevel * 2) > coffeeCupCnt + 1)
            {
                coffeeCupCnt++;
                if (globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().GetShowNotification())
                {
                    int notificationNumber = globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().GetNumber();
                    switch (globalRecords_GO.GetComponent<Records>().GetNotificationType())
                    {
                        case 0:
                            if (notification_GO != null)
                                Destroy(notification_GO);
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject(notificationNumber, "Coffee", "Coffee cup added", transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Coffee", "Coffee cup added", transform.gameObject, new Vector3(0, 0.25f, 0), scale: new Vector3(1.7f, 1.7f, 0.566666667f));
                            break;
                        case 1:
                            globalRecords_GO.GetComponent<Records>().AddNotificationOnDock(notificationNumber, "Coffee", "Coffee cup added", transform.GetInstanceID());
                            break;
                        case 2:
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnViewport(notificationNumber, "Coffee", "Coffee cup added", transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Coffee", "Coffee cup added", globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().GetNotificationBillboard(), new Vector3(0, 0.1f, 0), Quaternion.identity);
                            break;
                        case 3:
                            if (notification_GO != null)
                                Destroy(notification_GO);
                            notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject(notificationNumber, "Coffee", "Coffee cup added", transform.GetInstanceID());
                            notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Coffee", "Coffee cup added", transform.gameObject, new Vector3(0, 0.25f, 0), scale: new Vector3(1.7f, 1.7f, 0.566666667f));
                            break;
                    }
                }
                if (globalRecords_GO.GetComponent<Records>().GetNotificationType() == 4 && PersistentGOManager.instance.GetNotificationSound())
                {
                    PersistentGOManager.instance.GetComponent<PersistentGOManager>().AddData("Notification", "Coffee cup added" + ":" + transform.GetInstanceID().ToString(), 1);
                    Camera.main.transform.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    public Vector3 GetCoffeePotPos()
    {
        return coffeePotPos;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Coffee_Pot") && !other.GetComponent<CoffeePotManager>().GetPlaced()){
            if (!other.GetComponent<CoffeePotManager>().GetIsGrabbed())
            {
                // other.transform.parent = transform;
                other.transform.localPosition = coffeePotPos;
                other.transform.localRotation = Quaternion.identity;
                other.GetComponent<CoffeePotManager>().SetPlaced(true);
                coffeeLevel = other.GetComponent<CoffeePotManager>().GetCoffeeLevel();
            }
        }
        if (other.CompareTag("Coffee_Pot") && other.GetComponent<CoffeePotManager>().GetPlaced() && !coffeeLevelMeter.activeSelf)
        {
            coffeeLevelMeter.SetActive(true);
            coffeeLevel_GO.SetActive(true);
        }
    }

    public void TurnOnCoffeeMaker()
    {
        if (coffeeMakerOn)
        {
            coffeeMakerOn = false;
        }
        else
        {
            coffeeMakerOn = true;
            coffeeLevel = rend.material.GetFloat("FillLevel");
        }
    }

    public float GetCoffeeLevel()
    {
        return coffeeLevel;
    }
}
