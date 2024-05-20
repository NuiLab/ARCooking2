using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServingStationManager : MonoBehaviour
{
    [SerializeField] GameObject customerPrefab;
    [SerializeField] GameObject[] customerPositionGO;
    [SerializeField] GameObject globalRecords_GO;
    float customerDuration = 120; 
    int maxCustomerCount = 6;

    Dictionary<int, GameObject> customers = new Dictionary<int, GameObject>();
    Vector3[] customerPositions = new Vector3[3];
    List<string> foodItems = new List<string>() { "Pizza", "Burger", "Coffee" };
    List<string> foodItemsLeft = new List<string>() { "Pizza", "Burger", "Coffee" };
    int numCustomers = 0;
    int totalCustomers = 0;
    string[] currCustomerNames;
    GameObject notification_GO;
    bool pauseCustCntCheck = false;


    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        for (int i = 0; i < customerPositionGO.Length; i++)
        {
            customers.Add(i, null);
            customerPositions[i] = customerPositionGO[i].transform.localPosition;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (StudyInstructionsManager.instance.GetStartTask())
        {
            if (numCustomers < 3 && !pauseCustCntCheck && totalCustomers < maxCustomerCount)
            {
                pauseCustCntCheck = true;
                StartCoroutine(BringCustomer());
            }
            if (totalCustomers >= maxCustomerCount)
            {
                StopCoroutine(BringCustomer());
                StartCoroutine(WaitAndChangeScene());
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void AddCustomer(int custPos, GameObject custRef)
    {
        numCustomers++;
        totalCustomers++;
        PersistentGOManager.instance.AddData("Customer", custRef.name + totalCustomers.ToString() + ":" + custRef.GetInstanceID().ToString(), 1);
        customers[custPos] = custRef;
        custRef.transform.parent = transform;
        custRef.transform.localPosition = customerPositions[custPos];
        custRef.transform.localRotation = Quaternion.identity;
        custRef.GetComponent<CustomerManager>().CreateCustomer(customerDuration, GetFoodItem(), custPos, currCustomerNames, totalCustomers);
        custRef.GetComponent<CustomerManager>().SetObjectID(custRef.GetInstanceID().ToString());
        globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().AddData("Food Requested", custRef.name + totalCustomers.ToString() + ":" + custRef.GetInstanceID().ToString(), 1, custRef.GetComponent<CustomerManager>().CreateIngredientsString());

        if (globalRecords_GO.GetComponent<Records>().GetPersistentGO().GetComponent<PersistentGOManager>().GetShowNotification())
        {
            int notificationNumber = globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().GetNumber();
            switch (globalRecords_GO.GetComponent<Records>().GetNotificationType())
            {
                case 0:
                    notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject(notificationNumber, "Customer", "New Customer", custRef.transform.GetInstanceID());
                    notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Customer", "New Customer", custRef, new Vector3(0, 0.05f, -0.05f));
                    break;
                case 1:
                    globalRecords_GO.GetComponent<Records>().AddNotificationOnDock(notificationNumber, "Customer", "New Customer", custRef.transform.GetInstanceID());
                    break;
                case 2:
                    notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnViewport(notificationNumber, "Customer", "New Customer", custRef.transform.GetInstanceID());
                    notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Customer", "New Customer", globalRecords_GO.GetComponent<Records>().GetNotificationSetManager().GetComponent<NotificationSetManager>().GetNotificationBillboard(), new Vector3(0, -0.1f, 0), Quaternion.identity, new Vector3(2f, 2f, 2f));
                    break;
                case 3:
                    notification_GO = globalRecords_GO.GetComponent<Records>().AddNotificationOnObject(notificationNumber, "Customer", "New Customer", custRef.transform.GetInstanceID());
                    notification_GO.GetComponent<NotificationManager>().SetNotificationProperties(notificationNumber, "Customer", "New Customer", custRef, new Vector3(0, 0.05f, -0.05f));
                    break;
            }
        }
        if ((globalRecords_GO.GetComponent<Records>().GetNotificationType() == 2 || globalRecords_GO.GetComponent<Records>().GetNotificationType() == 5) && PersistentGOManager.instance.GetNotificationSound())
        {
            PersistentGOManager.instance.GetComponent<PersistentGOManager>().AddData("Notification", "Customer:New Customer" + ":" + custRef.GetInstanceID().ToString(), 1);
            Camera.main.transform.GetComponent<AudioSource>().Play();
        }
    }

    public void RemoveCustomer(GameObject cust)
    {
        for (int i = 0; i < numCustomers; i++)
        {
            if (customers[i] != null)
            {
                if (customers[i].Equals(cust))
                {
                    customers[i] = null;
                    break;
                }
            }
        }
        numCustomers--;
        PersistentGOManager.instance.AddData("Customer", cust.name + ":" + cust.GetInstanceID().ToString(), 2);
    }

    private IEnumerator BringCustomer()
    {
        yield return new WaitForSeconds(5);
        GameObject tempCustomer = Instantiate(customerPrefab);
        for (int i = 0; i < 3; i++)
        {
            if (customers[i] == null)
            {
                currCustomerNames = CustomerNames();
                AddCustomer(i, tempCustomer);
                break;
            }
        }
        pauseCustCntCheck = false;
    }

    string GetFoodItem()
    {
        if (foodItemsLeft.Count == 0)
            foodItemsLeft = new List<string>(foodItems);
        int foodItemIndex = Random.Range(0, foodItemsLeft.Count);
        string foodItem = foodItemsLeft[foodItemIndex];
        foodItemsLeft.RemoveRange(foodItemIndex, 1);
        return foodItem;
    }

    string[] CustomerNames()
    {
        string[] names = new string[transform.childCount - 3];

        for (int i = 3; i < transform.childCount; i++)
        {
            names[i - 3] = GetPrefabName(transform.GetChild(i).GetChild(4).gameObject);
        }
        return names;
    }

    string GetPrefabName(GameObject temp_GO)
    {
        string objectName = temp_GO.transform.name;
        for (int i = 0; i < objectName.Length; i++)
        {
            if (((objectName[i] - '0') <= 9 && (objectName[i] - '0') >= 0) || objectName[i] == '(')     // Prefab names are only made of alphabets. Number or Brackets signify copies
            {
                return objectName.Substring(0, i);
            }
        }
        return objectName;
    }

    public int GetCustomerCount()
    {
        return totalCustomers;
    }

    private IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(customerDuration + 3);
        if (PersistentGOManager.instance.GetSceneIndex() < 4)
            GameManager.instance.UpdateGameState(GameState.Scene);
        else
            GameManager.instance.UpdateGameState(GameState.End);
    }

}
