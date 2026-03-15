using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GridManager Grid;

    [Header("Cấu hình UI")]
    public GameObject GameOverPanel; // Panel khi thua
    public GameObject GameWinPanel;  // Panel khi thắng
    public GameObject Crosshair;     // Đối tượng tâm ngắm UI
    
    [Tooltip("Kéo RectTransform của Canvas chứa bảng Thua vào đây")]
    public RectTransform GameOverCanvasTransform; 
    [Tooltip("Kéo RectTransform của Canvas chứa bảng Thắng vào đây (nếu dùng chung thì kéo giống ô trên)")]
    public RectTransform WinCanvasTransform;

    [Header("Thực thể người chơi")]
    public PlayerController PlayerMovement;   
    public PlayerInteraction PlayerLook;       
    
    [Header("Cấu hình Camera")]
    public GameObject MainCamera;     // Camera FPV
    public GameObject GameOverCamera; // Camera góc rộng nhìn toàn cảnh

    public bool GameOver = false;
    public bool FirstClick = false;
    
    private GameSettings currentSettings;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        currentSettings = GameSettings.SelectedMode;
        StartGame(currentSettings);
        
        if(GameOverPanel != null) GameOverPanel.SetActive(false);
        if(GameWinPanel != null) GameWinPanel.SetActive(false);
        if(GameOverCamera != null) GameOverCamera.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartFromButton();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMenu();
        }
    }

    public void StartGame(GameSettings settings)
    {
        GameOver = false;
        FirstClick = false;
        currentSettings = GameSettings.SelectedMode;
        
        if(PlayerMovement != null) PlayerMovement.enabled = true;
        if(PlayerLook != null) PlayerLook.enabled = true;

        Grid.GenerateGrid(settings);
        
        if(GameOverPanel != null) GameOverPanel.SetActive(false);
        if(GameWinPanel != null) GameWinPanel.SetActive(false);
        if(Crosshair != null) Crosshair.SetActive(true);
        
        if(GameOverCamera != null) GameOverCamera.SetActive(false);
        if(MainCamera != null) MainCamera.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EndGame(bool win)
    {
        if (GameOver) return; 
        GameOver = true;

        StartCoroutine(GameEndRoutine(win));
    }

    IEnumerator GameEndRoutine(bool isWin)
    {
        // 1. Khóa điều khiển và ẩn tâm ngắm
        if(PlayerMovement != null) PlayerMovement.enabled = false;
        if(PlayerLook != null) PlayerLook.enabled = false;
        if(Crosshair != null) Crosshair.SetActive(false);

        // 2. Lộ diện toàn bộ bom
        if (Grid != null) Grid.RevealAllMines(isWin);

        // 3. Chờ ngắn cho hiệu ứng nổ/hiện bom
        yield return new WaitForSeconds(0.5f);

        // 4. Chuyển sang góc Camera quan sát toàn cảnh
        if (GameOverCamera != null && MainCamera != null)
        {
            MainCamera.SetActive(false);
            GameOverCamera.SetActive(true);

            // Cập nhật vị trí Canvas dựa trên kết quả Thắng/Thua
            RectTransform activeCanvas = isWin ? WinCanvasTransform : GameOverCanvasTransform;

            if (activeCanvas != null)
            {
                // Đưa Canvas về trước mặt Camera kết thúc (cách 2 đơn vị)
                activeCanvas.position = GameOverCamera.transform.position + GameOverCamera.transform.forward * 5f;
                // Xoay Canvas khớp với hướng nhìn của Camera
                activeCanvas.rotation = GameOverCamera.transform.rotation;
            }
        }

        // 5. Chờ thêm chút để người chơi quan sát bãi mìn
        yield return new WaitForSeconds(1.5f);

        // 6. Hiện bảng UI tương ứng
        if (isWin)
        {
            Debug.Log("<color=green>★★ YOU WIN! ★★</color>");
            if (GameWinPanel != null) GameWinPanel.SetActive(true);
        }
        else
        {
            Debug.Log("<color=red>XX GAME OVER XX</color>");
            if (GameOverPanel != null) GameOverPanel.SetActive(true);
        }

        // 7. Giải phóng chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartFromButton()
    {
        StartGame(currentSettings);
    }

    public void BackToMenu()
    {
        // Đảm bảo thời gian game không bị đóng băng nếu lỡ đang thua/thắng
        Time.timeScale = 1f; 

        // QUAN TRỌNG: Phải mở khóa và hiện lại con trỏ chuột. 
        // Nếu không có 2 dòng này, về Menu bạn sẽ không thấy chuột để bấm chữ PLAY.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load lại Scene Menu (Đảm bảo tên Scene trong ngoặc kép khớp với tên file Scene của bạn)
        SceneManager.LoadScene("Menu"); 
    }
}