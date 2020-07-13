using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject explanied;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void ShowExplain() {
        explanied.SetActive(true);
        animator.enabled = false;
    }

    public void StartGame() {
        SceneManager.LoadScene("SampleScene");
    }
}
