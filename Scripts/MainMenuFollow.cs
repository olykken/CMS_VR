using UnityEngine;

public class MainMenuFollow : MonoBehaviour {

    public GameObject attachPoint;
    public static MainMenuFollow Instance;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    void Update()
    {

        if (attachPoint == null)
        {
            attachPoint = GameObject.Find("MenuAttachPoint");
        }

        gameObject.transform.position = attachPoint.transform.position;
        gameObject.transform.rotation = attachPoint.transform.rotation;
    }
}
