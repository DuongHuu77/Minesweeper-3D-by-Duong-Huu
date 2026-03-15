using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Bắt buộc phải có để dùng UI của TextMeshPro

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown ModeDropdown; // Kéo Dropdown "Mode" vào đây

    // Gán hàm này vào nút PLAY
    public void PlayGame()
    {
        // Kiểm tra xem người chơi đang chọn dòng thứ mấy trong Dropdown
        // 0: Beginner, 1: Intermediate, 2: Expert
        int selectedIndex = ModeDropdown.value;

        if (selectedIndex == 0)
            GameSettings.SelectedMode = GameSettings.Beginner;
        else if (selectedIndex == 1)
            GameSettings.SelectedMode = GameSettings.Intermediate;
        else if (selectedIndex == 2)
            GameSettings.SelectedMode = GameSettings.Expert;

        // Load Scene Game (Thay "SampleScene" bằng đúng tên Scene game của bạn nếu khác)
        SceneManager.LoadScene("Game"); 
    }

    // Gán hàm này vào nút QUIT
    public void QuitGame()
    {
        Debug.Log("QUIT GAME!");
        Application.Quit();
    }
}