using UnityEngine;
using System.Collections;
[System.Serializable]
public class BendingSegment {
	public Transform firstTransform;
	public Transform lastTransform;
	public float thresholdAngleDifference = 0;
	public float bendingMultiplier = 0.6f;
	public float maxAngleDifference = 30;
	public float maxBendingAngle = 80;
	public float responsiveness = 5;
	internal float angleH;
	internal float angleV;
	internal Vector3 dirUp;
	internal Vector3 referenceLookDir;
	internal Vector3 referenceUpDir;
	internal int chainLength;
	internal Quaternion[] origRotations;
    internal Quaternion oldLookRotation;
}

[System.Serializable]
public class NonAffectedJoints {
	public Transform joint;
	public float effect = 0;
}

public class HeadLookController : BMLNetBehaviour {
	
	public Transform rootNode;
	public BendingSegment[] segments;
	public NonAffectedJoints[] nonAffectedJoints;
	public Vector3 headLookVector = Vector3.forward;
	public Vector3 headUpVector = Vector3.up;
	public Vector3 targetVector = Vector3.zero;
	public float effect = 1;
	public bool overrideAnimation = false;
    public Transform targetNode; // used for  controller.targetNode = target.transform;
    public Vector3 defaultTargetVector = Vector3.forward;

    public Transform[] eyeBalls;
    public float maxEyeHorizontalAngle = 30;
    public float maxEyeVerticalAngle = 15;

    void Start () {
		if (rootNode == null) {
			rootNode = transform; // this.rootNote = base.transform
		}
		
		// Setup segments: This loop has no effect because segments is empty
		foreach (BendingSegment segment in segments) {
			Quaternion parentRot = segment.firstTransform.parent.rotation;
			Quaternion parentRotInv = Quaternion.Inverse(parentRot);
			segment.referenceLookDir =
				parentRotInv * rootNode.rotation * headLookVector.normalized;
			segment.referenceUpDir =
				parentRotInv * rootNode.rotation * headUpVector.normalized;
			segment.angleH = 0;
			segment.angleV = 0;
			segment.dirUp = segment.referenceUpDir;
			
			segment.chainLength = 1;
			Transform t = segment.lastTransform;
			while (t != segment.firstTransform && t != t.root) {
				segment.chainLength++;
				t = t.parent;
			}
			
			segment.origRotations = new Quaternion[segment.chainLength];
			t = segment.lastTransform;
			for (int i=segment.chainLength-1; i>=0; i--) {
				segment.origRotations[i] = t.localRotation;
				t = t.parent;
			}
		} // foreach (BendingSegment segment in segments) 
	} // Start()
	
	void LateUpdate () {
        // always look at the target or look at forward
        if (targetNode == null)
        {
            targetVector = defaultTargetVector;
        }
        else
        {
            targetVector = targetNode.position;
        }

        if (active == false) // BMLNetBehaviour.active
            return; // active = true means "in progress"; active = false means the head look behaviour is completed

		if (Time.deltaTime == 0)
			return;
		
		// Remember initial directions of joints that should not be affected
		Vector3[] jointDirections = new Vector3[nonAffectedJoints.Length];
		for (int i=0; i<nonAffectedJoints.Length; i++) {
			foreach (Transform child in nonAffectedJoints[i].joint) {
				jointDirections[i] = child.position - nonAffectedJoints[i].joint.position;
				break;
			}
		}

        active = false; // // BMLNetBehaviour.active was true; set it to false tentatively; it will be true when the jead look behaviour is completed

		// Handle each segment: This loop has no effect because segments is empty
		foreach (BendingSegment segment in segments) {
			
			Transform t = segment.lastTransform;
			if (overrideAnimation) {
				for (int i=segment.chainLength-1; i>=0; i--) {
					t.localRotation = segment.origRotations[i];
					t = t.parent;
				}
			}
			
			Quaternion parentRot = segment.firstTransform.parent.rotation;
			Quaternion parentRotInv = Quaternion.Inverse(parentRot);
			
			// Desired look direction in world space
			Vector3 lookDirWorld = (targetVector - segment.lastTransform.position).normalized;

            //Debug.Log(lookDirWorld);
			
			// Desired look directions in neck parent space
			Vector3 lookDirGoal = (parentRotInv * lookDirWorld);
			
			// Get the horizontal and vertical rotation angle to look at the target
			float hAngle = AngleAroundAxis(
				segment.referenceLookDir, lookDirGoal, segment.referenceUpDir
			);
			
			Vector3 rightOfTarget = Vector3.Cross(segment.referenceUpDir, lookDirGoal);
			
			Vector3 lookDirGoalinHPlane =
				lookDirGoal - Vector3.Project(lookDirGoal, segment.referenceUpDir);
			
			float vAngle = AngleAroundAxis(
				lookDirGoalinHPlane, lookDirGoal, rightOfTarget
			);
			
			// Handle threshold angle difference, bending multiplier,
			// and max angle difference here

			float hAngleThr = Mathf.Max(
				0, Mathf.Abs(hAngle) - segment.thresholdAngleDifference
			) * Mathf.Sign(hAngle);
			
			float vAngleThr = Mathf.Max(
				0, Mathf.Abs(vAngle) - segment.thresholdAngleDifference
			) * Mathf.Sign(vAngle);
			
			hAngle = Mathf.Max(
				Mathf.Abs(hAngleThr) * Mathf.Abs(segment.bendingMultiplier),
				Mathf.Abs(hAngle) - segment.maxAngleDifference
			) * Mathf.Sign(hAngle) * Mathf.Sign(segment.bendingMultiplier);
			
			vAngle = Mathf.Max(
				Mathf.Abs(vAngleThr) * Mathf.Abs(segment.bendingMultiplier),
				Mathf.Abs(vAngle) - segment.maxAngleDifference
			) * Mathf.Sign(vAngle) * Mathf.Sign(segment.bendingMultiplier);
			
			// Handle max bending angle here
			hAngle = Mathf.Clamp(hAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
			vAngle = Mathf.Clamp(vAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
			
			Vector3 referenceRightDir =
				Vector3.Cross(segment.referenceUpDir, segment.referenceLookDir);
			
			// Lerp angles
			segment.angleH = Mathf.Lerp(
				segment.angleH, hAngle, Time.deltaTime * segment.responsiveness
			);
			segment.angleV = Mathf.Lerp(
				segment.angleV, vAngle, Time.deltaTime * segment.responsiveness
			);
			
			// Get direction
			lookDirGoal = Quaternion.AngleAxis(segment.angleH, segment.referenceUpDir)
				* Quaternion.AngleAxis(segment.angleV, referenceRightDir)
				* segment.referenceLookDir;
			
			// Make look and up perpendicular
			Vector3 upDirGoal = segment.referenceUpDir;
			Vector3.OrthoNormalize(ref lookDirGoal, ref upDirGoal);
			
			// Interpolated look and up directions in neck parent space
			Vector3 lookDir = lookDirGoal;
			segment.dirUp = Vector3.Slerp(segment.dirUp, upDirGoal, Time.deltaTime*5);
			Vector3.OrthoNormalize(ref lookDir, ref segment.dirUp);
			
			// Look rotation in world space
			Quaternion lookRot = (
				(parentRot * Quaternion.LookRotation(lookDir, segment.dirUp))
				* Quaternion.Inverse(
					parentRot * Quaternion.LookRotation(
						segment.referenceLookDir, segment.referenceUpDir
					)
				)
			);
			
			// Distribute rotation over all joints in segment
			Quaternion dividedRotation =
				Quaternion.Slerp(Quaternion.identity, lookRot, effect / segment.chainLength);
			t = segment.lastTransform;
			for (int i=0; i<segment.chainLength; i++) {
				t.rotation = dividedRotation * t.rotation;
				t = t.parent;
			}

            Quaternion q = dividedRotation * Quaternion.Inverse(segment.oldLookRotation);

            //Debug.Log(q);

            // if this behavior finish => Then 
            if (Mathf.Abs(hAngle) > Mathf.Epsilon || Mathf.Abs(vAngle) > Mathf.Epsilon)
            {
                active = true;
            }

            segment.oldLookRotation = dividedRotation;

		} // foreach (BendingSegment segment in segments)

        if (active  == false) // this.active is made false in line 104 above. 
                              // It was made true initially in controllerHead.SetBMLNetParam(block.getCharacterId(), behaviorID, eventName); 
							  // in   SyncPointCompleted(string behaviorID, string eventName) in VirtualHumanController
        {
            TriggerEvent(); // // // BMLNetBehaviour.TriggerEvent()
			// refer to:
			
	// 	protected void TriggerEvent()
    // {
    //     if (OnBehaviourCompleted != null)
    //     {
    //         Debug.Log("Trigger Event " + this.GetType() + " " + characterId + " " + behaviourId + " " + eventName);
    //         OnBehaviourCompleted(this, characterId, behaviourId, eventName);
    //         // will call: public void SetTrigger(BMLNetBehaviour obj, string characterId, string behaviorId, string eventName) in VirtualHumanController
    //     }
    // }

			//////////////////////////
        }
		
		// Handle non affected joints
		for (int i=0; i<nonAffectedJoints.Length; i++) {
			Vector3 newJointDirection = Vector3.zero;
			
			foreach (Transform child in nonAffectedJoints[i].joint) {
				newJointDirection = child.position - nonAffectedJoints[i].joint.position;
				break;
			}
			
			Vector3 combinedJointDirection = Vector3.Slerp(
				jointDirections[i], newJointDirection, nonAffectedJoints[i].effect
			);
			
			nonAffectedJoints[i].joint.rotation = Quaternion.FromToRotation(
				newJointDirection, combinedJointDirection
			) * nonAffectedJoints[i].joint.rotation;
		}

        // Handle for eye balls
        for (int i = 0;  i < eyeBalls.Length; i++)
        {
            Vector3 relativePos = targetVector - eyeBalls[i].position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            Vector3 euler = rotation.eulerAngles;

            if (euler.x < 180)
                euler.x = Mathf.Clamp(euler.x, 0, maxEyeVerticalAngle);
            else if (euler.x > 180)
                euler.x = Mathf.Clamp(euler.x, 360 - maxEyeVerticalAngle, 360);

            if (euler.y < 180)
                euler.y = Mathf.Clamp(euler.y, 0, maxEyeHorizontalAngle);
            else if (euler.y > 180)
                euler.y = Mathf.Clamp(euler.y, 360 - maxEyeHorizontalAngle, 360);

            eyeBalls[i].rotation = Quaternion.Euler(euler);
            
        }
    } // void LateUpdate () 
	
	// The angle between dirA and dirB around axis
	public static float AngleAroundAxis (Vector3 dirA, Vector3 dirB, Vector3 axis) {
		// Project A and B onto the plane orthogonal target axis
		dirA = dirA - Vector3.Project(dirA, axis);
		dirB = dirB - Vector3.Project(dirB, axis);
		
		// Find (positive) angle between A and B
		float angle = Vector3.Angle(dirA, dirB);
		
		// Return angle multiplied with 1 or -1
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
	}
}
