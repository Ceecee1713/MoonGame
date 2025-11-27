using UnityEngine;

public class OpenCluebookMainUI : MonoBehaviour
{
    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private GameObject craftingUI;

    [SerializeField]
    private GameObject pauseMenuUI;

    public void OpenCluebookClick()
    {
        if(craftingUI.activeSelf == true || pauseMenuUI.activeSelf == true)
            return;

        cluebookUI.SetActive(true);
    }
}
