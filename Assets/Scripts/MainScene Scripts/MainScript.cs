using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    public Button AddPicture;
    public Button ARScene;
    
    
    // Start is called before the first frame update
    void Start()
    {
      AddPicture.onClick.AddListener(() => { SceneManager.LoadScene(ConstantString.PictureScene); });
      ARScene.onClick.AddListener(() => { SceneManager.LoadScene(ConstantString.ARScene); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
