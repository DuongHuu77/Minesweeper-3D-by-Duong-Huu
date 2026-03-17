using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Tile : MonoBehaviour
{
    [Header("Thông tin tọa độ")]
    public int X;
    public int Y;

    [Header("Trạng thái")]
    public bool IsMine { get; private set; }
    public bool Revealed { get; private set; }
    public bool Flagged { get; private set; }
    public int Number;

    [Header("Cấu hình hiển thị")]
    public TextMeshPro NumberText; 
    
    [Header("Cấu hình Model 3D")]
    public GameObject FlagObject; // Model Cờ
    public GameObject MineObject; // Model Bom

    private GridManager grid;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (NumberText != null) NumberText.text = "";
    }

    public void Setup(int x, int y, GridManager g)
    {
        X = x; Y = y; grid = g;
        IsMine = false; Revealed = false; Flagged = false;

        // Tắt cả cờ và bom khi tạo map mới
        if (FlagObject != null) FlagObject.SetActive(false); 
        if (MineObject != null) MineObject.SetActive(false); 
    }

    public void SetMine() => IsMine = true;

    public void SetHighlight(bool active)
    {
        if (Revealed) return;
        if (active) rend.material.color = Color.white;
        else rend.material.color = Color.gray; 
    }

    public void Reveal()
    {
        if (Revealed || Flagged) return;

        if (!GameManager.Instance.FirstClick)
        {
            GameManager.Instance.FirstClick = true;
            grid.PlaceMinesSafe(X, Y); 
        }

        Revealed = true;

        if (IsMine)
        {
            // DẪM TRÚNG BẰNG CHÍNH ĐÔI CHÂN CỦA BẠN: Tô đỏ và hiện mìn
            rend.material.color = Color.red; 
            if (MineObject != null) MineObject.SetActive(true);
            if (FlagObject != null) FlagObject.SetActive(false);
            
            GameManager.Instance.EndGame(false);
            return;
        }

        rend.material.color = Color.white;

        if (Number > 0)
        {
            if (NumberText != null)
            {
                NumberText.text = Number.ToString();
                NumberText.color = GetNumberColor(Number);
            }
        }
        else 
        {
            foreach (Tile t in grid.GetNeighbors(X, Y)) t.Reveal();
        }

        grid.CheckWinCondition();
    }

    public void ToggleFlag()
    {
        if (Revealed) return; 
        
        Flagged = !Flagged;
        if (FlagObject != null) FlagObject.SetActive(Flagged);

        // Kích hoạt thay đổi số trên màn hình lớn (nếu có)
        if (BigScreenManager.Instance != null) BigScreenManager.Instance.AddFlag(Flagged);
    }

    private Color GetNumberColor(int num)
    {
        switch (num) {
            case 1: return Color.blue;
            case 2: return new Color(0, 0.5f, 0); 
            case 3: return Color.red;
            case 4: return new Color(0, 0, 0.5f); 
            case 5: return new Color(0.5f, 0, 0); 
            case 6: return new Color(0, 0.5f, 0.5f); 
            case 7: return Color.black;
            case 8: return Color.gray;
            default: return Color.black;
        }
    }

    // ĐỔI TÊN HÀM: Xử lý trạng thái khi Game Over
    public void HandleGameOver(bool isWin)
    {
        if (isWin)
        {
            // THẮNG: Lấp đầy cờ vào các ô bom chưa mở
            if (IsMine && !Revealed)
            {
                Revealed = true;
                if (FlagObject != null) FlagObject.SetActive(true);
            }
        }
        else
        {
            // THUA: Có 3 trường hợp
            if (IsMine && !Flagged && !Revealed)
            {
                // 1. Ô có bom mà chưa tìm ra -> Hiện bom lên NHƯNG KHÔNG NỔ
                Revealed = true;
                if (MineObject != null) 
                {
                    // Lấy script Explode (thuộc namespace Novasloth) và tắt nó đi
                    Novasloth.Explode explodeScript = MineObject.GetComponent<Novasloth.Explode>();
                    if (explodeScript != null)
                    {
                        explodeScript.enabled = false; // Tắt ngòi nổ
                    }
                    
                    MineObject.SetActive(true); // Chỉ hiện mô hình 3D lên
                }
            }
            else if (!IsMine && Flagged)
            {
                // 2. KHÔNG có bom nhưng lại cắm cờ -> CẮM SAI
                rend.material.color = Color.red;
            }
            // 3. Có bom và cắm đúng cờ -> Cờ giữ nguyên, không làm gì cả
        }
    }

    public bool IsRevealed() => Revealed;
    public bool IsFlagged() => Flagged;
    public Vector2 GridPosition => new Vector2(X, Y);
}