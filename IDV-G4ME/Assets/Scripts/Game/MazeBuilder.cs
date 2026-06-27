using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Génère un labyrinthe parfait via DFS (backtracking) et pose des cubes-murs sur une grille.
/// Entrée ouverte au sud (centre), sortie au nord (centre).
/// Pensé pour fonctionner avec un CharacterController (BoxCollider sur les murs).
/// </summary>
[ExecuteAlways]
public class MazeBuilder : MonoBehaviour
{
    [Header("Grille")]
    [Min(3)] public int rows = 9;   // nombre de cases en Z (je garde un impair pour un rendu plus propre)
    [Min(3)] public int cols = 9;   // nombre de cases en X (idem)
    public float cellSize = 2f;

    [Header("Murs")]
    public float wallThickness = 0.3f;
    public float wallHeight = 3f;
    public Material wallMaterial;
    [Tooltip("Tiling de la texture (pour éviter l'étirement). X=horizontal, Y=vertical")]
    public Vector2 textureTiling = new Vector2(1f, 1f);

    [Header("Génération")]
    public int seed = -1;           // <0 => aléatoire à chaque Play, >=0 => résultat reproductible
    public bool generateOnStart = true;

    // Données internes
    private System.Random rng;
    private bool[,] visited;
    private bool[,,] walls; // [r,c,4] 0=N, 1=E, 2=S, 3=W (true = mur présent)

    void Start()
    {
        if (Application.isPlaying && generateOnStart)
            Generate();
    }

    [ContextMenu("Generate Now")]
    public void Generate()
    {
        if (rows < 3) rows = 3;
        if (cols < 3) cols = 3;

        // Supprime toute génération précédente (enfants sous ce GameObject)
        ClearChildren();

        rng = (seed >= 0) ? new System.Random(seed) : new System.Random();

        // Initialise : toutes les cases non visitées et tous les murs fermés
        visited = new bool[rows, cols];
        walls = new bool[rows, cols, 4];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                for (int d = 0; d < 4; d++)
                    walls[r, c, d] = true;

        // Lancement du DFS depuis une case choisie aléatoirement
        int sr = rng.Next(rows), sc = rng.Next(cols);
        Carve(sr, sc);

        // Ouvre l'entrée (sud, au centre) et la sortie (nord, au centre)
        int cMid = cols / 2;
        walls[0, cMid, 2] = false;          // côté Sud de la première rangée
        walls[rows - 1, cMid, 0] = false;   // côté Nord de la dernière rangée

        // Instancie les meshes/Colliders des murs
        BuildVisuals();
    }

    void Carve(int r, int c)
    {
        visited[r, c] = true;

        // Parcours des directions dans un ordre aléatoire
        var dirs = new List<int> { 0, 1, 2, 3 };
        Shuffle(dirs);

        foreach (int dir in dirs)
        {
            int nr = r, nc = c;
            switch (dir)
            {
                case 0: nr = r + 1; nc = c; break; // Nord
                case 1: nr = r; nc = c + 1; break; // Est
                case 2: nr = r - 1; nc = c; break; // Sud
                case 3: nr = r; nc = c - 1; break; // Ouest
            }

            // Ignore si on sort de la grille ou si la case voisine a déjà été visitée
            if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
            if (visited[nr, nc]) continue;

            // Ouvre les deux murs entre (r,c) et (nr,nc)
            walls[r, c, dir] = false;
            int opp = (dir + 2) % 4;
            walls[nr, nc, opp] = false;

            Carve(nr, nc);
        }
    }

    void BuildVisuals()
    {
        // Origine au coin sud-ouest, posé au sol
        float totalW = cols * cellSize;
        float totalH = rows * cellSize;
        Vector3 origin = transform.position + new Vector3(-totalW / 2f, 0f, -totalH / 2f);
        float y = wallHeight / 2f;

        // Petit helper pour créer un mur cube correctement configuré
        GameObject MakeWall(Vector3 pos, Vector3 scale, string name)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(transform, true);
            go.transform.position = pos;
            go.transform.localScale = scale;

            var mr = go.GetComponent<MeshRenderer>();
            if (wallMaterial) mr.sharedMaterial = wallMaterial;

            // Ajuste le tiling de la texture pour éviter l'étirement
            if (wallMaterial && textureTiling != Vector2.one)
            {
                // Crée une copie du matériau pour ce mur spécifique
                Material mat = new Material(wallMaterial);
                mat.mainTextureScale = textureTiling;
                mr.sharedMaterial = mat;
            }

            // BoxCollider déjà présent suite à CreatePrimitive
            go.isStatic = true;
            return go;
        }

        // Je ne dessine que Nord et Ouest pour éviter les doublons entre cases
        // + je ferme les bords Est/Sud pour compléter le périmètre
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 cellCenter = origin + new Vector3((c + 0.5f) * cellSize, 0f, (r + 0.5f) * cellSize);

                // Mur Nord (entre cette case et la suivante au nord)
                if (walls[r, c, 0])
                {
                    Vector3 pos = cellCenter + new Vector3(0f, y, +cellSize / 2f);
                    Vector3 scl = new Vector3(cellSize, wallHeight, wallThickness);
                    MakeWall(pos, scl, $"W_N_{r}_{c}");
                }

                // Mur Ouest (entre cette case et la suivante à l’ouest)
                if (walls[r, c, 3])
                {
                    Vector3 pos = cellCenter + new Vector3(-cellSize / 2f, y, 0f);
                    Vector3 scl = new Vector3(wallThickness, wallHeight, cellSize);
                    MakeWall(pos, scl, $"W_W_{r}_{c}");
                }

                // Bords Est et Sud (uniquement sur les limites pour fermer le cadre)
                if (c == cols - 1 && walls[r, c, 1]) // Est
                {
                    Vector3 pos = cellCenter + new Vector3(+cellSize / 2f, y, 0f);
                    Vector3 scl = new Vector3(wallThickness, wallHeight, cellSize);
                    MakeWall(pos, scl, $"W_E_{r}_{c}");
                }
                if (r == 0 && walls[r, c, 2]) // Sud
                {
                    Vector3 pos = cellCenter + new Vector3(0f, y, -cellSize / 2f);
                    Vector3 scl = new Vector3(cellSize, wallHeight, wallThickness);
                    MakeWall(pos, scl, $"W_S_{r}_{c}");
                }
            }
        }
    }

    void Shuffle(List<int> list)
    {
        // Fisher–Yates basé sur mon RNG
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void ClearChildren()
    {
        // En mode Éditeur, DestroyImmediate évite les références fantômes
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var ch = transform.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(ch.gameObject);
            else Destroy(ch.gameObject);
#else
            Destroy(ch.gameObject);
#endif
        }
    }
}
