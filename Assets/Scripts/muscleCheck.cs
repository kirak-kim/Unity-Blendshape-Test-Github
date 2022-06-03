using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muscleCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] muscleName = HumanTrait.MuscleName;
        int i = 0;
        while (i < HumanTrait.MuscleCount) {
            Debug.Log(muscleName[i]);
            i++;
        }
    }
}
