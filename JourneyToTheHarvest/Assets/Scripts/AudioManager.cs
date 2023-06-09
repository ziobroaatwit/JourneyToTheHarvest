using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;
    private void Awake()
    {
        if (_instance == null)
        { _instance = this; }
        else
        {
            Destroy(this);
        }
    }
    public static AudioManager instance()
    {
        return _instance;
    }
    [SerializeField] private AudioSource winterSource;
    [SerializeField] private AudioSource springSource;
    [SerializeField] private AudioSource summerSource;
    [SerializeField] private AudioSource fallSource;
    [SerializeField] private AudioSource spookySource;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "WinterZone" && !winterSource.isPlaying)
        {
            print("Playing Winter Music");
            winterSource.Play();
        }
        if (collision.gameObject.tag == "SpringZone" && !springSource.isPlaying)
        {
            print("Playing Spring Music");
            springSource.Play();
        }
        if(collision.gameObject.tag == "SummerZone" && !summerSource.isPlaying)
        {
            print("Playing Summer Music");
            summerSource.Play();
        }
        if(collision.gameObject.tag == "FallZone" && !fallSource.isPlaying)
        {
            print("Playing Fall Music");
            fallSource.Play();
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "WinterZone" && winterSource.isPlaying)
        {
            print("Stopping Winter Music");
            winterSource.Stop();
        }
        if (collision.gameObject.tag == "SpringZone" && springSource.isPlaying)
        {
            print("Stopping Spring Music");
            springSource.Stop();
        }
        if (collision.gameObject.tag == "SummerZone" && summerSource.isPlaying)
        {
            print("Stopping Summer Music");
            summerSource.Stop();
        }
        if (collision.gameObject.tag == "FallZone" && fallSource.isPlaying)
        {
            print("Stopping Fall Music");
            fallSource.Stop();
        }
    }
}
