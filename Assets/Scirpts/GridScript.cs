using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject TilePrefab;
    private Tile[,] grid;
    private GameSettings settings;

    public void GenerateGrid(GameSettings s)
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        settings = s;
        grid = new Tile[s.Width, s.Height];

        for(int x=0; x<s.Width; x++) {
            for(int y=0; y<s.Height; y++) {
                //GameObject obj = Instantiate(TilePrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                GameObject obj = Instantiate(TilePrefab, new Vector3(x - s.Width / 2f, 0, y - s.Height / 2f), Quaternion.identity, transform);
                Tile tile = obj.GetComponent<Tile>();
                tile.Setup(x, y, this);
                grid[x, y] = tile;
            }
        }
    }

    public void PlaceMinesSafe(int startX, int startY)
    {
        int placed = 0;
        while(placed < settings.Mines)
        {
            int x = Random.Range(0, settings.Width);
            int y = Random.Range(0, settings.Height);

            // Không đặt bom vào ô vừa click và không trùng vào ô đã có bom
            if((x != startX || y != startY) && !grid[x, y].IsMine)
            {
                grid[x, y].SetMine();
                placed++;
            }
        }
        // Rải xong bom rồi mới tính toán con số hiển thị
        CalculateNumbers();
    }

    void CalculateNumbers()
    {
        for(int x=0; x<settings.Width; x++) {
            for(int y=0; y<settings.Height; y++) {
                if(grid[x, y].IsMine) continue;

                int count = 0;
                // GetNeighbors trả về tối đa 8 ô xung quanh
                foreach(Tile n in GetNeighbors(x, y)) {
                    if(n.IsMine) count++;
                }
                grid[x, y].Number = count;
            }
        }
    }

    public List<Tile> GetNeighbors(int x, int y)
    {
        List<Tile> list = new List<Tile>();
        for(int dx=-1; dx<=1; dx++) {
            for(int dy=-1; dy<=1; dy++) {
                if(dx == 0 && dy == 0) continue;
                int nx = x + dx; int ny = y + dy;
                if(nx >= 0 && ny >= 0 && nx < settings.Width && ny < settings.Height)
                    list.Add(grid[nx, ny]);
            }
        }
        return list;
    }

    public void CheckWinCondition()
    {
        int safeTiles = (settings.Width * settings.Height) - settings.Mines;
        int revealed = 0;
        foreach (Tile t in grid) if (t.Revealed && !t.IsMine) revealed++;

        if (revealed >= safeTiles) GameManager.Instance.EndGame(true);
    }

    public void RevealAllMines(bool isWin)
    {
        if (grid == null) return;

        foreach (Tile t in grid)
        {
            if (t != null && t.IsMine)
            {
                // Truyền trạng thái Thắng/Thua vào từng ô gạch
                t.RevealAsMine(isWin);
            }
        }
    }
}