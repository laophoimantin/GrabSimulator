using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [System.Serializable]
    public struct VFXData
    {
        public string Name;          // Tên để gọi (VD: "Explosion", "Dust")
        public GameObject Prefab;    // Prefab chứa ParticleSystem
        public float LifeTime;       // Thời gian tồn tại (nếu particle không tự tắt)
    }

    [Header("Config")]
    [SerializeField] private List<VFXData> _vfxList; // Kéo thả vào đây trong Inspector

    // Dictionary để tra cứu nhanh, chứ không phải đi mò từng thằng như mò kim đáy bể
    private Dictionary<string, VFXData> _vfxDictionary;

    void Awake()
    {
        // Singleton Pattern - Đảm bảo chỉ có 1 thằng quản lý thôi
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Chuyển List sang Dictionary ngay lúc đầu game
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _vfxDictionary = new Dictionary<string, VFXData>();
        foreach (var vfx in _vfxList)
        {
            if (_vfxDictionary.ContainsKey(vfx.Name))
            {
                Debug.LogWarning($"Ông bị mất trí nhớ à? Effect tên '{vfx.Name}' bị trùng rồi kìa!");
                continue;
            }
            _vfxDictionary.Add(vfx.Name, vfx);
        }
    }

    // Hàm này để gọi từ bất cứ đâu. VD: EffectManager.Instance.PlayVFX("Boom", transform.position);
    public void PlayVFX(string name, Vector3 position, Quaternion rotation = default)
    {
        if (_vfxDictionary.TryGetValue(name, out VFXData data))
        {
            // Nếu không truyền rotation thì mặc định là thẳng đứng (identity)
            if (rotation.Equals(default(Quaternion))) rotation = Quaternion.identity;

            // Spawn ra
            GameObject instance = Instantiate(data.Prefab, position, rotation);

            // Nếu ông muốn nó dính vào cái gì đó (như dính vào xe đang chạy), thì xử lý parent ở ngoài
            // Còn ở đây là spawn xong quên luôn (Fire and Forget)

            // Tự hủy sau thời gian LifeTime. 
            // Nếu LifeTime = 0, thử lấy duration của ParticleSystem
            float destroyTime = data.LifeTime;
            if (destroyTime <= 0)
            {
                var ps = instance.GetComponent<ParticleSystem>();
                if (ps != null) destroyTime = ps.main.duration + ps.main.startLifetime.constantMax;
                else destroyTime = 2f; // Fallback nếu ông lười set thông số
            }

            Destroy(instance, destroyTime);
        }
        else
        {
            Debug.LogError($"Tìm không thấy effect tên '{name}'. Gõ đúng chính tả đi cha nội!");
        }
    }
    
    // Overload: Gọi nhanh không cần Rotation
    public void PlayVFX(string name, Vector3 position)
    {
        PlayVFX(name, position, Quaternion.identity);
    }
}