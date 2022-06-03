using UnityEngine;
using System.Collections;

public class UnityObjectAnimation {

	public UnityCurveContainer[] curves;
	public Transform observeGameObject;
	public string pathName = "";

	public UnityObjectAnimation( string hierarchyPath, Transform observeObj ) {
		pathName = hierarchyPath;
		observeGameObject = observeObj;

		curves = new UnityCurveContainer[10];

		curves [0] = new UnityCurveContainer( "localPosition.x" );
		curves [1] = new UnityCurveContainer( "localPosition.y" );
		curves [2] = new UnityCurveContainer( "localPosition.z" );

		curves [3] = new UnityCurveContainer( "localRotation.x" );
		curves [4] = new UnityCurveContainer( "localRotation.y" );
		curves [5] = new UnityCurveContainer( "localRotation.z" );
		curves [6] = new UnityCurveContainer( "localRotation.w" ); //A quaternion can represent a 3D rotation and is defined by 4 real numbers. x, y and z represent a vector. w is a scalar that stores the rotation around the vector.(kirak)


		curves [7] = new UnityCurveContainer( "localScale.x" );
		curves [8] = new UnityCurveContainer( "localScale.y" );
		curves [9] = new UnityCurveContainer( "localScale.z" );
	}

	public void AddFrame ( float time ) {

		curves [0].AddValue (time, observeGameObject.localPosition.x); //AddValue() is a custom function (kirak)
		curves [1].AddValue (time, observeGameObject.localPosition.y);
		curves [2].AddValue (time, observeGameObject.localPosition.z);

		curves [3].AddValue (time, observeGameObject.localRotation.x);
		curves [4].AddValue (time, observeGameObject.localRotation.y);
		curves [5].AddValue (time, observeGameObject.localRotation.z);
		curves [6].AddValue (time, observeGameObject.localRotation.w);

		curves [7].AddValue (time, observeGameObject.localScale.x);
		curves [8].AddValue (time, observeGameObject.localScale.y);
		curves [9].AddValue (time, observeGameObject.localScale.z);

		// public void AddValue( float animTime, float animValue )
	{    //  public Keyframe(float time, float value, float inTangent, float outTangent);
		//Keyframe key = new Keyframe (animTime, animValue, 0.0f, 0.0f);
		//this.animCurve.AddKey (key);
	}

	}
}
