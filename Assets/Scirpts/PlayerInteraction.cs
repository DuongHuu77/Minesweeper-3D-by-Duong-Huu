using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Camera cam;
    public float distance = 5f;

    private Tile lastTile; 

    [Header("Hoạt ảnh")]
    public Animator anim;

    void Start()
    {
        // Tự động tìm Animator giống file Controller (Không cần kéo thả ngoài Inspector)
        //anim = GetComponent<Animator>(); 
    }

    void Update()
    {
        if (GameManager.Instance.GameOver)
        {
            ClearHighlight();
            return;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            Tile currentTile = hit.collider.GetComponent<Tile>();

            if (currentTile != null)
            {
                // ---- PHẦN 1: XỬ LÝ HIGHLIGHT (Đồ họa) ----
                if (currentTile != lastTile)
                {
                    ClearHighlight(); // Tắt ô cũ
                    currentTile.SetHighlight(true); // Bật ô mới
                    lastTile = currentTile; // Cập nhật lại biến tạm
                }

                // ---- PHẦN 2: XỬ LÝ TƯƠNG TÁC (Đứng độc lập bên ngoài) ----
                
                // Click trái để Mở
                if (Input.GetMouseButtonDown(0))
                {
                    if (anim != null) anim.SetTrigger("Mine"); // Bạn đổi tên Trigger thành "Mine" rồi đúng không?
                    currentTile.Reveal();
                }

                // Click phải để Cắm cờ
                if (Input.GetMouseButtonDown(1))
                {
                    currentTile.ToggleFlag();
                }
            }
            else
            {
                ClearHighlight(); // Nhìn trúng cục đá, bờ tường... thì tắt highlight
            }
        }
        else
        {
            ClearHighlight(); // Nhìn lên trời thì tắt highlight
        }
    }

    // Viết lại hàm này cho gọn, dùng lại được nhiều chỗ
    private void ClearHighlight()
    {
        if (lastTile != null)
        {
            lastTile.SetHighlight(false);
            lastTile = null;
        }
    }
}