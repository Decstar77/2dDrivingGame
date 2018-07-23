using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {


	public void PlaySingleLevel()
	{
		SceneManager.LoadScene("SingleLevel");
	}
	public void PlayInfiniteLevel()
	{
		SceneManager.LoadScene("SingleLevel");
	}
	public void RestartLevel()
	{
		SceneManager.LoadScene("SingleLevel");
	}
}
