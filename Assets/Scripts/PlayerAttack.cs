using Fusion;
using UnityEngine;

public class PlayerAttack : NetworkBehaviour
{
    [Header("Combat Settings")]
    public NetworkObject fireballPrefab;
    public Transform firePoint;
    private Animator _animator; // Thêm dòng này

    private void Awake()
    {
        _animator = GetComponent<Animator>(); // Thêm dòng này
    }

    private void Update()
    {
        if (!HasStateAuthority) return;

        // Chặn đánh khi đang mở khung chat hoặc click vào UI
        if (ChatUI.Instance != null && ChatUI.Instance.chatPanel != null && ChatUI.Instance.chatPanel.activeSelf)
            return;
            
        if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("Attack"); // Gọi animation đánh
            ShootFireball();
        }
    }

    private void ShootFireball()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 targetPoint;

        int playerLayerMask = LayerMask.GetMask("Player");
        int mask = ~playerLayerMask;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100f);

        Vector3 direction = targetPoint - firePoint.position;
        direction.Normalize();
        Quaternion rotation = Quaternion.LookRotation(direction);

        Runner.Spawn(fireballPrefab, firePoint.position, rotation, Object.StateAuthority);
    }
}