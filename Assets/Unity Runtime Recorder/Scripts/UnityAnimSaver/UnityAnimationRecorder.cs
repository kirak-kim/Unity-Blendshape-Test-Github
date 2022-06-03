#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class UnityAnimationRecorder : MonoBehaviour {

	// save file path
	public string savePath;
	public string fileName;

	// use it when save multiple files
	int fileIndex = 0;

	public KeyCode startRecordKey = KeyCode.Q;
	public KeyCode stopRecordKey = KeyCode.W;

	// options
	public bool showLogGUI = false;
	string logMessage = "";

	public bool recordLimitedFrames = false;
	public int recordFrames = 1000;
	int frameIndex = 0;

	public bool changeTimeScale = false;
	public float timeScaleOnStart = 0.0f;
	public float timeScaleOnRecord = 1.0f;

	public bool recordBlendShape = false;


	Transform[] recordObjs;
	SkinnedMeshRenderer[] blendShapeObjs;
	UnityObjectAnimation[] objRecorders;
	List<UnityBlendShapeAnimation> blendShapeRecorders;

	bool isStart = false;
	float nowTime = 0.0f;

	// Use this for initialization
	void Start () {
		SetupRecorders ();

	}

	void SetupRecorders () {

		this.recordObjs = this.gameObject.GetComponentsInChildren<Transform> ();

		this.objRecorders = new UnityObjectAnimation[recordObjs.Length]; //각각의 children에 대한 애니메이션
		this.blendShapeRecorders = new List<UnityBlendShapeAnimation> ();

		this.frameIndex = 0;
		this.nowTime = 0.0f;

		for (int i = 0; i < this.recordObjs.Length; i++) {
			//public static string GetTransformPathName ( Transform rootTransform, Transform targetTransform ) 
			string path = AnimationRecorderHelper.GetTransformPathName (this.gameObject.transform, this.recordObjs [i]); //Path is the child name (kirak)

            // Add an animation recorder for the current object (mesh) .
			this.objRecorders [i] = new UnityObjectAnimation ( path, this.recordObjs [i]);

			// check if theres blendShape
			if (this.recordBlendShape) {  // Does this.recordObjs, the children of this.gameObject, have blendShapes ?
				if (this.recordObjs [i].GetComponent<SkinnedMeshRenderer> ()) {
					SkinnedMeshRenderer tempSkinMeshRenderer = this.recordObjs [i].GetComponent<SkinnedMeshRenderer> ();

					// there is blendShape exist
					if (tempSkinMeshRenderer.sharedMesh.blendShapeCount > 0) {

                        // Add an blenshapes animation recorder to the current object (mesh)
						this.blendShapeRecorders.Add (new UnityBlendShapeAnimation (path, tempSkinMeshRenderer)); // Making curves, but not assigning values
					}
				}
			}
		}

		if (changeTimeScale)
			Time.timeScale = timeScaleOnStart;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown (startRecordKey)) {
			StartRecording ();
		}

		if (Input.GetKeyDown (stopRecordKey)) {
			StopRecording ();
		}

		if (isStart) {
			nowTime += Time.deltaTime;

			for (int i = 0; i < objRecorders.Length; i++) {
				objRecorders [i].AddFrame (nowTime);
			}

			if (recordBlendShape) {
				for (int i = 0; i < blendShapeRecorders.Count; i++) {
					blendShapeRecorders [i].AddFrame (nowTime); // Assigning values to curves
				}
			}
		}

	}

	public void StartRecording () {
		CustomDebug ("Start Recorder");
		isStart = true;
		Time.timeScale = timeScaleOnRecord;
	}


	public void StopRecording () {
		CustomDebug ("End Record, generating .anim file");
		isStart = false;

		ExportAnimationClip ();
		ResetRecorder ();
	}

	void ResetRecorder () {
		SetupRecorders ();
	}


	void FixedUpdate () {

		if (isStart) {

			if (recordLimitedFrames) {
				if (frameIndex < recordFrames) {
					for (int i = 0; i < objRecorders.Length; i++) {
						objRecorders [i].AddFrame (nowTime); // Assigning values to curves
					}

					++frameIndex;
				}
				else {
					isStart = false;
					ExportAnimationClip ();
					CustomDebug ("Recording Finish, generating .anim file");
				}
			}

		}
	}

	void OnGUI () {
		if (showLogGUI)
			GUILayout.Label (logMessage);
	}

	void ExportAnimationClip () {

		string exportFilePath = savePath + fileName;

		// if record multiple files when run
		if (fileIndex != 0)
			exportFilePath += "-" + fileIndex + ".anim";
		else
			exportFilePath += ".anim";


		AnimationClip clip = new AnimationClip ();

		clip.name = fileName;

		for (int i = 0; i < this.objRecorders.Length; i++) {
			
			UnityCurveContainer[] curves = this.objRecorders [i].curves;

			for (int x = 0; x < curves.Length; x++) { // curves.Length refers to the number of properties to be animated
				clip.SetCurve (this.objRecorders [i].pathName, typeof(Transform), curves [x].propertyName, curves [x].animCurve); //Setting curves to a animation clip
				//clip.SetCurve (this.objRecorders [i].pathName, typeof(Animator), curves [x].propertyName, curves [x].animCurve); //Making Humanoid animation (kirak)
				//     All keys defined in the animation curve.
                // public Keyframe[] keys { get; set; }
				//  public Keyframe this[int index] { get; }
			}
		}

		if (this.recordBlendShape) {
			for (int i = 0; i < this.blendShapeRecorders.Count; i++) {

				UnityCurveContainer[] curves = this.blendShapeRecorders [i].curves;

				for (int x = 0; x < curves.Length; x++) {
					clip.SetCurve ( this.blendShapeRecorders [i].pathName, typeof(SkinnedMeshRenderer), curves [x].propertyName, curves [x].animCurve);
					//clip.SetCurve ( this.blendShapeRecorders [i].pathName, typeof(Animator), curves [x].propertyName, curves [x].animCurve); //Making Humanoid animation (kirak)
					// the key values in  curves [x].animCurve are the weight values of the x-th blendshape
				}
				
			}
		}

		clip.EnsureQuaternionContinuity (); //Realigns quaternion keys to ensure shortest interpolation paths.(kirak)
		AssetDatabase.CreateAsset ( clip, exportFilePath );

		CustomDebug (".anim file generated to " + exportFilePath);
		fileIndex++;
	}

	void CustomDebug ( string message ) {
		if (showLogGUI)
			logMessage = message;
		else
			Debug.Log (message);
	}
}
#endif