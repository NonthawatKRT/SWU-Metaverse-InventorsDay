
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
        // This will automatically move all players to the new scene
        networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestSceneChange(RPCInfo info = default)
    {
        // Simply load the scene - PurrNet should handle moving all players
        PurrSceneSettings settings = new()
        {
            isPublic = false,
            mode = LoadSceneMode.Single
        };
        networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
    }

    [ContextMenu("Request Scene Change")]
    public void RequestSceneChangeClient()
    {
        RequestSceneChange();
    }
}
