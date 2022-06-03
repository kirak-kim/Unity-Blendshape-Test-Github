using UnityEngine;
using System.Collections;

public class UnityCurveContainer {

	public string propertyName = "";
	public AnimationCurve animCurve;

	public UnityCurveContainer( string _propertyName ) {
		this.animCurve = new AnimationCurve ();
		this.propertyName = _propertyName;
	}

	public void AddValue( float animTime, float animValue )
	{    //  public Keyframe(float time, float value, float inTangent, float outTangent);
		Keyframe key = new Keyframe (animTime, animValue, 0.0f, 0.0f);
		this.animCurve.AddKey (key);
	}
}
