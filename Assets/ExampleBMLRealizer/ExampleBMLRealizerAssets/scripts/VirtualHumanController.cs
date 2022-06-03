using UnityEngine;
using System.Collections;
using AssetPackage;
using BMLNet;
using SpeechLib;

public class VirtualHumanController : MonoBehaviour {

    SpVoice voice;

    RageBMLNet bmlNet = new RageBMLNet();

	// Use this for initialization
	void Start () {
        // windows text to speech
        voice = new SpVoice();
        voice.Volume = 100;
        voice.Rate = 0;

        // try to parse XML
        bmlNet.ParseFromFile("Assets/ExampleBMLRealizer/ExampleBMLRealizerAssets/BML2.xml");
        // or ParseFromString(string xml)

        // add callback for BMLNet
        bmlNet.OnSyncPointCompleted += SyncPointCompleted; 
        // when blm.OnSyncPointCompleted delegate is called, SyncPointCompleted will be called [together  with other 
        // event handler functions added to the delegate bml.OnSyncPointCompleted]. 
        // bmlNet.Update(Time.deltaTime);

    }
	
	// Update is called once per frame
	void Update () {
        // for every update of the virtual human, need to update BMLNet
        bmlNet.Update(Time.deltaTime);
	}

    /// <summary>
    /// callback when there is sync point that has been just encountered in the bml file
    /// this function will be called from BMLNet library
    /// </summary>
    /// <param name="behaviorID"></param>
    /// <param name="syncEventName"></param>
    /// 
    // Note    bmlNet.OnSyncPointCompleted(parentBlock.id, syncEventName) in Update() of BMLSyncPoint.cs will call
    // the following event handler.
    void SyncPointCompleted(string behaviorID, string syncEventName)
    {
        BMLBlock block = bmlNet.GetBehaviorFromId(behaviorID);

        // by Moon Jung, 2022/4/7

        Debug.Log( $"bmlNet.Timer={bmlNet.Timer}" );

        // get the character that will performs
        GameObject character = GameObject.Find(block.getCharacterId());

        Debug.Log(block.getCharacterId() + " " + behaviorID + " " + syncEventName);

        // cannot find the character: This also holds for the top level block whose character  is null
        if (character == null)
            return;

        if (block is BMLFace)
        {
            BMLFace face = (BMLFace)block;

        }
        else if (block is BMLFaceFacs)
        {
            BMLFaceFacs face = (BMLFaceFacs)block;

        }
        else if (block is BMLFaceLexeme)
        {
            BMLFaceLexeme face = (BMLFaceLexeme)block;

        }
        else if (block is BMLFaceShift)
        {
            BMLFaceShift face = (BMLFaceShift)block;

        }

        else if (block is BMLGaze)
        {
            // get the gaze behaviour
            BMLGaze gaze = (BMLGaze)block;

            // get the target that need to be gazed
            GameObject target = GameObject.Find(gaze.target);
            if (target != null)
            {
                HeadLookController controllerHead = character.GetComponent<HeadLookController>();
                 // controllerHead.active = false when an instance of HeadLookController is created
                if (controllerHead != null)
                {
                    // setting parameter for behaviour
                    controllerHead.targetNode = target.transform;

                    // setting parameter for callback
                    controllerHead.SetBMLNetParam(block.getCharacterId(), behaviorID, syncEventName);
                    // This call will set active = true among others.

    // refer to:
    // public void SetBMLNetParam(string characterId, string behaviourId, string eventname)
    // {
    //     Debug.Log("set parameter " + characterId + " " + behaviourId + " " + eventname);
        
    //     active = true; // this.active = active

    //     this.characterId = characterId;
    //     this.behaviourId = behaviourId;
    //     this.eventName = eventname;
    // }


                    // add callback
                    controllerHead.OnBehaviourCompleted += SetTriggerSyncPoint;
                     // SetTriggerSyncPoint() will be called when controller.OnBehaviourCompleted(,,) will be called in BMLNetBehaviour:
                     // controller.OnBehaviourCompleted(BMLNetBehaviour obj, string characterId, string behaviorId, string eventName)
                }
            }

        }
        else if (block is BMLGazeShift)
        {
            BMLGazeShift gazeShift = (BMLGazeShift)block;

        }

        else if (block is BMLGesture)
        {
            BMLGesture gesture = (BMLGesture)block;            

            Animator animator = character.GetComponentInChildren<Animator>();

            if (gesture.id == "gesture1") // Only one gesture called "gesture1" has been implemented
                animator.SetTrigger("Show"); // "gesture1" is implemented by means of animator component attached to the character, Sara.

            if (gesture.id == "gangnam") // Only one gesture called "gesture1" has been implemented
                animator.SetTrigger("Gangnam");


        }
        else if (block is BMLPointing)
        {
            BMLPointing pointing = (BMLPointing)block;

        }

        else if (block is BMLHead)
        {
            BMLHead head = (BMLHead)block;


        }
        else if (block is BMLHeadDirectionShift)
        {
            BMLHeadDirectionShift headDirectionShift = (BMLHeadDirectionShift)block;

        }

        else if (block is BMLLocomotion)
        {
            BMLLocomotion locomotion = (BMLLocomotion)block;

        }

        else if (block is BMLPosture)
        {
            BMLPosture posture = (BMLPosture)block;

        }
        else if (block is BMLPostureShift)
        {
            BMLPostureShift postureShift = (BMLPostureShift)block;

        }
        else if (block is BMLStance)
        {
            BMLStance stance = (BMLStance)block;

        }
        else if (block is BMLPose)
        {
            BMLPose pose = (BMLPose)block;

        }

        else if (block is BMLSpeech)
        {
            BMLSpeech speech = (BMLSpeech)block;

            voice.Speak(speech.text, SpeechVoiceSpeakFlags.SVSFlagsAsync);

        }
    }

    /// <summary>
    /// this function helper will be called from every sync point that need to be triggered from Unity
    /// See   controllerHead.OnBehaviourCompleted += SetTrigger above
    /// </summary>
    /// <param name="behaviour"></param>
    /// <param name="characterId"></param>
    /// <param name="behaviorId"></param>
    /// <param name="eventName"></param>
    public void SetTriggerSyncPoint(BMLNetBehaviour behaviour, string characterId, string behaviorId, string eventName)
    {
        // remove the callback
        behaviour.OnBehaviourCompleted -= SetTriggerSyncPoint;

        // trigger BMLNet callback
        bmlNet.TriggerSyncPoint(behaviorId, eventName);

        //  public void TriggerSyncPoint(string id, string eventName) // BMLNet.TriggerSyncPoint( , )
        // {
        //     if (scheduledBlocks.ContainsKey(id))
        //     {
        //         if (scheduledBlocks[id].syncPoints.ContainsKey(eventName) == false)
        //         {
        //             // create a new sync point
        //             scheduledBlocks[id].syncPoints.Add(eventName, new BMLSyncPoint(scheduledBlocks[id], eventName, ""));
        //         }

        //         // trigger sync point
        //         scheduledBlocks[id].syncPoints[eventName].TriggerSyncPoint(); // BMLSyncPoint.TriggerSyncPoint() for a given block id and sync point with eventName
        //     }
        // }

    } // public void SetTrigger

}
