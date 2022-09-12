using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoogleCloud : MonoBehaviour
{
	public static GoogleCloud instance;

	[SerializeField] private TextMeshProUGUI testText;
	[SerializeField] private InputField inputField;
	[SerializeField] private Button saveButton;


	private void Awake()
    {
		instance = this;

	}

	public void SaveUserDataWithCloud(UserData userData)
	{
		string serializedData = JsonConvert.SerializeObject(userData); // �����͸� �����ϱ� ���� ����ȭ�մϴ�.
		GPGSManager.Instance.SaveWithCloud("USERDATA", serializedData, (success) =>
		{ // �����͸� Ŭ���忡 �����մϴ�.
			if (success)
			{
				// ������ ���� ������

				print("���� ���� ���� ����");

			}
			else
			{
				// ������ ���� ���н�

				print("���� ���� �ҷ����� ����");
			}

		});
	}

	public UserData LoadUserDataWithCloud()
	{
		UserData userData = null;
		GPGSManager.Instance.LoadWithCloud("USERDATA", (success, serializedData) => { // �����͸� Ŭ���忡�� �ҷ��ɴϴ�.
			if (success)
			{
				// ������ �ε� ������
				print("���� ���� �ҷ����� ����");
				
				userData = JsonConvert.DeserializeObject<UserData>(serializedData); // �ҷ��� �����͸� ������ȭ�մϴ�.

				print(userData.testString);

				UserDataManager.instance.currentUserData = userData;

				SceneManager.LoadScene("MainScene");
			}
			else
			{
				print("���� ���� �ҷ����� ����");
				userData.testString = "�ҷ����� ����";
				// ������ �ε� ���н�
			}
		});

		return userData;
	}

	public void DelectWithCloud(string dataKey)
	{
		GPGSManager.Instance.DelectWithCloud(dataKey, (success) => { // �����͸� Ŭ���忡�� �����մϴ�..
			if (success)
			{
				// ������ ���� ������
			}
			else
			{
				// ������ ���� ���н�
			}
		});
	}



	//================================================================================================================

	public void TestUserDataSave()
    {
		UserData userData = new UserData();

		userData.testString = inputField.text;

		SaveUserDataWithCloud(userData);
    }

	public void TestUserDataLoad()
    {
		UserData userData = LoadUserDataWithCloud();

		print(userData.testString);

		testText.text = userData.testString;
    }
}
