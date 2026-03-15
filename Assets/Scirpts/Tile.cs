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
    
    private GridManager grid;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (NumberText != null) NumberText.text = "";
    }

    public void Setup(int x, int y, GridManager g)
    {
        X = x;
        Y = y;
        grid = g;
        IsMine = false;
        Revealed = false;
        Flagged = false;
    }

    public void SetMine() => IsMine = true;

    public void SetHighlight(bool active)
    {
        if (Revealed) return;

        if (active)
            rend.material.color = Flagged ? Color.blue : Color.white;
        else
            rend.material.color = Flagged ? Color.blue : Color.gray; // Cờ nên màu Blue để tránh nhầm với Bom (Red)
    }

    public void Reveal()
    {
        if (Revealed || Flagged) return;

        // Phát nhấn đầu tiên kích hoạt rải bom
        if (!GameManager.Instance.FirstClick)
        {
            GameManager.Instance.FirstClick = true;
            grid.PlaceMinesSafe(X, Y); 
        }

        Revealed = true;

        if (IsMine)
        {
            rend.material.color = Color.red; // Bom nổ màu đỏ
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
        rend.material.color = Flagged ? Color.blue : Color.gray; 
    }

    // Cập nhật bảng màu từ 1 đến 8 cho chuyên nghiệp
    private Color GetNumberColor(int num)
    {
        switch (num)
        {
            case 1: return Color.blue;
            case 2: return new Color(0, 0.5f, 0); // Xanh lá
            case 3: return Color.red;
            case 4: return new Color(0, 0, 0.5f); // Xanh dương đậm
            case 5: return new Color(0.5f, 0, 0); // Đỏ đậm
            case 6: return new Color(0, 0.5f, 0.5f); // Cyan đậm
            case 7: return Color.black;
            case 8: return Color.gray;
            default: return Color.black;
        }
    }

    // Hàm này chỉ để hiện quả bom lên mà không chạy logic thua cuộc
    public void RevealAsMine(bool isWin)
    {
        if (IsMine && !Revealed)
        {
            Revealed = true;
            
            if (isWin)
            {
                // Nếu THẮNG: Đổi sang màu Xanh dương (màu cờ) và hiện chữ F
                rend.material.color = Color.blue;
            }
            else
            {
                // Nếu THUA: Đổi sang màu Đỏ và hiện chữ X
                rend.material.color = Color.red;
                if (NumberText != null) 
                {
                    NumberText.text = "X"; 
                    NumberText.color = Color.black;
                }
            }
        }
    }

    // Các hàm kiểm tra trạng thái cho Script khác dùng
    public bool IsRevealed() => Revealed;
    public bool IsFlagged() => Flagged;
    public Vector2 GridPosition => new Vector2(X, Y);
}