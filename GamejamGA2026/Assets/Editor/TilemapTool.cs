// ============================================================
//  TilemapTool.cs  —  à placer dans un dossier Editor/
//  ex: Assets/Editor/TilemapTool.cs
// ============================================================
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilemapTool : EditorWindow
{
    // ── Layers ──────────────────────────────────────────────
    [System.Serializable]
    public class TileLayer
    {
        public string name = "New Layer";
        public int    yLevel = 0;          // hauteur Y en mètres
        public Color  color  = Color.white;
    }

    List<TileLayer> layers = new List<TileLayer>
    {
        new TileLayer { name = "Sol",      yLevel = 0, color = new Color(0.4f, 0.8f, 0.4f) },
        new TileLayer { name = "Mur",      yLevel = 0, color = new Color(0.6f, 0.6f, 0.9f) },
        new TileLayer { name = "1er étage",yLevel = 3, color = new Color(0.9f, 0.7f, 0.3f) },
    };

    // ── État ────────────────────────────────────────────────
    enum ToolMode { Paint, Erase }
    ToolMode    mode           = ToolMode.Paint;
    int         activeLayer    = 0;
    int         selectedPrefab = 0;
    bool        isActive       = false;

    // ── Prefabs ─────────────────────────────────────────────
    List<GameObject> prefabs = new List<GameObject>();

    // ── Apparence ───────────────────────────────────────────
    Vector2     scrollPrefabs;
    Vector2     scrollLayers;
    GameObject  ghostInstance;          // preview fantôme
    Vector3Int  lastCell = Vector3Int.one * int.MinValue;

    // ── Styles ──────────────────────────────────────────────
    GUIStyle styleHeader;
    GUIStyle styleModeBtn;
    GUIStyle styleActiveBtn;
    bool     stylesInit = false;

    // ────────────────────────────────────────────────────────
    [MenuItem("Tools/Tilemap Tool 3D")]
    public static void Open() => GetWindow<TilemapTool>("🧱 Tilemap Tool");

    // ────────────────────────────────────────────────────────
    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        DestroyGhost();
        isActive = false;
    }

    // ════════════════════════════════════════════════════════
    //  INTERFACE EDITORWINDOW
    // ════════════════════════════════════════════════════════
    void OnGUI()
    {
        InitStyles();

        // ── Header ──
        EditorGUILayout.Space(6);
        GUILayout.Label("🧱  TILEMAP TOOL 3D", styleHeader);
        EditorGUILayout.Space(4);

        // ── Bouton Activer ──
        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = isActive ? new Color(0.4f, 1f, 0.5f) : new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button(isActive ? "● ACTIF — cliquez pour désactiver" : "○ INACTIF — cliquez pour activer",
                             GUILayout.Height(30)))
        {
            isActive = !isActive;
            if (!isActive) DestroyGhost();
            SceneView.RepaintAll();
        }
        GUI.backgroundColor = prev;

        EditorGUILayout.Space(8);
        DrawSeparator();

        // ── Mode Paint / Erase ──
        GUILayout.Label("MODE", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        DrawModeButton(ToolMode.Paint, "🖌  Poser");
        DrawModeButton(ToolMode.Erase, "🗑  Effacer");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);
        DrawSeparator();

        // ── Layers ──
        DrawLayersSection();

        DrawSeparator();

        // ── Prefabs ──
        if (mode == ToolMode.Paint)
            DrawPrefabsSection();

        DrawSeparator();

        // ── Infos ──
        EditorGUILayout.HelpBox(
            "LMB → Poser/Effacer\n" +
            "Shift+LMB → action continue (drag)\n" +
            "Esc → désactiver le tool",
            MessageType.Info);
    }

    // ────────────────────────────────────────────────────────
    void DrawLayersSection()
    {
        EditorGUILayout.LabelField("LAYERS", EditorStyles.boldLabel);
        scrollLayers = EditorGUILayout.BeginScrollView(scrollLayers, GUILayout.MaxHeight(160));

        for (int i = 0; i < layers.Count; i++)
        {
            var l = layers[i];
            bool isSelected = (i == activeLayer);

            Color bg = GUI.backgroundColor;
            GUI.backgroundColor = isSelected
                ? Color.Lerp(l.color, Color.white, 0.3f)
                : new Color(0.25f, 0.25f, 0.25f);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            // Sélection
            if (GUILayout.Button(isSelected ? "▶" : "  ", GUILayout.Width(24)))
                SelectLayer(i);

            // Nom
            l.name = EditorGUILayout.TextField(l.name, GUILayout.Width(100));

            // Y Level
            GUILayout.Label("Y:", GUILayout.Width(16));
            l.yLevel = EditorGUILayout.IntField(l.yLevel, GUILayout.Width(36));

            // Couleur
            l.color = EditorGUILayout.ColorField(l.color, GUILayout.Width(40));

            // Supprimer
            GUI.backgroundColor = new Color(0.9f, 0.3f, 0.3f);
            if (layers.Count > 1 && GUILayout.Button("✕", GUILayout.Width(22)))
            {
                layers.RemoveAt(i);
                if (activeLayer >= layers.Count) activeLayer = layers.Count - 1;
                GUI.backgroundColor = bg;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }

            GUI.backgroundColor = bg;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("+ Ajouter un layer"))
            layers.Add(new TileLayer
            {
                name   = "Layer " + layers.Count,
                yLevel = layers.Count * 3,
                color  = Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.7f, 1f)
            });
    }

    // ────────────────────────────────────────────────────────
    void DrawPrefabsSection()
    {
        EditorGUILayout.LabelField("PREFABS", EditorStyles.boldLabel);

        // Drop zone
        Rect dropRect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
        GUI.Box(dropRect, "⬇  Glissez des Prefabs ici");

        HandlePrefabDrop(dropRect);

        if (prefabs.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucun prefab. Glissez-en depuis le Project.", MessageType.Warning);
            return;
        }

        scrollPrefabs = EditorGUILayout.BeginScrollView(scrollPrefabs, GUILayout.MaxHeight(120));
        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < prefabs.Count; i++)
        {
            if (prefabs[i] == null) continue;

            bool sel = (i == selectedPrefab);
            Color bg = GUI.backgroundColor;
            GUI.backgroundColor = sel ? new Color(0.4f, 0.8f, 1f) : new Color(0.3f, 0.3f, 0.3f);

            EditorGUILayout.BeginVertical(GUILayout.Width(64));

            Texture2D preview = AssetPreview.GetAssetPreview(prefabs[i]);
            if (GUILayout.Button(preview != null ? (GUIContent)new GUIContent(preview)
                                                  : new GUIContent(prefabs[i].name),
                                 GUILayout.Width(60), GUILayout.Height(60)))
            {
                selectedPrefab = i;
                DestroyGhost();
            }

            GUILayout.Label(prefabs[i].name, EditorStyles.miniLabel, GUILayout.Width(60));
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = bg;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Vider la liste"))
        {
            prefabs.Clear();
            selectedPrefab = 0;
            DestroyGhost();
        }
    }

    // ────────────────────────────────────────────────────────
    void HandlePrefabDrop(Rect zone)
    {
        Event e = Event.current;
        if (!zone.Contains(e.mousePosition)) return;

        if (e.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            e.Use();
        }
        else if (e.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();
            foreach (Object obj in DragAndDrop.objectReferences)
            {
                if (obj is GameObject go && !prefabs.Contains(go))
                    prefabs.Add(go);
            }
            e.Use();
        }
    }

    // ════════════════════════════════════════════════════════
    //  SCENE VIEW
    // ════════════════════════════════════════════════════════
    void OnSceneGUI(SceneView sv)
    {
        if (!isActive) return;

        Event e = Event.current;

        // Esc = désactiver
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            isActive = false;
            DestroyGhost();
            Repaint();
            e.Use();
            return;
        }

        // Bloquer la sélection Unity pendant que le tool est actif
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Vector3? hitPoint = RaycastGrid(e.mousePosition, sv.camera);
        if (hitPoint == null)
        {
            DestroyGhost();
            return;
        }

        Vector3Int cell = WorldToCell(hitPoint.Value);

        // ── Preview fantôme ──
        if (mode == ToolMode.Paint)
            UpdateGhost(cell);
        else
            DestroyGhost();

        // ── Dessin grille ──
        DrawGridPreview(cell);

        // ── Action souris ──
        bool doAction = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                        && e.button == 0;

        if (doAction)
        {
            if (mode == ToolMode.Paint)   PlaceTile(cell);
            else                           EraseTile(cell);
            e.Use();
        }

        sv.Repaint();
    }

    // ────────────────────────────────────────────────────────
    Vector3? RaycastGrid(Vector2 mousePos, Camera cam)
    {
        if (activeLayer < 0 || activeLayer >= layers.Count) return null;

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        float yPlane = layers[activeLayer].yLevel;

        // Intersection rayon / plan horizontal Y = yPlane
        if (Mathf.Abs(ray.direction.y) < 0.001f) return null;
        float t = (yPlane - ray.origin.y) / ray.direction.y;
        if (t < 0) return null;

        return ray.origin + ray.direction * t;
    }

    // ────────────────────────────────────────────────────────
    Vector3Int WorldToCell(Vector3 world)
    {
        return new Vector3Int(
            Mathf.RoundToInt(world.x),
            layers[activeLayer].yLevel,
            Mathf.RoundToInt(world.z)
        );
    }

    // X et Z sont centrés sur l'entier (0.0, 1.0, 2.0...) ce qui est correct
    Vector3 CellToWorld(Vector3Int cell) => new Vector3(cell.x, cell.y, cell.z);

    // ────────────────────────────────────────────────────────
    void PlaceTile(Vector3Int cell)
    {
        if (prefabs.Count == 0 || selectedPrefab >= prefabs.Count) return;

        Vector3 pos = CellToWorld(cell);

        // Évite les doublons
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.4f);
        foreach (var h in hits)
        {
            if (Vector3Int.RoundToInt(h.transform.position) == cell)
                return;
        }

        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[selectedPrefab]);
        go.transform.position = pos;
        go.name = $"{prefabs[selectedPrefab].name}_{cell.x}_{cell.y}_{cell.z}";

        // Parente sous un GameObject nommé par layer
        go.transform.SetParent(GetLayerParent().transform);

        Undo.RegisterCreatedObjectUndo(go, "Place Tile");
    }

    void EraseTile(Vector3Int cell)
    {
        Vector3 pos = CellToWorld(cell); // Y déjà à yLevel + 0.5
        Collider[] hits = Physics.OverlapBox(pos, Vector3.one * 0.4f);
        foreach (var h in hits)
        {
            Vector3Int hCell = new Vector3Int(
                Mathf.RoundToInt(h.transform.position.x),
                cell.y,
                Mathf.RoundToInt(h.transform.position.z)
            );
            if (hCell == cell)
            {
                Undo.DestroyObjectImmediate(h.gameObject);
                return;
            }
        }
    }

    // ────────────────────────────────────────────────────────
    GameObject GetLayerParent()
    {
        string name = $"[TileLayer] {layers[activeLayer].name}";
        GameObject existing = GameObject.Find(name);
        if (existing != null) return existing;

        GameObject parent = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(parent, "Create Layer Parent");
        return parent;
    }

    // ────────────────────────────────────────────────────────
    void UpdateGhost(Vector3Int cell)
    {
        if (prefabs.Count == 0 || selectedPrefab >= prefabs.Count)
        {
            DestroyGhost();
            return;
        }

        if (ghostInstance == null || lastCell != cell)
        {
            DestroyGhost();
            ghostInstance = Instantiate(prefabs[selectedPrefab], CellToWorld(cell), Quaternion.identity);
            ghostInstance.name = "__TileGhost__";

            // Rend le fantôme semi-transparent
            foreach (var r in ghostInstance.GetComponentsInChildren<Renderer>())
            {
                var mats = r.sharedMaterials;
                var newMats = new Material[mats.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = new Material(mats[i]);
                    m.color = new Color(m.color.r, m.color.g, m.color.b, 0.4f);
                    newMats[i] = m;
                }
                r.materials = newMats;
            }

            // Désactive les colliders du fantôme
            foreach (var c in ghostInstance.GetComponentsInChildren<Collider>())
                c.enabled = false;

            lastCell = cell;
        }
    }

    void DestroyGhost()
    {
        if (ghostInstance != null)
            DestroyImmediate(ghostInstance);
        ghostInstance = null;
    }

    // ────────────────────────────────────────────────────────
    void DrawGridPreview(Vector3Int center)
    {
        if (activeLayer < 0 || activeLayer >= layers.Count) return;

        Color layerColor = layers[activeLayer].color;
        int   yLevel     = layers[activeLayer].yLevel;
        int   range      = 10;

        Handles.color = new Color(layerColor.r, layerColor.g, layerColor.b, 0.25f);

        for (int x = center.x - range; x <= center.x + range + 1; x++)
        {
            Handles.DrawLine(
                new Vector3(x, yLevel + 0.01f, center.z - range),
                new Vector3(x, yLevel + 0.01f, center.z + range + 1)
            );
        }
        for (int z = center.z - range; z <= center.z + range + 1; z++)
        {
            Handles.DrawLine(
                new Vector3(center.x - range,     yLevel + 0.01f, z),
                new Vector3(center.x + range + 1, yLevel + 0.01f, z)
            );
        }

        // Cellule survolée
        Handles.color = mode == ToolMode.Erase
            ? new Color(1f, 0.3f, 0.3f, 0.8f)
            : new Color(layerColor.r, layerColor.g, layerColor.b, 0.9f);

        Vector3 p = CellToWorld(center);
        Handles.DrawWireCube(p + Vector3.up * 0.5f, Vector3.one);
    }

    // ════════════════════════════════════════════════════════
    //  HELPERS UI
    // ════════════════════════════════════════════════════════
    void SelectLayer(int i)
    {
        activeLayer = i;
        DestroyGhost();
        SceneView.RepaintAll();
    }

    void DrawModeButton(ToolMode m, string label)
    {
        bool sel = mode == m;
        Color bg = GUI.backgroundColor;
        GUI.backgroundColor = sel
            ? (m == ToolMode.Paint ? new Color(0.4f, 0.8f, 1f) : new Color(1f, 0.5f, 0.4f))
            : new Color(0.3f, 0.3f, 0.3f);

        if (GUILayout.Button(label, GUILayout.Height(28)))
        {
            mode = m;
            DestroyGhost();
            SceneView.RepaintAll();
        }

        GUI.backgroundColor = bg;
    }

    void DrawSeparator()
    {
        Rect r = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(r, new Color(0.5f, 0.5f, 0.5f, 0.4f));
        EditorGUILayout.Space(4);
    }

    void InitStyles()
    {
        if (stylesInit) return;
        stylesInit = true;

        styleHeader = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize  = 14,
            alignment = TextAnchor.MiddleCenter
        };
    }
}
