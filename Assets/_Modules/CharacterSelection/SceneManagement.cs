
using UnityEngine;
using PurrNet;
using PurrNet.Modules;
using UnityEngine.SceneManagement;
public class SceneManagement : NetworkBehaviour
{
    [PurrScene] public string sceneToChange;

    [ContextMenu("Change Scene")]
    public void ChangeScene()
    {
        PurrSceneSettings settings = new()
        {
            isPublic = false,
            mode = LoadSceneMode.Single
        };
        networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestSceneChange(RPCInfo info = default)
    {
        var scene = SceneManager.GetSceneByName(sceneToChange);
        if (!scene.isLoaded)
        {
            // Load the scene if it's not already loaded
            PurrSceneSettings settings = new()
            {
                isPublic = false,
                mode = LoadSceneMode.Single
            };
            networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
            return;
        }

        if(networkManager.sceneModule.TryGetSceneID(scene, out SceneID sceneID))
        {
            networkManager.scenePlayersModule.AddPlayerToScene(info.sender, sceneID);
        }
    }

    [ContextMenu("Request Scene Change")]
    public void RequestSceneChangeClient()
    {
        RequestSceneChange();
    }
}
