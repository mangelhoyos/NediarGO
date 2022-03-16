using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Collections;

public class AuthenticationManager : MonoBehaviour
{
    //General Firebase information
    private DependencyStatus dependencyStatus;
    protected private FirebaseAuth auth;
    protected private FirebaseUser user;

    [Header("Login setup")]
    [SerializeField]private GameObject logInPanel;
    [SerializeField]private TMP_InputField emailLoginField;
    [SerializeField]private TMP_InputField passwordLoginField;
    [SerializeField]private TMP_Text warningLoginText;

    [Header("Register setup")]
    [SerializeField]private GameObject registerPanel;
    [SerializeField]private TMP_InputField usernameRegisterField;
    [SerializeField]private TMP_InputField emailRegisterField;
    [SerializeField]private TMP_InputField passwordRegisterField;
    [SerializeField]private TMP_InputField passwordRegisterVerifyField;
    [SerializeField]private TMP_Text warningRegisterText;


    private void Awake() //Checks for dependencies status
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if(dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Error resolving Firebase dependencies: "+ dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    /// <summary>
    /// Uses the email and password to try and log in into the Firebase database
    /// </summary>
    public void LogInFirebase()
    {
        StartCoroutine(LogIn(emailLoginField.text,passwordLoginField.text));
    }

    private IEnumerator LogIn(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email,password);

        yield return new WaitUntil(predicate: ()=> loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {loginTask.Exception}");
            FirebaseException exception = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)exception.ErrorCode;

            string message = "Login failed";
            switch(errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid email";
                    break; 
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;   
            }
            warningLoginText.text = message;
        }
        else
        {
            user = loginTask.Result;
            Debug.LogFormat("User signed successfuly: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "Logged in";
        }
    }

    /// <summary>
    /// Uses the email and password to try and register into the Firebase database
    /// </summary>
    public void RegisterFirebase()
    {
        StartCoroutine(Register(emailRegisterField.text,passwordRegisterField.text));
    }

    private IEnumerator Register(string email, string password)
    {
        if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            warningRegisterText.text = "Password does not match";
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email,password);
            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if(registerTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {registerTask.Exception}");
                FirebaseException exception = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)exception.ErrorCode;

                string message = "Register failed!";
                switch(errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Password is too weak";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                user = registerTask.Result;
                
                if(user != null)
                {
                    UserProfile profile = new UserProfile {DisplayName = (email.Split('@'))[0]};

                    var profileTask = user.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                    if(profileTask.Exception != null)
                    {
                        Debug.LogWarning($"Failed to register task with {profileTask.Exception}");
                        FirebaseException exception = profileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)exception.ErrorCode;
                        warningRegisterText.text = "Username set failed";
                    }
                    else
                    {
                        registerPanel.SetActive(false);
                        logInPanel.SetActive(true);
                    }
                }
            }
        }
    }
}
