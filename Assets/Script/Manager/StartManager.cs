using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    Button startButton = null;
    ObjectActiveFalse[] ActiveFalsedObj = null;

    Coroutine loadScene = null;

    private void Awake()
    {
        startButton = GameObject.Find("StartButton").GetComponent<Button>();

        loadScene = StartCoroutine(LoadDefenseScene());
    }

    void Start()
    {
        //button condition init
        startButton.interactable = false;
        ActiveFalsedObj = GameObject.FindObjectsOfType<ObjectActiveFalse>();
    }

    public void OnStartButton()
    {
        startButton.interactable = false;
        StartCoroutine(CameraMove());
    }
    IEnumerator CameraMove()
    {
        while (true)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(-3.83f, 3f, -3.83f), Time.deltaTime);
            if (Camera.main.transform.position.x > -3.85f)
            {
                for (int i = 0; i < ActiveFalsedObj.Length; i++)
                {
                    ActiveFalsedObj[i].gameObject.SetActive(true);
                }
                GameManager.INSTANCE.SCENENUM = 1;
                SceneManager.UnloadScene("1_StartScene");
            }

            yield return null;
        }
    }
    IEnumerator LoadDefenseScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("2_GardenningScene", LoadSceneMode.Additive);

        while (true)
        {
            if (operation.isDone)
            {
                SaveLoadManager.INSTANCE.InitLoad();
                startButton.interactable = true;
                PhotonManager.INSTANCE.OnIPButton();
                PhotonManager.INSTANCE.OnSRButton();

                StopCoroutine(loadScene);
                yield break;
            }

            yield return null;
        }

    }
}
