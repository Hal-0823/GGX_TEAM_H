using UnityEngine;
using UnityEditor;

/// <summary>
/// 選択したモデルに対して、指定したレイヤー設定、MeshCollider（Convex = ON）、Rigidbodyを自動で追加するエディターツール。
/// </summary>
public class BrokenModelSetupTool : EditorWindow
{
    // UIで選択する変数を保持
    private int targetLayer = 0;

    // メニューバーに項目を追加
    [MenuItem("Tools/Model Setup/Auto Attach Components")]
    public static void ShowWindow()
    {
        // ウィンドウを表示
        GetWindow<BrokenModelSetupTool>("Model Setup + Convex");
    }

    private void OnGUI()
    {
        GUILayout.Label("設定", EditorStyles.boldLabel);

        // レイヤー選択フィールド
        targetLayer = EditorGUILayout.LayerField("Target Layer", targetLayer);

        EditorGUILayout.Space();
        GUILayout.Label("追加設定", EditorStyles.boldLabel);
        GUILayout.Label("- MeshColliderは Convex = ON で追加されます。", EditorStyles.helpBox);

        EditorGUILayout.Space();

        // 実行ボタン
        if (GUILayout.Button("Apply to Selected Objects"))
        {
            ApplyToSelection();
        }
    }

    private void ApplyToSelection()
    {
        // 選択中のGameObjectをすべて取得
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("オブジェクトが選択されていません。");
            return;
        }

        // Undoグループを開始（複数の操作をまとめて1回のUndoで戻せるようにする）
        Undo.SetCurrentGroupName("Model Setup With Convex");
        int undoGroupIndex = Undo.GetCurrentGroup();

        foreach (GameObject rootObj in selectedObjects)
        {
            // 自分自身とすべての子オブジェクトを取得 (非アクティブなものも含む)
            Transform[] allChildren = rootObj.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in allChildren)
            {
                GameObject obj = child.gameObject;

                // 現在のオブジェクトがルート（親）ならスキップ
                if (obj == rootObj)
                {
                    continue;
                }

                // --- 1. レイヤーの設定 ---
                if (obj.layer != targetLayer)
                {
                    Undo.RecordObject(obj, "Change Layer");
                    obj.layer = targetLayer;
                }

                // --- 2. MeshColliderのアタッチと設定 ---
                // まだアタッチされていない場合のみ処理する
                if (obj.GetComponent<MeshCollider>() == null)
                {
                    // Undo対応で追加し、追加されたコンポーネントの参照を受け取る
                    MeshCollider mc = Undo.AddComponent<MeshCollider>(obj);
                    
                    // ConvexをONにする
                    // (AddComponent直後なのでUndo.RecordObjectは不要だが、念のため明示的に行うスタイルも可)
                    mc.convex = true;
                }

                // --- 3. Rigidbodyのアタッチ ---
                if (obj.GetComponent<Rigidbody>() == null)
                {
                    Undo.AddComponent<Rigidbody>(obj);
                }
            }
        }

        // Undoグループを閉じる
        Undo.CollapseUndoOperations(undoGroupIndex);

        Debug.Log($"{selectedObjects.Length}個のルートオブジェクト（およびその子）に対して処理が完了しました。ConvexもONになっています。");
    }
}