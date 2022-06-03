using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlendShapes : MonoBehaviour {
	

	SkinnedMeshRenderer skinMeshRenderer;
	// Use this for initialization
	void Start () {
		
		skinMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {

		//skinMeshRenderer.SetBlendShapeWeight (0, 100);
	}
	
	public void AngryExpression(){
		float angry_slider = 0.0F;

		angry_slider = GameObject.Find ("SldAngry").GetComponent<Slider> ().value;

		skinMeshRenderer.SetBlendShapeWeight (0, angry_slider * 100);
	}

	public void HappyExpression(){
		float happy_slider = 0.0F;
		
		happy_slider = GameObject.Find ("SldHappy").GetComponent<Slider> ().value;
		
		skinMeshRenderer.SetBlendShapeWeight (3, happy_slider * 100);
	}

	public void SadExpression(){
		float sad_slider = 0.0F;
		
		sad_slider = GameObject.Find ("SldSad").GetComponent<Slider> ().value;
		
		skinMeshRenderer.SetBlendShapeWeight (2, sad_slider * 100);
	}

	public void SurprisedExpression(){
		float surprised_slider = 0.0F;
		
		surprised_slider = GameObject.Find ("SldSurprised").GetComponent<Slider> ().value;
		
		skinMeshRenderer.SetBlendShapeWeight (4, surprised_slider * 100);
	}
}
