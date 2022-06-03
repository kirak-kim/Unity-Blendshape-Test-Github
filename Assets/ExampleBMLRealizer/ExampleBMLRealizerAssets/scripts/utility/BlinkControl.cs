using UnityEngine;
using System.Collections;

// BlinkControl component is attached to the mesh of Sara virtual human in addition to Skinned Mesh Renderer component
public class BlinkControl : MonoBehaviour {

    SkinnedMeshRenderer skinMeshRenderer;
    [SerializeField] private int _blendShapeEyesClosed;

	// Use this for initialization
	void Start () {
		skinMeshRenderer = GetComponent<SkinnedMeshRenderer> ();

        if (skinMeshRenderer != null)
        {
            Debug.Log(skinMeshRenderer.sharedMesh.blendShapeCount);

            string s = "";
            for (int i = 0; i < skinMeshRenderer.sharedMesh.blendShapeCount; i++)
            {
                s += "[" + i + "] = " + skinMeshRenderer.sharedMesh.GetBlendShapeName(i) + "\n";
            }
            Debug.Log(s);
        }


		StartCoroutine ("Blink");		
	}
	
	// Update is called once per frame
	void Update () {

//		skinMeshRenderer.SetBlendShapeWeight (1, 100);
//
//		StartCoroutine(Wait(5));
//
//		skinMeshRenderer.SetBlendShapeWeight (1, 0);
	}

	private IEnumerator Blink()
	{
		while (true) {
			skinMeshRenderer.SetBlendShapeWeight (_blendShapeEyesClosed, 100);
			//Debug.Log ("eyes closed");
			yield return new WaitForSeconds (Random.Range(0.0f,0.2f));
			skinMeshRenderer.SetBlendShapeWeight (_blendShapeEyesClosed, 0);
			//Debug.Log ("eyes opened");
			yield return new WaitForSeconds (Random.Range(1.0f,3.0f));
		}
	}
}


/*
[0] = head.PHMNoseCompressionHD
[1] = head.PHMCheeksDimpleCreaseHDR
[2] = head.PHMCheeksDimpleCreaseHDL
[3] = head.PHMBrowCompressionHD
[4] = head.CTRLCheeksDimpleCreaseHD
[5] = head.eCTRLSurprised
[6] = head.eCTRLSmile
[7] = head.eCTRLFrown
[8] = head.eCTRLFlirting
[9] = head.eCTRLAngry
[10] = head.pCTRLNeckHeadTwist
[11] = head.pCTRLNeckHeadSide-Side
[12] = head.pCTRLNeckHeadBend
[13] = head.ePHMCheekCreaseR
[14] = head.ePHMCheekCreaseL
[15] = head.eCTRLvW
[16] = head.eCTRLvUW
[17] = head.eCTRLvTH
[18] = head.eCTRLvT
[19] = head.eCTRLvSH
[20] = head.eCTRLvS
[21] = head.eCTRLvOW
[22] = head.eCTRLvM
[23] = head.eCTRLvL
[24] = head.eCTRLvK
[25] = head.eCTRLvIY
[26] = head.eCTRLvIH
[27] = head.eCTRLvF
[28] = head.eCTRLvER
[29] = head.eCTRLvEH
[30] = head.eCTRLvEE
[31] = head.eCTRLvAA
[32] = head.eCTRLTongueUp-Down
[33] = head.eCTRLTongueSide-Side
[34] = head.eCTRLTongueRaise-Lower
[35] = head.eCTRLTongueNarrow-Wide
[36] = head.eCTRLTongueIn-Out
[37] = head.eCTRLTongueBendTip
[38] = head.eCTRLTongue Curl
[39] = head.eCTRLNostrilsFlare
[40] = head.eCTRLNoseWrinkle
[41] = head.eCTRLNoseScrunch
[42] = head.eCTRLMouthSmileSimpleR
[43] = head.eCTRLMouthSmileSimpleL
[44] = head.eCTRLMouthSmileSimple
[45] = head.eCTRLMouthSmileOpen
[46] = head.eCTRLMouthSmile
[47] = head.eCTRLMouthSide-SideR
[48] = head.eCTRLMouthSide-SideL
[49] = head.eCTRLMouthSide-Side
[50] = head.eCTRLMouthOpenWide
[51] = head.eCTRLMouthOpen
[52] = head.eCTRLMouthNarrowR
[53] = head.eCTRLMouthNarrowL
[54] = head.eCTRLMouthNarrow
[55] = head.eCTRLMouthFrown
[56] = head.eCTRLMouthCornerUp-Down
[57] = head.eCTRLMouthCornerBackR
[58] = head.eCTRLMouthCornerBackL
[59] = head.eCTRLMouthCornerBack
[60] = head.eCTRLLipTopUp-DownR
[61] = head.eCTRLLipTopUp-DownL
[62] = head.eCTRLLipTopUp-Down
[63] = head.eCTRLLipTopIn-OutR
[64] = head.eCTRLLipTopIn-OutL
[65] = head.eCTRLLipTopIn-Out
[66] = head.eCTRLLipsPuckerWide
[67] = head.eCTRLLipsPucker
[68] = head.eCTRLLipsPartCenter
[69] = head.eCTRLLipsPart
[70] = head.eCTRLLipBottomUp-DownR
[71] = head.eCTRLLipBottomUp-DownL
[72] = head.eCTRLLipBottomUp-Down
[73] = head.eCTRLLipBottomIn-OutR
[74] = head.eCTRLLipBottomIn-OutL
[75] = head.eCTRLLipBottomIn-Out
[76] = head.eCTRLJawSide-Side
[77] = head.eCTRLJawOut-In
[78] = head.eCTRLEyesUpDown
[79] = head.eCTRLEyesSquintR
[80] = head.eCTRLEyesSquintL
[81] = head.eCTRLEyesSquint
[82] = head.eCTRLEyesSideSide
[83] = head.eCTRLEyesCrossed
[84] = head.eCTRLEyesClosedR
[85] = head.eCTRLEyesClosedL
[86] = head.eCTRLEyesClosed
[87] = head.eCTRLEyelidsUpperDownUpR
[88] = head.eCTRLEyelidsUpperDownUpL
[89] = head.eCTRLEyelidsUpperDownUp
[90] = head.eCTRLEyelidsLowerUpDownR
[91] = head.eCTRLEyelidsLowerUpDownL
[92] = head.eCTRLEyelidsLowerUpDown
[93] = head.eCTRLCheeksBalloonPucker
[94] = head.eCTRLCheeksBalloon
[95] = head.eCTRLCheekFlexR
[96] = head.eCTRLCheekFlexL
[97] = head.eCTRLCheekFlex
[98] = head.eCTRLCheekEyeFlexR
[99] = head.eCTRLCheekEyeFlexL
[100] = head.eCTRLCheekEyeFlex
[101] = head.eCTRLCheekCrease
[102] = head.eCTRLBrowUp-DownR
[103] = head.eCTRLBrowUp-DownL
[104] = head.eCTRLBrowUp-Down
[105] = head.eCTRLBrowSqueeze
[106] = head.eCTRLBrowOuterUp-DownR
[107] = head.eCTRLBrowOuterUp-DownL
[108] = head.eCTRLBrowOuterUp-Down
[109] = head.eCTRLBrowInnerUp-DownR
[110] = head.eCTRLBrowInnerUp-DownL
[111] = head.eCTRLBrowInnerUp-Down    
*/
