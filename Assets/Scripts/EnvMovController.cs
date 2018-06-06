// @Copyright Davide Galdo
// @License GPLv3
// @Sinusoidal Environmental movement to simulate an earthquake


using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvMovController : MonoBehaviour {

    //Amount and Speed of Demo Shaking
    float normalAmplitude = 0.3f; 
    float speed = 5.0f;
    float startingTime;
    float totalDuration;
    float peakTime;
    float peakDuration;
    float peakAmplitude;
    float zPeakAmplitude= 0.2f;
    int numberOfSamples = 0;

    private float lowIntensitySoundValueStart = 0.01f;
    private float highIntensitySoundStart = 0.035f;

    float timePassed=0f; //Time passed from the starting of the shacking;

    //Flag used so start the demo shaking
    bool automaticMovement=false;

    /// <summary>
    /// The environment that has to be moved starting from the acceleration value
    /// </summary>
    public GameObject environment;

    public GameObject soundControllerGameObject;    //Sound controller game object

    private SoundController soundController;        //Actual sound controller

    void Awake()
    {
        this.soundController = this.soundControllerGameObject.GetComponent<SoundController>();
        numberOfSamples = 0;
    }

    public void MoveEnvironment(Vector3 displacement)
    {
        numberOfSamples++;
        Debug.LogWarning("numberOfSamples of samples: " + numberOfSamples);
        Vector3 newPosition;
        //newPosition.x = 0.5f * accelerations.x * Time.deltaTime;
        //newPosition.y = 0.5f * accelerations.y * Time.deltaTime;
        //newPosition.z = 0.5f * accelerations.z * Time.deltaTime;

        newPosition.x = (displacement.x) ;
        //newPosition.y = (displacement.y) ;

        //Modifica per gestire l'oscillazione verticale che attualmente è nulla
        newPosition.y = (displacement.z) ;
        newPosition.z = (displacement.z) ;
        Debug.Log("X:" + newPosition.x + " Y: " + newPosition.y + " Z: "+newPosition.z);
                       
        environment.transform.localPosition = newPosition;



        //If the sounds are not playing then start them
        if (!soundController.lowIntensityisPlaying && (Mathf.Abs(newPosition.x) > lowIntensitySoundValueStart || Mathf.Abs(newPosition.z)> lowIntensitySoundValueStart))
        {
            soundController.StartLowIntensitySounds();
        }
        
        if (!soundController.highIntensityisPlaying && (Mathf.Abs(newPosition.x) > highIntensitySoundStart || Mathf.Abs(newPosition.z) > highIntensitySoundStart))
        {
                soundController.StartHighIntensitySounds();
                Debug.LogWarning("High Intensity Sound Played!");
        }   
                 
     }


    /// <summary>
    /// This method is used to start a random movement of the environment
    /// </summary>
    public void StartAutomaticMovement(float startingT, float totalDur, float peakT, float peakDur, float peakAmp)
    {
        this.startingTime = startingT;
        this.totalDuration =  totalDur;
        this.peakTime = peakT;
        this.peakDuration = peakDur;
        this.peakAmplitude = peakAmp;
        this.automaticMovement = true;
        Debug.Log("Start Automatic Movement called!");
    }

    /// <summary>
    /// This methos is used to stop the random movement of the environment
    /// </summary>
    public void StopAutomaticMovement()
    {
        //Stop the movements
        this.automaticMovement = false;
        
        //Stop the sounds
        if (this.soundController.lowIntensityisPlaying)
            this.soundController.StopLowIntensitySounds();

        if (this.soundController.highIntensityisPlaying)
            this.soundController.StopHighIntensitySounds();

        this.soundController.StopScream();

        //Reset the experiment timer
        this.timePassed = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (automaticMovement)
        {            
            if (timePassed >= startingTime) //Shacking in corso
            {
                //Zona di bassa intensità
                if (timePassed < (startingTime + peakTime) || timePassed > (startingTime + peakTime + peakDuration))
                {
                    //If the sounds are not playing then start them
                    if (!soundController.lowIntensityisPlaying)
                    {
                        soundController.StartLowIntensitySounds();
                    }
                    //Stop the high intensity sounds in they are playing
                    if(soundController.highIntensityisPlaying)
                    {
                        soundController.StopHighIntensitySounds();                    
                    }
                    //Move the environment
                    if (environment != null){
                        environment.transform.Translate(Random.Range(-normalAmplitude, normalAmplitude) * Mathf.Sin(Time.deltaTime * speed),
                        Random.Range(-normalAmplitude, normalAmplitude) * Mathf.Sin(Time.deltaTime * speed),
                        Random.Range(-normalAmplitude, normalAmplitude) * Mathf.Sin(Time.deltaTime * speed));
                    }
                    
                }
                else
                //High intensity zone
                {
                    //Check the sound playing
                    if (!soundController.highIntensityisPlaying)
                    {
                        soundController.StartHighIntensitySounds();
                    }
                    //Move the environment
                    if (environment != null)
                    {
                        environment.transform.Translate(Random.Range(-peakAmplitude, peakAmplitude) * Mathf.Sin(Time.deltaTime * speed),
                        Random.Range(-peakAmplitude, peakAmplitude) * Mathf.Sin(Time.deltaTime * speed),
                        Random.Range(-zPeakAmplitude, zPeakAmplitude) * Mathf.Sin(Time.deltaTime * speed));
                    }
                    
                }
            }
            Debug.Log("Tempo trascorso: " + timePassed.ToString());
            timePassed += Time.deltaTime * 1f;

            if (timePassed > startingTime + totalDuration)
            {                
                this.StopAutomaticMovement();
            }
                
        }            
	}
}
