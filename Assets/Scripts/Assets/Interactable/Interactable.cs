using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Name_Item")]
    public string itemName = "Item";

    public Vector3 handLocalPosition = Vector3.zero;
    public Vector3 handLocalRotation = Vector3.zero;

    private Rigidbody rb;
    private Collider col;

    [HideInInspector] public bool isHeld = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // ��ͧ��� Collider ����� Trigger ������� Player �Թ����������ǵ�Ǩ�Ѻ��
        col.isTrigger = true;
    }

    // ���¡�͹�١��������
    public void OnPickup(Transform handParent)
    {
        isHeld = true;

        if (rb != null)
        {
            rb.isKinematic = true; // �Դ���ԡ�� �����鵡���Ͷ١���͹��������
        }

        col.enabled = false; // �Դ collider �͹�������� �ѹ仪��Ѻ����ͧ���ͧ͢���

        transform.SetParent(handParent);
        transform.localPosition = handLocalPosition;
        transform.localRotation = Quaternion.Euler(handLocalRotation);
    }

    // ���¡�͹�ҧ/��駢ͧ (�����͹Ҥ���ҡ���к���ͻ�ͧ)
    public void OnDrop(Transform dropPoint)
    {
        isHeld = false;

        transform.SetParent(null);
        transform.position = dropPoint.position;

        col.enabled = true;

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
