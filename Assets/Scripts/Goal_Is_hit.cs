using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Goal_Is_hit : MonoBehaviour
{
  [HideInInspector] public bool is_hit_red=false;
  [HideInInspector] public bool is_hit_blue=false;
  [HideInInspector] public bool isHit=false;
  [HideInInspector] public int total_blue_goals=0;
  [HideInInspector] public int total_red_goals=0;
  [SerializeField]private ParticleSystem particle_goalhit;
  [SerializeField]private Camera cam;
  [SerializeField]private TMP_Text text;




    void OnTriggerEnter(Collider ball)
    {
        isHit=true;
        if (ball.CompareTag("blue_goal"))
        {
            is_hit_blue=true;
            total_red_goals+=1;
            // this fun will apply shake effect + text
            apply_effects();



            
        }

        if (ball.CompareTag("red_goal"))
        {
            is_hit_red=true;
            total_blue_goals+=1;
            apply_effects();
            





        }
    
    }


IEnumerator CamShake(float duration, float strength)
{
    float elapsed = 0f;
    Vector3 startPos = Camera.main.transform.localPosition;

    while (elapsed < duration)
    {
        float x = (Mathf.PerlinNoise(elapsed * 20f, 0f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(0f, elapsed * 20f) - 0.5f) * 2f;

        Camera.main.transform.localPosition = startPos + new Vector3(x, y, 0f) * strength;

        elapsed += Time.deltaTime;
        yield return null;
    }

    Camera.main.transform.localPosition = startPos;
}
IEnumerator dest_gameobj(TMP_Text obj, float duration)
{
    float elapsed = 0f;


    while (elapsed < duration)
    {
       



        elapsed += Time.deltaTime;
        yield return null;
    }
    Destroy(obj);


}
IEnumerator slow_mo(float duration)
{
    float elapsed = 0f;

    Time.timeScale=.5f;
    

    while (elapsed < duration)
    {

        elapsed += Time.deltaTime;
        yield return null;
    }
    Time.timeScale = 1f;


}
void apply_effects()

    {
            StartCoroutine(slow_mo(.2f));
            // particale
            Instantiate(particle_goalhit,transform.position,Quaternion.identity);
            // shake
            StartCoroutine(CamShake(0.3f, 0.15f));
            // text effect
            var instance_text=Instantiate(text,transform.position,Quaternion.identity);
            instance_text.transform.LookAt(cam.transform.position);
            instance_text.transform.Rotate(0f,180f,0f);
            StartCoroutine(dest_gameobj(instance_text,1f));
        
    }



}


