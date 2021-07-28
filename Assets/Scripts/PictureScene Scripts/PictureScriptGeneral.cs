using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PictureScriptGeneral : MonoBehaviour
{
    public Button BackButton;
    public Button CameraButton;
    public Button GalleryButton;
    public Button AudioButton;
    public RawImage rawImage;
    public GameObject NoImageText;
    public Image PreviewImage;
    public Text Imagename;
    public GameObject AudioRecPanel;
    
    // Buttons from Audio Recording Panel
    public Button CancelRecPanel;
    public Button AudioRec;
    public Button StopRec;
    public Button SaveRec;
   

    // AudioSource
    public AudioSource audioSource;
    public Text Audioname;
    

    //Playback Recorded Audioclip
    public Button PlayPauseButton;
    public Image PlayPauseImage;

    public List<Sprite> PlayPausePictures;
    
    
    //Constraint variables
    private bool PictureChecker = false;
    

    // Start is called before the first frame update
    void Start()
    {
        
        rawImage.gameObject.SetActive(false);
        AudioRecPanel.SetActive(false);

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            //Permission.RequestUserPermission(Permission.Microphone);
            Permission.RequestUserPermissions(new []{Permission.Microphone,Permission.ExternalStorageWrite,Permission.ExternalStorageRead,Permission.Camera});
        }
        



        /*if (NativeCamera.CheckPermission() != NativeCamera.Permission.Granted)
        {
           // NativeCamera.RequestPermission();
        }else if (NativeCamera.CheckPermission() == NativeCamera.Permission.ShouldAsk)
        {
            //NativeCamera.RequestPermission();
        }*/


        BackButton.onClick.AddListener(() => { SceneManager.LoadScene(ConstantString.MainScene);});
        CameraButton.onClick.AddListener(() => { CameraButtonClick();});
        GalleryButton.onClick.AddListener(() => { GalleryButtonClick();});
        AudioButton.onClick.AddListener(() => { AudioButtonClick(); });
        
        //Implement onClick of the Audio recording button
        CancelRecPanel.onClick.AddListener(() => { AudioRecPanel.SetActive(false); });
        AudioRec.onClick.AddListener(() => {AudioRecord(); });
        StopRec.onClick.AddListener(() => { AudioRecordStop();});
        SaveRec.onClick.AddListener(() => {SaveRecord(); });
        
        // Implement Audio Playback Play and Pause
        PlayPauseButton.onClick.AddListener(() => {PlayPause(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #region Button click methods

    void CameraButtonClick()
    {
        //TODO: Implement something here 
        PictureChecker = true;
        TakePicture(512);
    }


    void GalleryButtonClick()
    {
        //TODO: Implement something here 
        PictureChecker = true;
        //RetrieveImage();
    }

    void AudioButtonClick()
    {
        //TODO: Implement something here

        if (PictureChecker)
        {
            // A picture is selected
            AudioRecPanel.SetActive(true);

        }
        else
        {
            // No picture is selected
            _ShowAndroidToastMessage("Please Select an Image before you continue");
        }

        
    }
    
    void AudioRecord()
    {
        //TODO: Implement Audio recording here
        StartAudioRecording();
    }

    void AudioRecordStop()
    {
        //TODO: Implement Stoping of Audio recording here
        StopAudioRecording();
    }


    void SaveRecord()
    {
        //TODO: Save both the Picture and Audio to the same location with the same name
        
        SaveRecordingAndFiles();
    }


    private bool isPlaying = false;
    void PlayPause()
    {
        if (onceRecorded)
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
        else
        {
            _ShowAndroidToastMessage("There is no Recorded AudioClip present. Click on the Record button to record an audio");
        }

        
    }

    #endregion




    #region Picture Methods

    private void TakePicture( int maxSize )
    {

        NativeCamera.Permission permission = NativeCamera.TakePicture( ( path ) =>
        {
            Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath( path, maxSize );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }
                filename = $"vocalimage{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";



                Sprite sprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                NoImageText.SetActive(false);
                
                Imagename.text = filename+".jpg";
                
                PreviewImage.sprite = sprite;
                PreviewImage.gameObject.SetActive(true);
                    
                
                //rawImage.texture = texture;
                //rawImage.gameObject.SetActive(true);

               
               // SaveSpriteAsTexture(sprite);
               
               
               //Texture2D photo = new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
               // photo.SetPixels(texture.GetPixels());
              // photo.Apply();

              
              /*_ShowAndroidToastMessage("NativeTool");
               NativeToolkit.SaveImage(sprite.texture,filename,"png");*/
              
               
               



            }
        }, maxSize );

        Debug.Log( "Permission result : " + permission );
        
        
        
    }
    
   /*public void RetrieveImage()
    {

        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read) != NativeGallery.Permission.Granted)
        {
            NativeGallery.Permission permission =  NativeGallery.RequestPermission(NativeGallery.PermissionType.Read);
            
            
            
        }
        //else
        //{
        NativeGallery.Permission permissionr =
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (path != null)
                {
                    
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);

                    if (texture == null)
                    {
                       
                        return;
                    }

                    rawImage.texture = texture;
                    
                    rawImage.gameObject.SetActive(true);


                    /*StartCoroutine(StoreImages(new Item(texture, path),
                        path.Substring(path.Length - 10, path.Length)));*/




              //  }


          //  }, "Select an Image", "image/*");
        //}



    //}*/
    


   
    

    #endregion

    #region Audio Method

    private bool isRecording = false;
    private bool onceRecorded = false;
    private void StartAudioRecording()
    {
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 20, 44100);
        audioSource.Play();

        isRecording = true;
        onceRecorded = true;

        Audioname.text = filename + ".wav";
        
        _ShowAndroidToastMessage("Audio recording starts");
    }

    private void StopAudioRecording()
    {
        Microphone.End(Microphone.devices[0]);
        isRecording = false;
        
        _ShowAndroidToastMessage("Audio recording stoped");
    }

   
    
    
    

    #endregion

    #region Saving Files

    public string filename;
    
    private void SaveRecordingAndFiles()
    {

        // _ShowAndroidToastMessage(filepath);

       Texture2D readableTexture = duplicateTexture(PreviewImage.sprite.texture);
       NativeToolkit.SaveImage(readableTexture,filename,"jpg");
       




       //SaveImage(PreviewImage.sprite.texture, filepath);
       // SaveSpriteAsTexture(PreviewImage.sprite);
       //SaveAudio(audioSource.clip,filepath);


    }
    
    
    /*private void SaveImage(Texture2D texture, String filepath)
    {
        
        _ShowAndroidToastMessage(texture.ToString());


        byte[] bytes = texture.EncodeToPNG();

        _ShowAndroidToastMessage(""+bytes.Length);
        System.IO.File.WriteAllBytes(filepath + ".png", bytes);
        _ShowAndroidToastMessage(bytes.Length / 1024 + "Kb was saved as: " + filepath);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }*/
    

    private void SaveAudio(AudioClip audioClip,String filepath)
    {
        //EncodeMP3.convert(audioClip,filepath,128);
        SavWav.Save(filename, audioClip);
    }



    public void SaveTexture(Texture2D texture2D)
    {
        //this._texturePreview = texture2D;

        byte[] bytes = texture2D.EncodeToPNG();

        string directoryPath = Application.persistentDataPath + "/Picaudio";
        
        _ShowAndroidToastMessage(directoryPath);
        

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filepath = directoryPath + filename + ".png";

        File.WriteAllBytes(filepath, bytes);

        _ShowAndroidToastMessage(bytes.Length / 1024 + "Kb was saved as: " + directoryPath);
        
    }
    
    
    public void SaveSpriteAsTexture(Sprite sprite)
    {
        /*if (sprite != null)
        {
            _ShowAndroidToastMessage("Sprite not null");
        }
        else
        {
            _ShowAndroidToastMessage("Sprite is null");
        }*/

        Texture2D texture = new Texture2D(
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height,TextureFormat.RGBA32,false);

        /*Color[] pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height
        );*/

        texture.SetPixels(sprite.texture.GetPixels());
        texture.Apply();
        
        _ShowAndroidToastMessage(sprite.texture.ToString());

        // NativeToolkit.SaveImage(texture,filename,"png");
        //SaveTexture(texture);
        //SaveImageToGallery(texture, _ShowAndroidToastMessage);
    }




    #endregion
    
    
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
    AndroidJavaClass Androidjavaclass = null;
    
    public void SaveImageToGallery(Texture2D texture2D, System.Action<string> _callbackImagePath)
    {
        Androidjavaclass = new AndroidJavaClass("com.codemaster.android.ActivityControl");
        Texture2D screenImage = texture2D;
        byte[] imageBytes = screenImage.EncodeToPNG();
        object[] objects = { imageBytes };
        string pathofimage = Androidjavaclass.CallStatic<string>("saveImageToGallery", objects);
        _callbackImagePath(pathofimage);
        Debug.Log("==Save image path==" + pathofimage);
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
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    
    
    private void OnEnable()
    {
        NativeToolkit.OnImageSaved += ImageSaved;
        
    }

    private void OnDisable()
    {
        NativeToolkit.OnImageSaved -= ImageSaved;
    }
    
    
    void ImageSaved(string path)
    {
        //console.text += "\n" + texture.name + " saved to: " + path;
        
        string message = $"The picture is saved to {path}";
        
        _ShowAndroidToastMessage(message);
        //NativeToolkit.ScheduleLocalNotification("Hello there", message, 1, 0, "sound_notification", true, "ic_notification", "ic_notification_large");
        
       
        
        //string dirPath = Application.persistentDataPath + "/Picaudio";
        string dirPath = Application.persistentDataPath;
        
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        
        
        //_ShowAndroidToastMessage(filename);

        string filepath = dirPath +"/"+ filename;
        
        SaveAudio(audioSource.clip,filepath);

    }



}
