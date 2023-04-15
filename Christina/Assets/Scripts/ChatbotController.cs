using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the UI namespace
// If you're using TextMeshPro, add the following line:
using TMPro;
using System.Text;
using UnityEngine.Networking; // Import the Networking namespace for API requests

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ApiResponse
{
    public List<ResponseChoice> choices;
}

[System.Serializable]
public class ResponseChoice
{
    public string text;
}


public class ChatbotController : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TMP_Text chatbotResponseText;
    // Reference to the Text component for displaying chatbot responses
    // If using TextMeshPro, replace 'Text' with 'TextMeshProUGUI'
    public Button sendButton; // Reference to the Send Button

    // Start is called before the first frame update
    void Start()
    {
        sendButton.onClick.AddListener(HandleUserInput);
    }

    // send user input to chatgpt api, and clear the field
    void HandleUserInput()
    {
        string userInput = userInputField.text;
        userInputField.text = "";
        StartCoroutine(SendToChatGPT(userInput));
    }

    // send input to chatgpt

    private IEnumerator SendToChatGPT(string userInput)
    {
        // Add the code to send the request to the ChatGPT API
        string apiKey = "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"; // Replace this with your actual API key
        string apiUrl = "https://api.openai.com/v1/completions";

        // Set up the API request headers
        UnityWebRequest request = new UnityWebRequest(apiUrl, UnityWebRequest.kHttpVerbPOST);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Create message objects with the system message and the user's input
        Debug.Log("userInput" + userInput); // Add this line
                                            // Create a JSON object with the desired format
        Dictionary<string, object> jsonObject = new Dictionary<string, object>();
        jsonObject.Add("model", "gpt-3.5-turbo");

        // Create a list of messages
        List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();
        Dictionary<string, string> message = new Dictionary<string, string>();
        message.Add("role", "user");
        message.Add("content", "Say this is a test!");
        messages.Add(message);

        // Add the messages and temperature to the JSON object
        jsonObject.Add("messages", messages);
        jsonObject.Add("temperature", 0.7f);

        // Convert the JSON object to a string
        string jsonString = JsonUtility.ToJson(jsonObject).ToString();

        // Log the JSON string
        Debug.Log(jsonString);

        Debug.Log("Response body: " + jsonObject.ToString()); // Add this line

        // Prepare the data for the request body
        //
        string jsonRequestBody = $"{{\"model\":\"gpt-3.5-turbo\",\"messages\":{jsonObject},\"max_tokens\":50,\"temperature\":0.7,}}";
        byte[] requestBody = Encoding.UTF8.GetBytes(jsonRequestBody);

        // Set the request body and send the request
        request.uploadHandler = new UploadHandlerRaw(requestBody);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Wait for the request to complete
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending message to ChatGPT: " + request.error);
            Debug.LogError("Response body: " + request.downloadHandler.text); // Add this line
            yield break;
        }

        // Extract the chatbot's response from the API response
        var responseJson = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
        string chatbotResponse = responseJson.choices[0].text.Trim();

        // Display the chatbot's response in the chatbotResponseText component
        chatbotResponseText.text = chatbotResponse;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SendToChatGPT(userInputField.text));
        }
    }

}