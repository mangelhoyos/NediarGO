using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("User data")]
    [SerializeField] TMP_Text userName;

    void Awake()
    {
        GetAuthenticationData();
    }

    void GetAuthenticationData()
    {
        AuthenticationManager auth = GameObject.FindObjectOfType<AuthenticationManager>();
        Debug.Log(auth);
        if(auth != null)
        {
            userName.text = (auth.GetUserData().Email.Split('@'))[0];
            Debug.Log(auth.GetUserData().DisplayName);
        }
    }
}
