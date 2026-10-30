using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMerger : MonoBehaviour
{

    public string ServerScene;
    public string LoggerSceneName;
    
    public TMP_InputField input_Name;
    public TMP_Text host_Button;

    public List<GameObject> ToDesac = new();

    Client client;

    bool _doIHost = false;

    public void HostOrNot()
    {
        client ??= FindAnyObjectByType<Client>();

        _doIHost = !_doIHost;

        input_Name.interactable = !_doIHost;

        ToDesac.ForEach(x => x.SetActive(!x.activeSelf));

        if(_doIHost) 
        {
            host_Button.text = "Stop Host";
            SceneManager.LoadScene(ServerScene,LoadSceneMode.Additive);
            client.ConnectToLocal();
        }
        else
        {
            host_Button.text = "Host";
            client.Disconnect();
            SceneManager.UnloadSceneAsync(ServerScene);
        } 
    }


    public void Start()
    {
        Scene loggerScene = SceneManager.GetSceneByName(LoggerSceneName);
        if (!loggerScene.isLoaded)
        {
            SceneManager.LoadScene(LoggerSceneName, LoadSceneMode.Additive);
        }
    }
}
