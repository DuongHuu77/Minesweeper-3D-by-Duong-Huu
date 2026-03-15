using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam;
    public float distance = 5f;

    private Tile lastTile; // Biến lưu trữ ô gạch đang được highlight

    void Update()
    {
        if (GameManager.Instance.GameOver)
        {
            // Nếu game kết thúc, tắt highlight ô cuối cùng nếu có
            if (lastTile != null) lastTile.SetHighlight(false);
            return;
        }

        // Bắn Ray từ giữa màn hình (0.5, 0.5)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            Tile currentTile = hit.collider.GetComponent<Tile>();

            if (currentTile != null)
            {
                // KIỂM TRA HIGHLIGHT
                if (currentTile != lastTile)
                {
                    // Tắt highlight của ô cũ
                    if (lastTile != null) lastTile.SetHighlight(false);
                    
                    // Bật highlight của ô mới
                    currentTile.SetHighlight(true);
                    
                    // Gán ô hiện tại vào biến tạm
                    lastTile = currentTile;
                }

                // Click chuột trái để mở
                if (Input.GetMouseButtonDown(0))
                    currentTile.Reveal();

                // Click chuột phải để cắm cờ
                if (Input.GetMouseButtonDown(1))
                    currentTile.ToggleFlag();
            }
            else
            {
                // Nếu tia Ray chạm vật thể khác không phải Tile
                ClearHighlight();
            }
        }
        else
        {
            // Nếu tia Ray không chạm gì cả
            ClearHighlight();
        }
    }

    // Hàm phụ trợ để dọn dẹp highlight khi nhìn ra ngoài không trung
    private void ClearHighlight()
    {
        if (lastTile != null)
        {
            lastTile.SetHighlight(false);
            lastTile = null;
        }
    }
}