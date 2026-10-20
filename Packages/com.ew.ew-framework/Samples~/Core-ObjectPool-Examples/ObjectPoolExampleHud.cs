using UnityEngine;
using EW_Framework.Core.ObjectPool.Manager;

namespace EW_Framework.Core.ObjectPool.Examples
{
    /// <summary>
    /// Game 视图 HUD：对比"调用 Spawn 的次数"与"实际 Instantiate 的实例数"，直观展示对象池的复用。
    /// 实例数取自 SyncPoolManager 下的 "[Pool] 预制体名" 节点：池创建的实例都挂在它下面，激活的在用，未激活的在池中闲置。
    /// </summary>
    public sealed class ObjectPoolExampleHud : MonoBehaviour
    {
        [SerializeField] private SyncPoolSpawnerExample _spawner;

        // Layout is authored for a 720p-tall Game view and scaled to the actual height.
        private const float ReferenceHeight = 720f;

        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _highlightStyle;
        private GUIStyle _legendStyle;
        private Texture2D _panelTexture;

        private void OnGUI()
        {
            if (_spawner == null || _spawner.Prefab == null) return;

            // IMGUI APIs (including GUI.skin) must be called from within OnGUI.
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 26, fontStyle = FontStyle.Bold };
                _bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 18 };
                _highlightStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
                _highlightStyle.normal.textColor = new Color(0.45f, 0.85f, 1f);
                _legendStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
                _panelTexture = new Texture2D(1, 1);
                _panelTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
                _panelTexture.Apply();
            }

            CountPoolInstances(out int created, out int active);
            int spawnCalls = _spawner.TotalSpawnCalls;
            int reused = Mathf.Max(0, spawnCalls - created);

            float scale = Screen.height / ReferenceHeight;
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));

            const float pad = 16f;
            const float width = 560f;
            float x = pad;
            float y = pad;
            GUI.DrawTexture(new Rect(x - 8f, y - 6f, width, 186f), _panelTexture);

            GUI.Label(new Rect(x, y, width, 34f), "ObjectPool Example (SyncPoolManager)", _titleStyle);
            y += 38f;

            GUI.Label(new Rect(x, y, width, 28f), $"Spawn calls: {spawnCalls}    Instantiated: {created}", _highlightStyle);
            y += 30f;

            GUI.Label(new Rect(x, y, width, 24f), $"Reused from pool: {reused}", _bodyStyle);
            y += 24f;

            GUI.Label(new Rect(x, y, width, 24f), $"Active: {active}    Idle in pool: {created - active}", _bodyStyle);
            y += 24f;

            // Colours match PooledObjectExample's defaults.
            _legendStyle.normal.textColor = new Color(1f, 0.55f, 0.15f);
            GUI.Label(new Rect(x, y, 260f, 24f), "● newly instantiated", _legendStyle);
            _legendStyle.normal.textColor = new Color(0.3f, 0.85f, 1f);
            GUI.Label(new Rect(x + 210f, y, 260f, 24f), "● reused from pool", _legendStyle);
            y += 28f;

            GUI.Label(new Rect(x, y, width, 24f),
                $"Space: spawn {_spawner.SpawnCountPerBatch}    Backspace: despawn last    C: clear pools", _bodyStyle);

            GUI.matrix = previous;
        }

        private void CountPoolInstances(out int created, out int active)
        {
            created = 0;
            active = 0;

            // Don't create the manager just to draw the HUD.
            var manager = FindFirstObjectByType<SyncPoolManager>();
            if (manager == null) return;

            Transform poolParent = manager.transform.Find($"[Pool] {_spawner.Prefab.name}");
            if (poolParent == null) return;

            created = poolParent.childCount;
            foreach (Transform child in poolParent)
            {
                if (child.gameObject.activeSelf) active++;
            }
        }

        private void OnDestroy()
        {
            if (_panelTexture != null) Destroy(_panelTexture);
        }
    }
}
