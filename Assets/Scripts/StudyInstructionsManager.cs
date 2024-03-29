using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StudyInstructionsManager : MonoBehaviour
{
    [SerializeField] GameObject mainText;
    [SerializeField] GameObject continueBtn;

    public static StudyInstructionsManager instance;

    IMixedRealitySceneSystem sceneSystem;
    List<string> instructions = new List<string>();
    int instructionsNum = -1;
    Dictionary<string, string> sceneInstructions = new Dictionary<string, string>();
    bool startTasks = false;


    private void Awake()
    {
        instance = this;
        sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        InitializeInstructions();
        InitializeSceneInstructions();
        GameManager.OnGameStateChanged += DisplayInstructionsScreen;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= DisplayInstructionsScreen;
    }

    public void DisplayInstructionsScreen(GameState state)
    {
        startTasks = false;
        if (!state.Equals(GameState.Setup) && instructionsNum != -1)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }


        switch (state)
        {
            case GameState.Setup:
                instructionsNum = 0;
                break;
            case GameState.Welcome:
                if (instructionsNum == -1)
                {
                    instructionsNum = 0;
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                mainText.GetComponent<TextMeshProUGUI>().text = instructions[instructionsNum++];
                break;
            case GameState.Scene:
                if (GameManager.instance.GetSceneName().Equals("NoO_WS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["NoO_WS Scene"];
                else if (GameManager.instance.GetSceneName().Equals("NoO_WA Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["NoO_WA Scene"];
                else if (GameManager.instance.GetSceneName().Equals("NoV_WOS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["NoV_WOS Scene"];
                /*
                else if (GameManager.instance.GetSceneName().Equals("NoO_WOS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["NoO_WOS Scene"];
                else if (GameManager.instance.GetSceneName().Equals("NoV_WS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["NoV_WS Scene"];
                else if (GameManager.instance.GetSceneName().Equals("NoV_WOS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["NoV_WOS Scene"];
                else if (GameManager.instance.GetSceneName().Equals("Control_WS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["Control_WS Scene"];
                else if (GameManager.instance.GetSceneName().Equals("Control_WOS Scene"))
                    mainText.GetComponent<TextMeshProUGUI>().text = sceneInstructions["Control_WOS Scene"];
                */
                break;
            case GameState.End:
                mainText.GetComponent<TextMeshProUGUI>().text = instructions[instructions.Count - 1];
                continueBtn.SetActive(false);
                break;
        }
    }


    public GameObject GetMainText()
    {
        return mainText;
    }

    void InitializeSceneInstructions()
    {
        sceneInstructions.Add("Instructions Scene", "In this section you will follow a tutorial to learn how to interact with the objects. " +
            "There will be instructions with an arrow pointing to the object used to complete the current task. If the instruction is not in the viewport there will be an arrow guiding you to it. " +
            "After completing each task press the right arrow to go to the next instruction. \n\n" +
            "There will be 3 customers each asking for different food item. You will have all the time you need to present them the food item they requested. " +
            "After preparing the food place it on the tray in front of the customer. If the food item is wrong, there will be a text stating it. If the food item is correct the customer will leave. \n\n" +
            "Once you are done serving all customers, you can press the next button on the last instruction (located at the left of trash can) to start the actual experiment.");

        sceneInstructions.Add("NoD_WS Scene", "In this section you will be presented with a Dock where all the notifications will be stored. " +
            "You can think of this dock similar to how notifications are displayed on your phone when you swipe down. " +
            "You can place the dock wherever you want by moving it how you move any other objects. \n\n " +
            "There is a button next to the dock which serves three purposes:\n " +
            "1. Clicking the button allows you to show or hide the notifications.\n 2. The number on the button shows number of non-interacted notifications.\n " +
            "3. Red color represents there is a new notification and Blue color represents you have looked at all notifications (this doesn't mean you have interacted with it.)\n\n " +
            "Note: In this section a bubble popping sound will accompany the notifications to help draw your attention to the notification. The sound will originate from the dock.");

        sceneInstructions.Add("NoD_WOS Scene", "In this section you will be presented with a Dock where all the notifications will be stored. " +
            "You can think of this dock similar to how notifications are displayed on your phone when you swipe down. " +
            "You can place the dock wherever you want by moving it how you move any other objects. \n\n " +
            "There is a button next to the dock which serves three purposes:\n " +
            "1. Clicking the button allows you to show or hide the notifications.\n 2. The number on the button shows number of non-interacted notifications.\n " +
            "3. Red color represents there is a new notification and Blue color represents you have looked at all notifications (this doesn't mean you have interacted with it.)");

        sceneInstructions.Add("NoO_WS Scene", "In this section the notification will pop-up above the objects that changed state, those that prompted the notification. \n\n" +
            "Note: In this section a bubble popping sound will accompany the notifications to help draw your attention to the notification. The sound will originate from the notification itself.");

        sceneInstructions.Add("NoO_WOS Scene", "In this section the notification will pop-up above the objects that changed state, those that prompted the notification.");

        sceneInstructions.Add("Control_WS Scene", "In this section there will be no notifications. " +
            "You will have to pay attention to all stations at all times and serve the customers to the best of your ability. \n\n" +
            "Note: In this section a bubble popping sound will play to help you realize there is a state change on one of the stations. You can think of this as an audio notification. " +
            "The sound will not have a distinguishable origin point and will seem like it's playing right in your head.");

        sceneInstructions.Add("Control_WOS Scene", "In this section there will be no notifications. " +
            "You will have to pay attention to all stations at all times and serve the customers to the best of your ability.");

        sceneInstructions.Add("NoO_WA Scene", "In this section the notification will pop-up above the objects that changed state, those that prompted the notification. \n\n" +
            "Note: In this section an arrow will point to the notifications to help draw your attention to the said notification.");

        sceneInstructions.Add("NoV_WOS Scene", "In this section the notification will pop-up at the top-center position of your viewport.");

    }
    void InitializeInstructions()
    {
        instructions.Add("Welcome to study titled \"Notifications in Pervasive Augmented Reality Scenario\"");

        instructions.Add("The following screens will help you get familiarized with different elements of the environment. The textual instructions will give majority of the details. " +
            "This will be followed by a tutorial section where in place instructions will guide you through the process of interacting with the environment. \n\n" +
            "Note: If you have any questions, it is encouraged to ask them starting now till the end of tutorial section.");

        instructions.Add("Customer Station\n\nCustomers will appear at this station. Each customer will have a request for a food item. " +
            "This will be represented in a speech bubble top right of the customer. The images will display the ingredients for the food item in order. \n\n" +
            "You have a limited amount of time to present the food item to the customer or else they'll leave. The order of the food item requested will be random.");

        instructions.Add("Burger Station\n\nThis is where you will find all ingredients to make a burger. A grill is present to the right of the ingredients. The grill is where you can put the beef patty to cook it. " +
            "Beef patty has 3 stages of cooking: Uncooked, Cooked, Burnt. \n\nNote: The customers will always request for Cooked beef patty.");

        instructions.Add("Pizza Station\n\nThis is where you will find all ingredients to make a pizza. An oven is present to the left of the ingredients. " +
            "After putting all the ingredients on the pizza you put the pizza in the oven to cook it. " +
            "The pizza has 3 stages of cooking: Uncooked, Cooked, Burnt. \n\nNote: The customers will always request for a cooked pizza.");

        instructions.Add("Coffee Station\n\nThis is where you will find a coffee machine and coffee cups. " +
            "When the coffee pot is placed in the coffee machine an indicator, displaying how many cups of coffee can be filled using current level of coffee, will be displayed to top left of the coffee machine. " +
            "Make sure to remove the coffee cups from the blue spawning plate before you pour the coffee in the cup. " +
            "Once the cup is filled a cap will automatically appear to close the coffee cup. \n\nNote: The customers will always request for a full cup of coffee.");

        instructions.Add("To help you with the tasks, you will be provided with visual and auditory aid to help you with your tasks. " +
            "The visual aids are presented in a blue box with 2 pieces of information in format - Station:Status. " +
            "Where \"Station\" represents different categories of notification invoking objects (Burger, Pizza, Coffee, and Customer), and \"Status\" represents the different states of the station as mentioned before. " +
            "In some sections visual aid will be accompanied by a \"bubble popping sound\". \n\n" +
            "For each notification, interact with it by clicking on it using your finger, like a button. " +
            "This to acknowledge you have read the notification (This doesn't mean you have to interact with the object that prompted the notification.)");

        instructions.Add("In this section you will follow a tutorial to learn how to interact with the objects. " +
            "There will be instructions with an arrow pointing to the object used to complete the current task. If the instruction is not in the viewport there will be an arrow guiding you to it. " +
            "After completing each task press the right arrow to go to the next instruction. \n\n" +
            "There will be 3 customers each asking for different food item. You will have all the time you need to present them the food item they requested. " +
            "After preparing the food place it on the tray in front of the customer. If the food item is wrong, there will be a text stating it. If the food item is correct the customer will leave. \n\n" +
            "Once you are done serving all customers you can press the next button on the last instruction (located at the left of trash can) to start the actual experiment.");


        instructions.Add("Thank you for participating!");
    }

    public void SetNextInstruction(int index)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = instructions[index];
    }

    public void Continue()
    {
        if (instructionsNum < instructions.Count - 1)
            GameManager.instance.UpdateGameState(GameState.Welcome);
        else
        {
            startTasks = true;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public bool GetStartTask()
    {
        return startTasks;
    }

    public void ResetInstructionNumber()
    {
        instructionsNum = -1;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetInstructionsNumer(int instructionNumber)
    {
        instructionsNum = instructionNumber;
    }
}
