using UnityEngine;

public class WindowInteract : MonoBehaviour
{
    [Header("Ссылки")]
    public DialogueManager dialogueManager;
    public GameObject hintObject;

    [Header("Настройки")]
    [TextArea(3, 5)]
    public string windowText = "За окном слишком темно... Я не вижу даже сада.(Нажми Е, чтобы выйти)";
    public Color darkColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _canInteract = false;
    private bool _isDialogueOpen = false; // Следим, открыто ли окно сейчас

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null) _originalColor = _sr.color;
    }

    void Update()
    {
        if (_canInteract && Input.GetKeyDown(KeyCode.E))
        {
            if (!_isDialogueOpen)
            {
                // Если закрыто — открываем
                dialogueManager.ShowMonologue(windowText, false);
                _isDialogueOpen = true;
                if (hintObject != null) hintObject.SetActive(false); // Прячем подсказку "Нажми Е"
            }
            else
            {
                // Если уже открыто — закрываем по нажатию Е
                CloseCurrentDialogue();
            }
        }
    }

    private void CloseCurrentDialogue()
    {
        dialogueManager.CloseDialogue();
        _isDialogueOpen = false;

        // Возвращаем подсказку "Нажми Е", так как игрок всё еще в зоне окна
        if (_canInteract && hintObject != null) hintObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = true;
            if (!_isDialogueOpen && hintObject != null) hintObject.SetActive(true);
            if (_sr != null) _sr.color = darkColor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = false;
            CloseCurrentDialogue(); // Закрываем, если игрок просто убежал
            if (_sr != null) _sr.color = _originalColor;
            if (hintObject != null) hintObject.SetActive(false);
        }
    }
}