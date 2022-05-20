using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public enum ControllerMode
{
    TiltAllDirections,
    TiltHorizontalOnly,   
};

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    public float vSpeed = 1f;
    public Camera playerCam;
    private RenderLayer instance;
    public ControllerMode controllerMode;
    private ExplodingCube explodingCube;
    public AudioMixer mixer;
    public AudioSource rolling;
    public AudioSource boom;
    public AudioSource boom2;
    public AudioSource respawn;
    public AudioSource click;
    public AudioClip grind;
    public ShowPanelAndText showEnding;
    private bool allGone;
    public NextScene nextLevel;
    
    [Range (0f, 20f)]
    public float lowPassFadeIn;
    [Range (0f, 20f)]
    public float lowPassFadeOut;

    public static bool[] trackMute = new bool[9];
    private System.Random trackNumber = new System.Random();

    private int jamCount;

    // Use this for initialization

    private void Awake()
    { 
        DontDestroyOnLoad(this.gameObject);      
    }

    void Start()
    {
        StopAllCoroutines();
        allGone = false;
        for(int i=0; i<9; i++)
        {
            trackMute[i] = false;
            StartCoroutine(StartFadeIn(i, 0f, 0.2f, 0f));
            
        }
        //StartCoroutine(StartFadeIn(999, 0f, 0.2f, 0f));
        mixer.SetFloat("999 Volume", 0);

        showEnding.InitAlpha();
        rb = this.GetComponent<Rigidbody>();
        instance = RenderLayer.GetInstance();
        controllerMode = ControllerMode.TiltAllDirections;

        //showEnding.FadeIn();

        ExplodingCube[] cubes = GameObject.FindObjectsOfType<ExplodingCube>();
        explodingCube = cubes[0];
    }
    
    
    public void SetControllerMode(ControllerMode controllerMode)
    {
        this.controllerMode = controllerMode;
    }

    // Update is called once per frame
    void Update()
    {
        if (allGone == true)
        {
            //StartCoroutine(StartFadeOut(999, 0f, 0.7f, -80.0f));
            return;
        }
            
        float v = 0;
        float h = Input.GetAxis("Horizontal");
        /*
        if (controllerMode == ControllerMode.TiltAllDirections)
        {
            v = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(h * speed, 0, v * speed);
            rb.AddForce(movement);
        }
        else
        {
            v = 1f;
            Vector3 movement = new Vector3(h * speed, 0, 0);
            rb.AddForce(movement);
            movement = rb.velocity;
            movement.z = vSpeed;
            rb.velocity = movement;
        }
        */

#if UNITY_EDITOR_OSX
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        
        rb.AddForce(new Vector3(h * speed, 0, v * speed));
        

        //Camera.main.transform.Rotate(100*(vel1-vel0));
#endif
#if UNITY_IOS
        Vector3 acc = Input.acceleration;
        rb.AddForce(acc.x * 80f, 0, acc.y * 80f);
#endif
        //playerCam.transform.position = transform.position + instance.currentCamSettings.offset;
        
        //Debug.Log("Magnitude "+rb.velocity.magnitude);
        rolling.pitch = 0.2f*rb.velocity.magnitude;
        rolling.volume = rb.velocity.magnitude;
        
        

//        Debug.Log(instance.layer.maze1[Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.z)].color);
    }
    
    public IEnumerator StartFadeIn(int exposedParam, float waitTime, float duration, float targetValue)
    {
        yield return new WaitForSeconds(waitTime);
        float currentTime = 0;
        float currentVol;
        string Param = exposedParam+" Volume";
        mixer.GetFloat(Param, out currentVol);
        //float targetValue = Mathf.Clamp(targetVolume, -80f, 0f);
        
        while (currentTime < duration)
        {
            if (allGone == true)
                yield return null;

            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            //Debug.Log(newVol);
            mixer.SetFloat(Param, newVol);
            yield return null;
        }
        if(exposedParam != 999)
        {
            trackMute[exposedParam] = false;
        }
        
        yield break;
    }

    public IEnumerator StartFadeOut(int exposedParam, float waitTime, float duration, float targetValue)
    {
        yield return new WaitForSeconds(waitTime);
        float currentTime = 0;
        float currentVol;
        string Param = exposedParam + " Volume";
        mixer.GetFloat(Param, out currentVol);
        //float targetValue = Mathf.Clamp(targetVolume, -80f, 0f);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            //Debug.Log(newVol);
            mixer.SetFloat(Param, newVol);
            yield return null;
        }
        yield break;
    }

    public IEnumerator RegrowCube(Transform regrowingCube, float waitTime, float duration)
    {
        float currentTime = 0;

        yield return new WaitForSeconds(waitTime);
        respawn.pitch = Random.Range(0.9f, 1.2f);
        respawn.volume = 6.0f;
        respawn.PlayOneShot(grind);
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float y = Mathf.Lerp(-1, 1, currentTime / duration);
            Vector3 sixfeetunder = regrowingCube.position;
            sixfeetunder.y = y;
            regrowingCube.position = sixfeetunder;

            yield return null;
        }
        yield break;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (allGone == true)
            return;
        if(collision.gameObject.tag == "CollectableCube")
        {
            //Debug.Log("Velocity "+ collision.relativeVelocity.magnitude);

            
            boom.volume = 0.03f*collision.relativeVelocity.magnitude;
            boom2.volume = 0.03f*collision.relativeVelocity.magnitude;
            
            boom.pitch = Random.Range(0.9f, 1.2f);
            boom2.pitch = Random.Range(0.9f, 1.2f);
            
            boom.Play();
            boom2.Play();
            
            explodingCube.setPosition(this.transform.position);
            explodingCube.explode();

            // Destroy(collision.gameObject);

            int trNum = trackNumber.Next(0, 8);
            if (trackMute[trNum] == true)
            {
                int i = 0;
                while(trackMute[trNum] == true)
                {
                    trNum++;
                    trNum = trNum % 9;
                    i++;
                    if (i > 8)
                    {
                        allGone = true;
                        StartCoroutine(StartFadeOut(999, 0f, 0.7f, -80.0f));
                        //StopAllCoroutines();
                        showEnding.FadeIn();
                        nextLevel.LoadSlow("StartScreen");
                        break;
                    }
                        

                }
            }    
            trackMute[trNum] = true;
            Debug.Log("Track Number "+ trNum);

            //string num = trNum + " Volume";

            Vector3 sixfeetunder = collision.gameObject.transform.position;
            sixfeetunder.y = -1;
            collision.gameObject.transform.position = sixfeetunder;
            StartCoroutine(RegrowCube(collision.gameObject.transform, 12f, 2f));
            StartCoroutine(StartFadeIn(trNum, 12f, 2f, 0f));
            StartCoroutine(StartFadeOut(trNum, 0f, 0.2f, -80.0f));

        

            /*
            if (jamCount==0)
                StartCoroutine(StartFade("Drums Volume", 0.2f, -19.0f));
               // mixer.SetFloat("Drums Volume", -5.0f);
            if (jamCount==1)
                StartCoroutine(StartFade("Bass Volume", 0.2f, -19.0f));
               // mixer.SetFloat("Bass Volume", -5.0f);
            if (jamCount==2)
                StartCoroutine(StartFade("Rhodes Volume", 0.2f, -19.0f));
               // mixer.SetFloat("Rhodes Volume", -5.0f);
            if (jamCount==3)
                StartCoroutine(StartFade("Synth Volume", 0.2f, -19.0f));
            if (jamCount==4)
               StartCoroutine(StartFade("DrLoPass", lowPassFadeIn, 800));
            if (jamCount==5)
               StartCoroutine(StartFade("BLoPass", lowPassFadeIn, 800));
            if (jamCount==6)
                StartCoroutine(StartFade("RhLoPass", lowPassFadeIn, 800));
            if (jamCount==7)
                StartCoroutine(StartFade("SynthLoPass", lowPassFadeIn, 800));
            if (jamCount==8)
                StartCoroutine(StartFade("RhLoPass", lowPassFadeOut, 22000));
            if (jamCount==9)
                StartCoroutine(StartFade("DrLoPass", lowPassFadeOut, 22000));
            if (jamCount==10)
                StartCoroutine(StartFade("BLoPass", lowPassFadeOut, 22000));
            if (jamCount==11)
                StartCoroutine(StartFade("SynthLoPass", lowPassFadeOut, 22000));
            
            jamCount++;
            */
            
        }

        string tag = collision.gameObject.tag;
        if (tag == "WestWall" || tag == "EastWall" || tag == "NorthWall" || tag == "SouthWall")
        {
            click.pitch = 2.5f;

            //if (collision.relativeVelocity.magnitude>1.3f)
            if (Vector3.Scale(collision.contacts[0].normal, collision.relativeVelocity).magnitude > 1.3f)
            {
                //Debug.Log("Velocity " + Vector3.Scale(collision.contacts[0].normal, collision.relativeVelocity).magnitude);
                click.volume = 0.2f*Vector3.Scale(collision.contacts[0].normal, collision.relativeVelocity).magnitude;
                click.Play();
            }
            
        }
    }
    
}
