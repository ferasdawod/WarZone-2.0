using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject LoadingPanel;
    public Text LoadingText;

    public GameObject LogedInIndicator;
    public GameObject InRoomIndicator;

    private void Awake()
    {
        NetworkManager.Instance.OnLoginStart += () =>
        {
            LoadingText.text = "Attempting to login";
            LoadingPanel.gameObject.SetActive(true);
        };

        NetworkManager.Instance.OnLogin += () =>
        {
            LoadingPanel.gameObject.SetActive(false);
            LogedInIndicator.SetActive(true);
            LogedInIndicator.GetComponent<Image>().color = NetworkManager.Instance.IsLoggedIn ? Color.green : Color.red;
        };

        NetworkManager.Instance.OnJoinRoomStart += () =>
        {
            LoadingText.text = "Joining a room";
            LoadingPanel.gameObject.SetActive(true);
        };

        NetworkManager.Instance.OnRoomJoined += () =>
        {
            LoadingPanel.gameObject.SetActive(false);
            InRoomIndicator.SetActive(true);
            InRoomIndicator.GetComponent<Image>().color = PhotonNetwork.inRoom ? Color.green : Color.red;
        };
    }
}
