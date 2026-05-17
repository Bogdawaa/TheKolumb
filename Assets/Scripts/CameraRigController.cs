using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRig : MonoBehaviour
{
    [Header("Настройки следования")]
    public Transform player;
    public bool isFollowing = false;
    
    [Header("Настройки движения")]
    public float moveSpeed = 10f;

    private Vector2 moveInput;
    private Keyboard keyboard;
    private Vector3 offset;

    void Awake()
    {
        keyboard = Keyboard.current;
    }

    void Start()
    {
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (keyboard != null && keyboard.fKey.wasPressedThisFrame)
        {
            isFollowing = !isFollowing;

            if (isFollowing)
            {
                // При переключении в режим слежения - переносим CameraRig к игроку с учётом смещения
                transform.position = player.position + offset;
            }
        }

        if (isFollowing)
        {
            // Режим СЛЕЖЕНИЯ
            transform.position = player.position + offset;
        }
        else
        {
            // TODO: Тут есть проблема в том что Движение в одну сторону + Движение в обратную сторону не возвращают в исходную точку
            // Свободное перемещение камеры
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (forward * moveInput.y) + (right * moveInput.x);
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        // При таком решении камера всегда двигается вперед/назад/лево/право в мировых координатах (независимо от поворота игрока) 
        // else
        // {
        //     // Движение строго по мировым осям X и Z
        //     Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        //     transform.position += moveDirection * moveSpeed * Time.deltaTime;
        // }
    }
}