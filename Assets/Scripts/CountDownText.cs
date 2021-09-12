using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownText : MonoBehaviour
{
	private Text m_Text;

    private void Start()
    {
	}

    private void Update()
    {
	}

	private IEnumerator CountdownCoroutine()
	{

		m_Text.text = "3";
		yield return new WaitForSeconds(1.0f);

		m_Text.text = "2";
		yield return new WaitForSeconds(1.0f);

		m_Text.text = "1";
		yield return new WaitForSeconds(1.0f);

		m_Text.text = "GO!";
		yield return new WaitForSeconds(1.0f);

		m_Text.text = "";
		m_Text.gameObject.SetActive(false);
	}

	public void StartCountDown()
    {
		m_Text = GetComponent<Text>();
		StartCoroutine(CountdownCoroutine());
	}
}
