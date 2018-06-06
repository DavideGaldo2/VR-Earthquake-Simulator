// @Copyright Davide Galdo
// @License GPLv3
// @Virtual Reality Earthquake Simulator Character Position Controller Kinect Interface



using UnityEngine;
using System.Collections;

public class CharacterPositionController : MonoBehaviour {

    public float multFactor = 1f;
    private GameController gameController;


    //Variabili di movimento del character
    private Vector3 targetPosition = new Vector3(0, 0, 0);
    private Vector3 currentPosition = new Vector3(0, 0, 0);
    private Vector3 movingVector = new Vector3(0, 0, 0);
    CharacterController controller;

    // Use this for initialization
    void Awake () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //Posizionamento della camera
        currentPosition.x = gameController.userPosition.x;
        currentPosition.y = gameController.userPosition.y;
        currentPosition.z= -gameController.userPosition.z;
        controller = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {

        //Blocco con la Controllo character controller
        //Aggiorno la posizione di destinazione
        targetPosition.x = gameController.userPosition.x;
        targetPosition.y = gameController.userPosition.y;
        targetPosition.z= -gameController.userPosition.z;        
        if (currentPosition!=targetPosition) 
        {
            //Calcolo il vettore di movimento
            movingVector= targetPosition - currentPosition;
            //La posizione di destinazione diventa la posizione attuale
            currentPosition = targetPosition * multFactor;
            //Effettuo la traslazione
            gameObject.transform.position = targetPosition * multFactor;
            
        }
        
	} 
}
