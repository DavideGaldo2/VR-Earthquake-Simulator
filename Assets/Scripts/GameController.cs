// @Copyright Davide Galdo
// @License GPLv3
// @Virtual Reality Earthquake Simulator GameController

using UnityEngine;
using System.Collections;
//using NetworkObj;
//using NetworkLibraryMessages;
using QuakeSimulatorNetworkLibrary;
using System.Globalization;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    
    //GameController singleton instance
    public static GameController instance;

    public Canvas UiCanvas;
    
    /// <summary>
    /// Network client used to communicate with Kinect Server 
    /// </summary>
    static private NetworkObject quakeServerNetObj;
    
    /// <summary>
    /// Network client used to comunicate with environment Acceleration Sensor
    /// </summary>
    //static private NetworkObject accNetObj;
    
    /// <summary>
    /// Environment accelerations given by accelerometer sensor
    /// </summary>
    public Vector3 envAccelerations;

    /// <summary>
    /// User position given by kinect sensor
    /// </summary>
    public Vector3 userPosition;

    /// <summary>
    /// Environmental Controller
    /// </summary>
    public EnvMovController envController;

    private string quakeServerReceivedMex=string.Empty;
    public string quakeServerIPAddress = string.Empty;
    public string quakeServerTcpPort = string.Empty;

    //private string accelerometerReceivedMex = string.Empty;
    //public string accelerometerServerIPAddress = string.Empty;
    //public string accelerometerServerTcpPort = string.Empty;

    private bool loadIntro = false;
    private bool loadIndoor = false;
    private bool loadOutdoor = false;
    //private float timePassed = 0f;
    //private bool indoorSceneLoaded = false;

    //Position of the messages in the received network messages
    private int payloadIndex = 1;
    private int messageIndex = 2;
    private int userPositionXIndex = 2;
    private int userPositionYIndex = 3;
    private int userPositionZIndex = 4;
    private int groundPositionXIndex = 5;
    private int groundPositionYIndex = 6;
    private int groundPositionZIndex = 7;





    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loading environmental controller");
        envController = GameObject.FindGameObjectWithTag("EnvMovController").GetComponent<EnvMovController>();
        UiCanvas= GameObject.FindGameObjectWithTag("UiCanvas").GetComponent<Canvas>();
        if (string.Compare( scene.name,"Intro")!=0) //Livello caricato diverso da intro
        {
            UiCanvas.enabled = false;
        }
        
        loadIntro = false;
        loadIndoor = false;
        loadOutdoor = false;
}

    void Awake()
    {
        //implemento il singleton del game manager
        if (instance == null)
        {
            instance = this;
            ConnectQuakeServer();
            //ConnectAccelerometer();
        }
            
        else if (instance != this)
            DestroyObject(gameObject);
        //Evito che il game object venga distrutto al cambio scena
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {        
        if (quakeServerNetObj.remoteIsConnected)
        {
            quakeServerNetObj.SendMessage(NetMessages.readyToReceive);
        }      
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //timePassed += Time.deltaTime;
        //if (timePassed > 35 && !indoorSceneLoaded)
        //{
        //    indoorSceneLoaded = true;
        //    loadIndoor = true;
        //}

        if (loadIntro)
        {
            SceneManager.LoadScene("Intro");
        }else if (loadIndoor)
        {
            SceneManager.LoadScene("Indoor");            
        }else if (loadOutdoor)
        {
            SceneManager.LoadScene("Outdoor");
        }
        
        //Kinect data raceived
        string[] receivedDataArray;
        string kinectDato1 = string.Empty;
        string kinectDato2 = string.Empty;
        string kinectDato3 = string.Empty;

        //Accelerometer data received
        string accelerometerDataX = string.Empty;
        string accelerometerDataY = string.Empty;
        string accelerometerDataZ = string.Empty;

        //Check received data 
        if (quakeServerNetObj != null && quakeServerNetObj.remoteIsConnected)
        {            
            receivedDataArray = quakeServerNetObj.receivedMex.Split((char)124);            
            if (receivedDataArray.Length >= 2)
            {                
                if (receivedDataArray[payloadIndex] == NetMessages.dataPayload)
                {
                    kinectDato1 = receivedDataArray[userPositionXIndex];
                    kinectDato2 = receivedDataArray[userPositionYIndex];
                    kinectDato3 = receivedDataArray[userPositionZIndex];
                    kinectDato1 = kinectDato1.Replace(",", ".");
                    kinectDato2 = kinectDato2.Replace(",", ".");
                    kinectDato3 = kinectDato3.Replace(",", ".");
                    //userPosition.x = float.Parse(kinectDato1, CultureInfo.InvariantCulture);
                    //userPosition.y = float.Parse(kinectDato2, CultureInfo.InvariantCulture);
                    //userPosition.z = float.Parse(kinectDato3, CultureInfo.InvariantCulture);
                    userPosition.x = float.Parse(kinectDato1);
                    userPosition.y = float.Parse(kinectDato2);
                    userPosition.z = float.Parse(kinectDato3);



                    accelerometerDataX = receivedDataArray[groundPositionXIndex];
                    accelerometerDataY = receivedDataArray[groundPositionYIndex];
                    accelerometerDataZ = receivedDataArray[groundPositionZIndex];

                    accelerometerDataX = accelerometerDataX.Replace(",", ".");
                    accelerometerDataY = accelerometerDataY.Replace(",", ".");
                    accelerometerDataZ = accelerometerDataZ.Replace(",", ".");
                    //envAccelerations.x = float.Parse(accelerometerDataX, CultureInfo.InvariantCulture);
                    //envAccelerations.y = float.Parse(accelerometerDataY, CultureInfo.InvariantCulture);
                    //envAccelerations.z = float.Parse(accelerometerDataZ, CultureInfo.InvariantCulture);

                    envAccelerations.x = float.Parse(accelerometerDataX);
                    envAccelerations.y = float.Parse(accelerometerDataY);
                    envAccelerations.z = float.Parse(accelerometerDataZ);
                    envController.MoveEnvironment(envAccelerations);
                }

            }            
        }


        ////Controllo dei dati accelerometro
        //if (accNetObj != null && accNetObj.remoteIsConnected)
        //{
        //    accelerometerDataArray = accNetObj.receivedMex.Split((char)124);
        //    if (accelerometerDataArray.Length >= 2)
        //    {
        //        if (accelerometerDataArray[0] == NetMessages.accelerationHeader)
        //        {
        //            accelerometerDato1 = accelerometerDataArray[1];
        //            accelerometerDataY = accelerometerDataArray[2];
        //            accelerometerDataZ = accelerometerDataArray[3];
        //            envAccelerations.x = float.Parse(accelerometerDato1, CultureInfo.InvariantCulture);
        //            envAccelerations.y = float.Parse(accelerometerDataY, CultureInfo.InvariantCulture);
        //            envAccelerations.z = float.Parse(accelerometerDataZ, CultureInfo.InvariantCulture);
        //            envController.MoveEnvironment(envAccelerations);
        //        }
        //    }
        //}
    }

    public void OnApplicationQuit()
    {
        if (quakeServerNetObj != null && quakeServerNetObj.remoteIsConnected)
        {
            quakeServerNetObj.SendMessage(NetMessages.disconnectRequest);
            System.Threading.Thread.Sleep(500);
        }
        //if (accNetObj !=null && accNetObj.remoteIsConnected)
        //{
        //    accNetObj.SendMessage(NetworkLibraryMessages.NetMessages.disconnectRequest);
        //    System.Threading.Thread.Sleep(500);
        //}
    }

    public void ConnectQuakeServer()
    {        
        //Salvo l'ndirizzo IP del server
        Debug.Log("Indirizzo IP: " + quakeServerIPAddress);
        int port = int.Parse(quakeServerTcpPort);
        quakeServerNetObj = new NetworkObject(quakeServerIPAddress, port);
        System.Threading.Thread.Sleep(500);
        //quakeServerNetObj.OpenTcpConnection();
        quakeServerNetObj.connectionStateChanged += QuakeServerNetClient_ConnectionStateChanged;
        quakeServerNetObj.messageReceived += QuakeServerNetClient_MessageReceived;
        
    }

    //public void ConnectAccelerometer()
    //{
    //    //Salvo l'ndirizzo IP del server
    //    Debug.Log("Indirizzo IP server Accelerometro: " + accelerometerServerIPAddress);
    //    int port = int.Parse(accelerometerServerTcpPort);
    //    accNetObj = new NetworkObject(accelerometerServerIPAddress, port);
    //    System.Threading.Thread.Sleep(500);
    //    accNetObj.OpenTcpConnection();
    //    accNetObj.connectionStateChanged += accNetClient_connectionStateChanged;
    //    //accNetObj.messageReceived += netClient_messageReceived;
    //}




    //void accNetClient_connectionStateChanged(object source, System.EventArgs e)
    //{
    //    Debug.Log("Stato connessione accelerogramma cambiato");

    //}
    //Gestore di evento messaggi ricevuti
    void QuakeServerNetClient_MessageReceived(object source, System.EventArgs e)
    {
        //Analisi messaggio ricevuto

        if (quakeServerNetObj != null && quakeServerNetObj.remoteIsConnected)
        {
            string[] dataArray;
            dataArray = quakeServerNetObj.receivedMex.Split((char)124);
            if (dataArray.Length >= 2)
            {
                if (dataArray[payloadIndex] == NetMessages.messagePayload)
                {
                    if (dataArray[messageIndex] == NetMessages.startSignal)
                    {
                        //Reperire le informazioni passate dal server e passarle al controller di movimento ambientale
                        string startingTimeStr = dataArray[3];
                        string totalDurationStr = dataArray[4];
                        string peakTimeStr = dataArray[5];
                        string peakDurationStr = dataArray[6];
                        string peakAmplitudeStr = dataArray[7];
                        float startingTime = float.Parse(startingTimeStr, CultureInfo.InvariantCulture);
                        float totalDuration = float.Parse(totalDurationStr, CultureInfo.InvariantCulture);
                        float peakTime = float.Parse(peakTimeStr, CultureInfo.InvariantCulture);
                        float peakDuration = float.Parse(peakDurationStr, CultureInfo.InvariantCulture);
                        float peakAmplitude = float.Parse(peakAmplitudeStr, CultureInfo.InvariantCulture);
                        //Passaggio parametri a controller movimento ambientale
                        envController.StartAutomaticMovement(startingTime, totalDuration, peakTime, peakDuration, peakAmplitude);                                                
                    }
                    else if (dataArray[messageIndex] == NetMessages.stopSignal)
                    {
                        Debug.Log("Stop Signal");
                        envController.StopAutomaticMovement();

                    }
                    else if (dataArray[messageIndex] == NetMessages.disconnectRequest)
                    {
                        Debug.Log("Disconnection request");

                    }
                    else if (dataArray[messageIndex] == NetMessages.loadIndoor)
                    {
                        //Caricamento Livello indoor
                        
                        Debug.Log("Carico Scena Indoor");
                        IndoorScene();
                    }
                    else if (dataArray[messageIndex] == NetMessages.loadOutdoor)
                    {
                        //Caricamento Livello indoor
                        Debug.Log("Carico Scena Outdoor");
                        OutdoorScene();
                        
                    }
                    else if (dataArray[messageIndex] == NetMessages.loadIntro)
                    {
                        //Caricamento Livello Intro
                        Debug.Log("Carico Intro");
                        IntroScene();                        
                    }
                }
            }
        }
    }

    private void QuakeServerNetClient_ConnectionStateChanged(object source, System.EventArgs e)
    {
        //Da implementare
        //int i = 0;
        
    }

    /*
    public void StartGame()
    {
        //Object.DontDestroyOnLoad(gameManager);
        kinectNetObj.SendMessage(NetMessages.readyToReceive);
        accNetObj.SendMessage(NetMessages.readyToReceive);
    }
    */
    
    public void QuitGame()
    {
        Application.Quit();
    } 

    public void IntroScene()
    {
        loadIntro = true;
        loadIndoor = false;
        loadOutdoor = false;
    }

    public void IndoorScene()
    {
        loadIntro = false;
        loadIndoor = true;
        loadOutdoor = false;
    }

    public void OutdoorScene()
    {
        loadIntro = false;
        loadIndoor = false;
        loadOutdoor = true;
        

    }

    
    
    

}