using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class BlendshapeParse : MonoBehaviour
{
    public Transform skin;
    SkinnedMeshRenderer skinnedMeshRenderer;

    public Transform teeth;
    SkinnedMeshRenderer skinnedMeshRendererTeeth;

    public string textRoot;

    private List<float[]> blendshapeWeights;

    float[] BlendshapesCurrent = new float[64];

    int frame = 0;

    void Start(){
        Debug.Log("parsing happens");
        skinnedMeshRenderer = skin.GetComponent<SkinnedMeshRenderer>();
        skinnedMeshRendererTeeth = teeth.GetComponent<SkinnedMeshRenderer>();

        BlendShapeParse();
    }

    void Update(){
        float[] BlendshapeGoal = blendshapeWeights[frame];
        for (int i = 0; i < 63; i++) { skinnedMeshRenderer.SetBlendShapeWeight(i, (int)BlendshapeGoal[i]); }

        if(frame < blendshapeWeights.Count-1){
            frame++;
        }
    }

    public void BlendShapeParse(){
        string text = File.ReadAllText(textRoot);

        string[] textLines = text.Split('\n');
        
        blendshapeWeights = new List<float[]>();

        foreach(string line in textLines){
            float[] weights = Array.ConvertAll(line.Split(','), float.Parse);
            blendshapeWeights.Add(weights);
        }
    }
}
