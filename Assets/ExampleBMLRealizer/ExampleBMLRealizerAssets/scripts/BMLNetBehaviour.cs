using UnityEngine;
using System.Collections;

public class BMLNetBehaviour : MonoBehaviour {

    public bool active = false;
    public string characterId = "";
    public string behaviourId = "";
    public string eventName = "";

    public delegate void BehaviourCompleted(BMLNetBehaviour obj, string characterId, string behaviourId, string eventName);
    public BehaviourCompleted OnBehaviourCompleted;  
    // controller.OnBehaviourCompleted += SetTrigger in  void SyncPointCompleted(string behaviorID, string eventName) in VirtualHumanController.cs


    //void Start() {}
    
    //void Update() {}
    public void SetBMLNetParam(string characterId, string behaviourId, string eventname)
    {
        Debug.Log("set parameter " + characterId + " " + behaviourId + " " + eventname);
        
        active = true;

        this.characterId = characterId;
        this.behaviourId = behaviourId;
        this.eventName = eventname;
    }

    protected void TriggerEvent() 
    // BMLNetBehaviour.TriggerEvent() is called when the behaviour is completed; e.g. called from HeadLookController.LateUpdate()
    // which is a subclass of this class BMLNetBehaviour
    {
        if (OnBehaviourCompleted != null)
        {
            Debug.Log("Trigger Event " + this.GetType() + " " + characterId + " " + behaviourId + " " + eventName);
            OnBehaviourCompleted(this, characterId, behaviourId, eventName);
            // This call of the delegate OnBehaviourCompleted will call the event handler added to it:
            //  SetTriggerSyncPoint(BMLNetBehaviour obj, string characterId, string behaviorId, string eventName) in VirtualHumanController
        }
    }

   


}
