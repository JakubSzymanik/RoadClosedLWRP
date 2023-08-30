using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitor : MonoBehaviour
{
    [SerializeField] private float blackoutSpeed;
    [SerializeField] private Image transitionImg;

    public static SceneTransitor instance { get; private set; }

    private bool isLoading;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        Application.targetFrameRate = 60;
    }

    public void LoadScene(int index)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadAsync(index));
            Overlord.lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Overlord.currentSceneIndex = index;
        }
    }

    public void LoadSceneInstant(int index)
    {
        Overlord.lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Overlord.currentSceneIndex = index;
        StopAllCoroutines();
        StartCoroutine(LoadAsyncInstant(index));
    }

    private IEnumerator LoadAsync(int index)
    {
        isLoading = true;
        transitionImg.enabled = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = false;
        while (transitionImg.color.a < 1 || asyncLoad.progress < 0.9f) //0.9f - loaded unactivated scene
        {
            transitionImg.color += Color.black * blackoutSpeed * Time.deltaTime;
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
        yield return null;
        while (transitionImg.color.a > 0)
        {
            transitionImg.color -= Color.black * blackoutSpeed * Time.deltaTime;
            yield return null;
        }
        transitionImg.enabled = false;
        isLoading = false;
    }

    private IEnumerator LoadAsyncInstant(int index)
    {
        isLoading = true;
        transitionImg.enabled = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = false;
        transitionImg.color = Color.black;
        while (asyncLoad.progress < 0.9f) //0.9f - loaded unactivated scene
        {
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
        yield return null;
        Shader.WarmupAllShaders();
        while (transitionImg.color.a > 0)
        {
            transitionImg.color -= Color.black * blackoutSpeed * Time.deltaTime;
            yield return null;
        }

        transitionImg.enabled = false;
        isLoading = false;
    }
}
