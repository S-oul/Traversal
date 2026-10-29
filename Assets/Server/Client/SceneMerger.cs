using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMerger : MonoBehaviour
{

    public SceneAsset ServerScene;
    
    public List<GameObject> ToDesac = new();

    Client client;

    bool flipFlop = false;

    public void HostOrNot()
    {
        client ??= FindAnyObjectByType<Client>();

        flipFlop = !flipFlop;


        ToDesac.ForEach(x => x.SetActive(!x.activeSelf));

        if(flipFlop) 
        {
            client.ConnectToLocal();
            SceneManager.LoadScene(ServerScene.name,LoadSceneMode.Additive);
        }
        else
        {
            client.Disconnect();
            SceneManager.UnloadSceneAsync(ServerScene.name);
        } 
    }
}
