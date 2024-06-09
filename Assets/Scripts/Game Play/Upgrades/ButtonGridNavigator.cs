using UnityEngine;
using UnityEngine.UI;

public class ButtonGridNavigator : MonoBehaviour
{
    public UpgradeButton[] upgradeButtons; // An array of upgrade buttons
    private const int Columns = 2; // Number of columns in the grid
    private int highlightedButtonIndex = -1; // -1 indicates no button is highlighted
    private Vector3 lastMousePosition; // To track mouse position
    private float lastJoystickVerticalInput; 
    private float lastJoystickHorizontalInput;
    private bool joystickInputUsed = false;
    private float inputCooldown = 0.1f; // Small cooldown to prevent input overlap
    private float lastInputTime;

    private void Update()
    {
        // Check for mouse movement
        if (Input.mousePosition != lastMousePosition)
        {
            lastMousePosition = Input.mousePosition;
            ResetHighlightedButton();
        }

        // Process input based on the most recently used input method
        if (Time.time > lastInputTime + inputCooldown)
        {
            if (joystickInputUsed)
            {
                ProcessJoystickInput();
                if (IsKeyboardInputUsed())
                {
                    joystickInputUsed = false;
                }
            }
            else
            {
                ProcessKeyboardInput();
                if (IsJoystickInputUsed())
                {
                    joystickInputUsed = true;
                }
            }
        }

        // Enter - Activate the onClick event of the highlighted button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Submit"))
        {
            ActivateHighlightedButton();
        }
    }

    private bool IsKeyboardInputUsed()
    {
        return Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) ||
               Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W) ||
               Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D);
    }

    private bool IsJoystickInputUsed()
    {
        return Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.2f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.2f;
    }

    private void ProcessKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            NavigateVertical(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            NavigateVertical(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            NavigateHorizontal(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            NavigateHorizontal(-1);
        }
    }

    private void ProcessJoystickInput()
    {
        float joystickVerticalInput = Input.GetAxisRaw("Vertical");
        float joystickHorizontalInput = Input.GetAxisRaw("Horizontal");
        float threshold = 0.2f; // Adjusted threshold

        if (Mathf.Abs(joystickVerticalInput) > threshold && Mathf.Abs(lastJoystickVerticalInput) <= threshold)
        {
            NavigateVertical(joystickVerticalInput < 0 ? 1 : -1);
        }
        else if (Mathf.Abs(joystickHorizontalInput) > threshold && Mathf.Abs(lastJoystickHorizontalInput) <= threshold)
        {
            NavigateHorizontal(joystickHorizontalInput > 0 ? 1 : -1);
        }

        lastJoystickVerticalInput = joystickVerticalInput;
        lastJoystickHorizontalInput = joystickHorizontalInput;
    }

    private void ActivateHighlightedButton()
    {
        if (highlightedButtonIndex >= 0 && highlightedButtonIndex < upgradeButtons.Length)
        {
            upgradeButtons[highlightedButtonIndex].OnUpgradeButtonClicked();
        }
    }

    private void NavigateVertical(int step)
    {
        // Special handling when starting from -1
        if (highlightedButtonIndex == -1 && step > 0)
        {
            // Find the first interactable button from the top
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                if (upgradeButtons[i].upgradeButton.interactable)
                {
                    ChangeHighlightedButton(i);
                    return;
                }
            }
        }

        // Special handling for moving up from the Continue Game button
        if (highlightedButtonIndex == upgradeButtons.Length - 1 && step < 0)
        {
            // Determine which column to check based on the disabled state of the left column
            bool leftColumnDisabled = true;
            for (int i = 0; i < upgradeButtons.Length - Columns; i += Columns)
            {
                if (upgradeButtons[i].upgradeButton.interactable)
                {
                    leftColumnDisabled = false;
                    break;
                }
            }

            int preferredColumn = leftColumnDisabled ? 1 : 0;

            // Find the first interactable button in the preferred column, starting from the bottom
            for (int i = upgradeButtons.Length - Columns - 1 + preferredColumn; i >= 0; i -= Columns)
            {
                if (upgradeButtons[i].upgradeButton.interactable)
                {
                    ChangeHighlightedButton(i);
                    return;
                }
            }
        }

        // Calculate the next row index
        int currentRow = highlightedButtonIndex / Columns;
        int newRow = currentRow + step;

        while (newRow >= 0 && newRow <= (upgradeButtons.Length - 1) / Columns)
        {
            int col = highlightedButtonIndex % Columns;
            int newIndex = newRow * Columns + col;

            // Check for bounds and interactability
            if (newIndex >= 0 && newIndex < upgradeButtons.Length && 
                upgradeButtons[newIndex].upgradeButton.interactable)
            {
                ChangeHighlightedButton(newIndex);
                return;
            }

            // Special handling for reaching the Continue Game button
            if (newIndex >= upgradeButtons.Length - Columns && step > 0)
            {
                newIndex = upgradeButtons.Length - 1; // Index of Continue Game button
                if (upgradeButtons[newIndex].upgradeButton.interactable)
                {
                    ChangeHighlightedButton(newIndex);
                    return;
                }
            }

            newRow += step; // Move to the next/previous row
        }
    }

    private void NavigateHorizontal(int step)
    {
        if (highlightedButtonIndex == -1)
        {
            return; // Exit if no button is currently highlighted
        }

        int currentRow = highlightedButtonIndex / Columns;
        int currentColumn = highlightedButtonIndex % Columns;
        int newColumn = currentColumn + step;

        if (newColumn < 0 || newColumn >= Columns)
        {
            return; // Exit if the new column is outside the grid bounds
        }

        // Try to find the next active button in the new column
        for (int row = 0; row < (upgradeButtons.Length + Columns - 1) / Columns; row++)
        {
            int newIndex = row * Columns + newColumn;
            if (newIndex < upgradeButtons.Length && upgradeButtons[newIndex].upgradeButton.interactable)
            {
                ChangeHighlightedButton(newIndex);
                return;
            }
        }
        // If no active button is found in the new column, do not change the highlighted button
    }

    private void ResetHighlightedButton()
    {
        if (highlightedButtonIndex >= 0)
        {
            HighlightButton(upgradeButtons[highlightedButtonIndex], false);
            highlightedButtonIndex = -1;
        }
    }

    private void ChangeHighlightedButton(int newIndex)
    {
        if (upgradeButtons[newIndex].upgradeButton.interactable)
        {
            if (highlightedButtonIndex >= 0) // Unhighlight the previous button
            {
                HighlightButton(upgradeButtons[highlightedButtonIndex], false);
            }

            highlightedButtonIndex = newIndex; // Update the index
            HighlightButton(upgradeButtons[highlightedButtonIndex], true); // Highlight the new button
        }
    }

    private void HighlightButton(UpgradeButton button, bool highlight)
    {
        ColorBlock colors = button.upgradeButton.colors;
        colors.normalColor = highlight ? colors.highlightedColor : button.originalNormalColor;
        button.upgradeButton.colors = colors;
    }
}
