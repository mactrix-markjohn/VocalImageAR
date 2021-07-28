using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARTrackedImageManager))]
public class ARScriptGeneral : MonoBehaviour
{
    public Button BackButton;
    public Button SettingsButton;
    public ARTrackedImageManager trackedImageManager;
    public AudioSource audioSource;
    private Dictionary<string, Texture2D> pictures = new Dictionary<string, Texture2D>();
    Dictionary<string,AudioClip> audioClips = new Dictionary<string, AudioClip>();
    
    // The audio UI Gameobject
    public GameObject AudioUIContainer;
    public Text Audioname;
    //Playback Recorded Audioclip
    public Button PlayPauseButton;
    public Button StopRecord;
    public Image PlayPauseImage;
    public List<Sprite> PlayPausePictures;



    // Start is called before the first frame update
    void Start()
    {
        // set the audio playback container invisible
        AudioUIContainer.SetActive(false);
        BackButton.onClick.AddListener(() => { SceneManager.LoadScene(ConstantString.MainScene);});
        SettingsButton.onClick.AddListener(() => { SettingButtonClick(); });
        PlayPauseButton.onClick.AddListener(() => { PlayPause();});
        StopRecord.onClick.AddListener(() => { StopRecordMethod();});
        
        // Get all the Pictures and AudioClips
        GetAllTextures();
        GetAllAudioClip();
        
        
        // Populate the ImageReference Library with the Pictures in the Image Dictionary
        PopulateImageLibrary();
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SettingButtonClick()
    {
       //TODO: Implement something here
    }


    void PopulateImageLibrary()
    {
        foreach ( var pic in pictures)
        {
            StartCoroutine(AddImageJob(pic.Value, pic.Key));
        }
    }

    void GetAllTextures()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.jpg");

        if (info.Length <= 0)
        {
            _ShowAndroidToastMessage("There is no Image yet.");
        }
        else
        {
            
            foreach (FileInfo f in info)
            {


                string filename = f.Name.Substring(0,f.Name.LastIndexOf(".", StringComparison.Ordinal)); 
                string path = f.ToString();

                Texture2D texture = NativeCamera.LoadImageAtPath(path, 512);
                
                Texture2D readableTexture = duplicateTexture(texture);
                
                //Adding the Texture from Path to the Dictionary
                pictures.Add(filename,readableTexture);
                

            }
            
            
            
        }
    }

    void GetAllAudioClip()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] info = dir.GetFiles("*.wav");

        if (info.Length <= 0)
        {
            _ShowAndroidToastMessage("There is no audio recording yet.");
        }
        else
        {
            
            foreach (FileInfo f in info)
            {


                string filename = f.Name.Substring(0,f.Name.LastIndexOf(".", StringComparison.Ordinal)); 
                string path = f.ToString();
                StartCoroutine(LoadAudio(filename, path));



            }
            
        }
    }
    
    IEnumerator LoadAudio(string filename, string FullPath)
    {
        //string FullPath = "C:/AudioFiles/audio1.wav";
        FullPath = "file:///" + FullPath;
        WWW URL = new WWW(FullPath);
        yield return URL;

        AudioClip clip = URL.GetAudioClip(false, true);
        audioClips.Add(filename,clip);
        
    }
    
    


    public IEnumerator AddImageJob(Texture2D texture2D, string texturename)
    {
        yield return null;

        var firstGuid = new SerializableGuid(0,0);
        var secondGuid = new SerializableGuid(0,0);
        
        XRReferenceImage newImage = new XRReferenceImage(firstGuid, secondGuid, new Vector2(0.1f,0.1f), texturename,texture2D );

        try
        {
            
            _ShowAndroidToastMessage(newImage.ToString());

            if (trackedImageManager.subsystem.subsystemDescriptor.supportsMutableLibrary)
            {
                MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

                var jobHandle =
                    mutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(texture2D,texturename, 0.1f);


                while (!jobHandle.jobHandle.IsCompleted)
                {
                    string status = "Job is Running...";
                }
                
                

            }

            

        }
        catch (Exception e)
        {
            string error = e.ToString();
        }

    }


    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;

    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
           // UpdateImage(trackedImage);
           
        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {

            /*if (audioSource.isPlaying)
            {
                audioSource.Stop();
                AudioUIContainer.SetActive(false);
            }*/
            
        }
        
        
        
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        //Get the name of the tracked image
        string trackedname = trackedImage.referenceImage.name;
        
        // locate the audio clip with that name
        if (audioClips.ContainsKey(trackedname))
        {
            AudioClip clip = audioClips[trackedname];
            
            // Add to the AudioSource
            audioSource.clip = clip;
            Audioname.text = trackedname + ".wav";
            AudioUIContainer.SetActive(true);
            audioSource.Play();
            isPlaying = true;
            PlayPauseImage.sprite = PlayPausePictures[1];
        }
        else
        {
            _ShowAndroidToastMessage("There is no matching audio for this Image");
        }



        /*Vector3 position = trackedImage.transform.position;

        GameObject prefab = spawnedPrefabs[name];
        prefab.transform.position = position;
        prefab.SetActive(true);*/

        /*foreach (GameObject go in spawnedPrefabs.Values)
        {
            if (go.name != name)
            {
                go.SetActive(false);
            }
        }*/
    }
    
    
    private bool isPlaying = false;
    void PlayPause()
    {
       
            if (!isPlaying)
            {
            
                audioSource.Play();
                PlayPauseImage.sprite = PlayPausePictures[1];
                isPlaying = true;
            }
            else
            {

                audioSource.Pause();
                PlayPauseImage.sprite = PlayPausePictures[0];
                isPlaying = false;
            }
        
       
        
    }

    void StopRecordMethod()
    {

        if (isPlaying)
        {
            audioSource.Stop();
            PlayPauseImage.sprite = PlayPausePictures[0];
            isPlaying = false;
            AudioUIContainer.SetActive(false);
        }
    }


    private Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height,TextureFormat.RGBA32,false);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }


    /*#region Implement AugmentedImage

    [SerializeField] private GameObject[] placeablePreefab;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    


    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        foreach (GameObject prefab in placeablePreefab)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    

    #endregion*/



}
