using UnityEngine;
using System.Collections;

public class BlinkController : MonoBehaviour {

    public Transform[] UpperEyeLid;
    public Transform[] LowerEyeLid;
    public Transform[] EyeBalls;

    public float MaxAngleLowerEyeLid;
    public float MaxAngleUpperEyeLid;

    // Caffier, Philipp P., Udo Erdmann, and Peter Ullsperger. "Experimental evaluation of eye-blink parameters as a drowsiness measure." European journal of applied physiology 89.3-4 (2003): 319-325.
    public float duration = 0.202f;
    public float variance = 1.47411f;
    public float closingPercentage = 0.3f;  // percentage
    public float reopeningTime = 0.13836f;
    public float frequency = 16.33f;    // per 60 seconds

    // https://www.liverpool.ac.uk/~pcknox/teaching/Eymovs/params.htm#sac
    public float saccadeDuration = 0.05f;

    public Vector3 targetVector = Vector3.zero;
    public Transform target;

    private float stdDeviation;
    public float currentDuration;
    public float currentTime;

    public enum State
    {
        IDLE        = 0,
        BLINK_CLOSE = 1,
        BLINK_OPEN  = 2
    };

    public State state;

    // Use this for initialization
    void Start () {
        stdDeviation = Mathf.Sqrt(variance);

        if (EyeBalls.Length == 2)
        {
            targetVector = (EyeBalls[0].position + EyeBalls[1].position) / 2;
            targetVector += (EyeBalls[0].forward.normalized) * 2;
        }

        state = NextState(State.IDLE);
    }
	
	void LateUpdate () {

        switch (state)
        {
            case State.IDLE:
                break;

            case State.BLINK_CLOSE:
                for (int i = 0; i < UpperEyeLid.Length; i++)
                {
                    float angle = Mathf.LerpAngle(0, MaxAngleUpperEyeLid, currentTime / currentDuration);
                    UpperEyeLid[i].localEulerAngles = new Vector3(angle, 0, 0);
                }

                for (int i = 0; i < LowerEyeLid.Length; i++)
                {
                    float angle = Mathf.LerpAngle(0, MaxAngleLowerEyeLid, currentTime / currentDuration);
                    LowerEyeLid[i].localEulerAngles = new Vector3(angle, 0, 0);
                }
                break;

            case State.BLINK_OPEN:
                for (int i = 0; i < UpperEyeLid.Length; i++)
                {
                    float angle = Mathf.LerpAngle(MaxAngleUpperEyeLid, 0, currentTime / currentDuration);
                    UpperEyeLid[i].localEulerAngles = new Vector3(angle, 0, 0);
                }

                for (int i = 0; i < LowerEyeLid.Length; i++)
                {
                    float angle = Mathf.LerpAngle(MaxAngleLowerEyeLid, 0, currentTime / currentDuration);
                    LowerEyeLid[i].localEulerAngles = new Vector3(angle, 0, 0);
                }
                break;
        }

        for (int i = 0; i < EyeBalls.Length; i++)
        {
//            EyeBalls[i].localEulerAngles = Vector3.Slerp(EyeBalls[i].localEulerAngles, targetVector, Time.deltaTime);
        }

        currentTime += Time.deltaTime;
        if (currentTime >= currentDuration)
        {
            currentTime = 0;

            state = NextState(state);
        }
    }
    
    private State NextState(State state)
    {
        switch (state)
        {
            // starting to close the eyes
            case State.IDLE:
                //currentDuration = ((stdDeviation * Random.Range(-1.0f, 1.0f)) + duration) * closingPercentage;
                currentDuration = duration * closingPercentage;
                return State.BLINK_CLOSE;

            // starting to open the eyes
            case State.BLINK_CLOSE:
                // time to open the eyes
                currentDuration = currentDuration / closingPercentage;
                return State.BLINK_OPEN;

            // starting to idle
            case State.BLINK_OPEN:
            default:
                // time to close eyes again
                currentDuration = Random.Range(40, 100) / frequency;
                return State.IDLE;
        }
    }

}
