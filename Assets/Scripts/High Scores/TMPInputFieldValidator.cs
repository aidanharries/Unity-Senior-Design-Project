using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class TMPInputFieldValidator : MonoBehaviour
{
    public delegate void SubmitAction(string input);
    public event SubmitAction OnSubmit;
    public AudioClip select;
    private AudioSource audioSource;
    private TMP_InputField inputField;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.Select();
        inputField.ActivateInputField();
        inputField.onValueChanged.AddListener(HandleInputChanged);
        inputField.onEndEdit.AddListener(HandleSubmit);
        audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource
    }

    private void HandleInputChanged(string input)
    {
        input = input.ToUpper(); // Convert to uppercase
        input = System.Text.RegularExpressions.Regex.Replace(input, "[^A-Z]", ""); // Remove non-letter characters
        inputField.text = input;
    }

    private void HandleSubmit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            audioSource.PlayOneShot(select);
            OnSubmit?.Invoke(input);
        }
    }
}
