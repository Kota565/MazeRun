using UnityEngine;

/// <summary>
/// プレイヤーを追いかけるカメラの制御用スクリプトです。
/// オフセットと角度をInspectorで調整できるようにしています。
/// </summary>
public class FollowCamera : MonoBehaviour
{
   
    [SerializeField] private Transform targetTransform;                              // 追いかける対象
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -10);         // カメラの位置のずれ
    [SerializeField] private Vector3 cameraRotationEuler = new Vector3(45f, 0f, 0f); // カメラの角度

    void LateUpdate()
    {
        // カメラはLateUpdateで動かすと、プレイヤーの移動後に追従できる
        transform.position = targetTransform.position + cameraOffset;

        // Inspectorで設定した角度をそのまま使う
        transform.rotation = Quaternion.Euler(cameraRotationEuler);
    }

    // プレイヤー生成時に呼び出して、追従対象をセットする
    public void SetTarget(Transform player)
   {
        targetTransform = player;
   }
}
