using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    public GameObject congratulation;
    public GameObject vc1;
    public GameObject vc2;
    public GameObject vc3;
    public GameObject endMenu;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            congratulation.SetActive(true);
            congratulation.transform.position=other.transform.position+Vector3.up*10;
            StartCoroutine(ChangeCamera1());
        }
    }
    IEnumerator ChangeCamera1()
    {
        SoundManager.PlaySound(SoundManager.Sound.Congratulation);

        yield return new WaitForSeconds(SoundManager.GetSoundLength(SoundManager.Sound.Congratulation));

        CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
      
        cinemachineBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
        cinemachineBrain.m_DefaultBlend.m_Time = 4.0f;
        vc1.SetActive(true);

        yield return new WaitForSeconds(4);
        vc1.SetActive(false);
        vc2.SetActive(true);
        yield return new WaitForSeconds(4);
        vc2.SetActive(false);
        vc3.SetActive(true);
        yield return new WaitForSeconds(4);
        congratulation.SetActive(false);
        endMenu.SetActive(true);
    }
    public void PlayAgain()
    {
        Debug.Log("start");
        //SceneManager.LoadScene("Start");
    }
    public void Menu()
    {
        Debug.Log("Menu");
        //SceneManager.LoadScene("Menu");
    }
   
}
