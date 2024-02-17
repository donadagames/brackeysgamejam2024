using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public int time = 10;
    public Volume volume;
    public ChromaticAberration aberration;
    public Vignette vignette;

    public static Player instance;
    private Vector3 initialPos = new Vector3();
    private Vector3 initialRotation = new Vector3();
    public EnemySpawner[] allSpawners;
    public Crate[] allCrates;
    public Interactable[] allDoors;
    public bool haveWin;
    public FinalDoor finalDoor;
    public void ResetAll()
    {
        finalDoor.ResetDoor();

        vignette.smoothness.value = 0;
        aberration.intensity.value = 0;

        foreach (EnemySpawner spawner in allSpawners)
        {
            spawner.ResetSpawner();
        }

        foreach (Crate crate in allCrates)
        {
            crate.ResetCrate();
        }

        foreach (Interactable door in allDoors)
        {
            door.ResetDoor();
        }

        potionQuantity = 0;
        potionQuantityText.text = "0";

        minutes = time - 1;
        seconds = 60;

        

        keyQuantity = 0;
        keyQuantityText.text = "0";
        haveWin = false;
        currentHealth = health;
        healthBar.fillAmount = 1;
        transform.localPosition = initialPos;
        transform.localEulerAngles = initialRotation;
        timeTextSeconds.gameObject.SetActive(value: true);
        timeCount = time * 60;
        timeTextSeconds.text = "60";
        timeTextMinutes.text = (time - 1).ToString();
        DealTime();
        isAlive = true;

        LeanTween.value(gameObject, 0, 1, time * 60).setOnUpdate(DealVignete).setOnComplete(PlayerDies);
        LeanTween.value(gameObject, 0, 1, time * 60).setOnUpdate(DealVignete).setOnComplete(PlayerDies);

    }


    #region STATUS REGION
    public float health;
    public Camera mainCamera;
    public float currentHealth;
    public bool isAlive = true;
    public int force;
    public EnemyAttackPosition[] enemyPosition;

    public void TakeDamage(float damage, bool isCritical)
    {
        if (currentHealth - damage > 0)
            currentHealth -= damage;
        else
            currentHealth = 0;

        isDamaged = isCritical;
        OnHealthChange?.Invoke(this,
            new OnHealthEventHandler { _currentHealth = currentHealth });
        CheckDeath();
    }

    public EnemyAttackPosition GetEnemyPosition()
    {
        foreach (EnemyAttackPosition pos in enemyPosition)
        {
            if (pos.isInUse == false)
            {
                pos.isInUse = true;
                return pos;
            }
        }

        return null;
    }

    public event EventHandler<OnHealthEventHandler> OnHealthChange;

    public class OnHealthEventHandler : EventArgs
    {
        public float _currentHealth;
    }

    public void OnHealth(object sender, OnHealthEventHandler handler)
    {
        healthBar.fillAmount = handler._currentHealth / 1000;
    }

    public void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            isAlive = false;
            OnDie?.Invoke(this, new OnDieEventHandler { _isAlive = isAlive });
        }
    }

    public event EventHandler<OnDieEventHandler> OnDie;

    public class OnDieEventHandler : EventArgs
    {
        public bool _isAlive;
    }


    #endregion

    #region STATES REGION
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public bool canAttack = true;
    public Sword sword;
    public bool isDamaged = false;
    [HideInInspector] public bool hasPressedMeleeAttackButton = false;
    [HideInInspector] public bool hasPressedJumpButton = false;
    [HideInInspector] public bool hasPressedInteractionButton = false;

    [HideInInspector] public bool IsGrounded() => characterController.isGrounded;

    #endregion

    #region ANIMATION REGION
    public string IDLE;
    public string DIE;
    public string JUMP;
    public string[] ATTACKS;
    public string GETHIT;
    public string MOVE;
    public string CHUTE;

    public float DIE_DURATION;
    public float JUMP_DURATION;
    public float[] ATTACKS_DURATION;
    public float GETHIT_DURATION;
    public float CHUTE_DURATION;

    public Animator animator;

    public void PlayAnimation(string animation)
    {
        animator.Play(animation);
    }

    public void IncreaseParameter()
    {
        animator.SetInteger("count", (animator.GetInteger("count") +
            UnityEngine.Random.Range(0, 2)));
    }

    public void SetParameterToZero()
    {
        animator.SetInteger("count", 0);
    }
    #endregion

    #region MOVEMENT REGION
    [SerializeField] float smoothTime = .05f;
    public Vector3 direction = new Vector3();
    public float speed;
    bool footIsOnWater;
    public Vector2 input = new Vector2();
    public CharacterController characterController;
    public bool canMove = true;
    private float currentVelocity;
    public float jumpHight;
    public bool canJump = true;
    private float velocity;
    private const float GRAVITY = -9.8f;
    [HideInInspector] public PlayerInputActions playerInputActions;
    public Transform footPos;
    public LayerMask waterLayer;
    public void GetDirection()
    {
        GetInput();
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;
        direction = input.y * forward + input.x * right;
    }

    public void ApplyMovement()
    {
        characterController.Move(direction * speed * Time.deltaTime);
    }

    public virtual void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x + 0.0001f, 0f, direction.z + 0.0001f));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void ApplyRotation()
    {
        if (input.sqrMagnitude == 0) return;

        var tangetAngle = Mathf.Atan2(direction.x,
            direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, tangetAngle, ref currentVelocity,
            smoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void ApplyAllMovement()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    public void ApplyGravity()
    {

        if (IsGrounded() && velocity < 0.0f)
            velocity = -10;

        else
            velocity += GRAVITY * Time.deltaTime;

        direction.y = velocity;
    }

    public void DetectWater()
    {
        footIsOnWater = CheckFootdWater();
    }

    public bool CheckFootdWater() => Physics.CheckSphere(footPos.position, .1f, waterLayer);

    #endregion

    #region INTERACTABLES REGION
    public bool isInteracting = false;
    public Transform headPos;
    public Interactable interactable;
    public LayerMask interactablesLayer;
    public LayerMask enemySpawnerLayer;
    public void SearchForEnemySpawner()
    {
        var spawners = Physics.OverlapSphere(transform.position, 50, enemySpawnerLayer);

        foreach (var spawner in spawners)
        {
            var enemySpawner = spawner.GetComponent<EnemySpawner>();
            enemySpawner.SpawEnemy();
        }
    }
    #endregion

    #region INVENTORY REGION
    public TextMeshProUGUI potionQuantityText;
    public int potionQuantity;


    public TextMeshProUGUI keyQuantityText;
    public int keyQuantity;
    public void UsePotion(InputAction.CallbackContext callback)
    {
        if (potionQuantity <= 0)
        {
            potionQuantity = 0;
            potionQuantityText.text = "0";
            ErrorSound();
        }

        else
        {
            PotionSound();
            potionQuantity--;
            potionQuantityText.text = potionQuantity.ToString();

            if (currentHealth + 200 >= health)
                currentHealth = health;
            else
                currentHealth += 200;

            OnHealthChange?.Invoke(this,
                new OnHealthEventHandler { _currentHealth = currentHealth });
        }
    }

    public void AddPotion()
    {
        potionQuantity++;
        potionQuantityText.text = potionQuantity.ToString();
    }

    public void AddKey()
    {
        KeySound();
        keyQuantity++;
        keyQuantityText.text = keyQuantity.ToString();
    }

    #endregion


    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        healthBar.fillAmount = 1;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.Player.Jump.performed += Jump;
        playerInputActions.Player.Attack.performed += Attack;
        playerInputActions.Player.Interact.performed += Interact;
        playerInputActions.Player.UseItem.performed += UsePotion;
        playerInputActions.Player.Pause.performed += PauseGame;
        playerInputActions.Player.Exit.performed += ExitGame;

        #region STATES DECLARATION
        var idle = new Idle(this);
        var move = new Move(this);
        var jump = new Jump(this);
        var interact = new Interact(this);
        var getHit = new GetHit(this);
        var attack = new Attack(this);
        var die = new Die(this);
        #endregion

        stateMachine = new StateMachine();
        void AddTransition(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition((IState)from, (IState)to, condition);


        #region IDLE TO OTHER STATES
        AddTransition(idle, move, PlayerHasMovementInput());
        AddTransition(idle, jump, () => ShouldJump());
        AddTransition(idle, attack, ShouldAttack());
        AddTransition(idle, interact, () => isAlive && isInteracting);
        #endregion

        #region JUMP IN PLACE TO OTHER STATES
        AddTransition(jump, idle, () => IsGrounded() && canJump == true);
        #endregion

        #region MOVING TO OTHER STATES
        AddTransition(move, idle, PlayerHasNoMovementInput());
        AddTransition(move, attack, ShouldAttack());
        AddTransition(move, jump, () => ShouldJump());
        AddTransition(move, interact, () => isAlive && isInteracting);
        #endregion

        #region ATTACK TO OTHER STATES
        AddTransition(attack, idle, () => canAttack == true);
        #endregion
        #region INTERACTING TO OTHER STATES
        AddTransition(interact, idle, () => isAlive && !isInteracting);
        #endregion

        stateMachine.AddAnyTransition(die, () => !isAlive);
        AddTransition(die, idle, () => isAlive);

        #region CONDITIONS
        Func<bool> PlayerHasMovementInput() => () => input.sqrMagnitude > .2f && isAlive && !ShouldJump() && !haveWin;
        Func<bool> PlayerHasNoMovementInput() => () => input.sqrMagnitude <= 0 && isAlive && !ShouldJump();
        Func<bool> ShouldAttack() => () => canAttack == true && hasPressedMeleeAttackButton && !isDamaged && isAlive && !ShouldJump() && !haveWin;
        #endregion

        stateMachine.SetState(idle);
    }

    public void SearchForInteractables()
    {
        if (interactable == null)
        {
            if (HasInteractable())
            {
                var _interactable = CheckForInteractable()[0].GetComponentInParent<Interactable>();

                interactable = _interactable;
                interactable.OnEnter();
            }
        }

        else
        {
            if (!HasInteractable())
            {
                interactable.OnExit();
                interactable = null;
            }
        }
    }

    public Collider[] CheckForInteractable() => Physics.OverlapSphere(headPos.position, .3f, interactablesLayer);

    public bool HasInteractable() => Physics.CheckSphere(headPos.position, .3f, interactablesLayer);

    public void Jump()
    {
        velocity = Mathf.Sqrt(jumpHight * -2 * GRAVITY);
    }

    public bool ShouldJump() => IsGrounded() && hasPressedJumpButton && isAlive && !haveWin;

    void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out aberration);

        minutes = time - 1;
        seconds = 60;

        LeanTween.value(gameObject, 0, 1, time * 60).setOnUpdate(DealVignete).setOnComplete(PlayerDies);
        LeanTween.value(gameObject, 0, 1, time * 60).setOnUpdate(DealVignete).setOnComplete(PlayerDies);

        timeTextSeconds.gameObject.SetActive(value: true);

        initialRotation = transform.localEulerAngles;
        initialPos = transform.localPosition;

        fadeCanvasGroup.alpha = 1;
        fadeCanvasGroup.LeanAlpha(0, 2f);
        deathPanel.SetActive(false);
        OnHealthChange += OnHealth;
        OnDie += OnDeathEvent;
        timeTextSeconds.text = "60";
        timeTextMinutes.text = (time - 1).ToString();
        timeCount = time * 60;
        DealTime();
    }

    private void DealVignete(float value)
    {
        vignette.smoothness.value = value;
        aberration.intensity.value = value;
    }

    public void PlayerDies()
    {
        currentHealth = 0;
        CheckDeath();
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    public void GetInput()
    {
        if (!canMove)
        {
            input = new Vector2(0, 0);
            return;
        }

        input = playerInputActions.Player.Move.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext callback)
    {

        if (!hasPressedJumpButton)
        {
            hasPressedJumpButton = true;
            canJump = false;
        }

        else
        {
            return;
        }

    }

    public GameObject pausePanel;

    private void PauseGame(InputAction.CallbackContext callback)
    {
        if (pausePanel.activeSelf == false)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }

        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void ExitGame(InputAction.CallbackContext callback)
    {
        Application.Quit();
    }

    private void Attack(InputAction.CallbackContext callback)
    {
        if (!hasPressedMeleeAttackButton)
        {
            hasPressedMeleeAttackButton = true;
        }

        else
        {
            return;
        }

    }

    private void Interact(InputAction.CallbackContext callback)
    {

        if (interactable != null && interactable.hasInteract == false)
        {
            interactable.Interact();
        }

        else
        {
            return;
        }

    }

    #region UI REGION

    public Image interactIcon;
    public Image interactKeyIcon;
    bool isClosing;
    public int leanIndex;
    public int leandDefaultIndex;
    public Image healthBar;
    public TextMeshProUGUI timeTextSeconds;
    public TextMeshProUGUI timeTextMinutes;
    public float startTime;
    public int timeCount = 900;
    public int finalCount;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI titleText;
    public GameObject deathPanel;
    public CanvasGroup fadeCanvasGroup;

    public void OnQuitButtonPressed()
    {
        ClickSound();
        Application.Quit();
    }

    public void OnTryAgainButtonPressed()
    {
        fadeCanvasGroup.LeanAlpha(1, 2f).setOnComplete(() => StartCoroutine(Resete()));
        deathPanel.SetActive(false);

        //SceneManager.LoadScene("Game", LoadSceneMode.Single);
        ClickSound();
    }

    private IEnumerator Resete()
    {
        yield return new WaitForSeconds(1);
        LeanTween.cancelAll();
        ResetAll();
        yield return new WaitForSeconds(1);
        fadeCanvasGroup.LeanAlpha(0, 2f);
    }


    [ContextMenu("Delete High Score")]
    public void DeleteAllPlayerPreffs()
    {
        PlayerPrefs.SetInt("time", 0);
    }

    private void OnDeathEvent(object sender, OnDieEventHandler handler)
    {
        fadeCanvasGroup.alpha = 0;

        finalCount = timeCount;

        titleText.text = "YOU ARE DEAD!";

        if (TrySetNewHighscore(finalCount) == true)
        {
            infoText.text = $"YOU HAVE REACHED A NEW SURVIVAL RECORD IN THE DOOR MAZE: {finalCount} SECONDS.";
        }

        else
        {
            infoText.text = $"YOU SURVIVED THE DOOR MAZE FOR {finalCount} SECONDS.\nYOUR PERSONAL RECORD IS {PlayerPrefs.GetInt("time")} SECONDS.";
        }


        fadeCanvasGroup.LeanAlpha(1, 2f).setOnComplete(() => deathPanel.SetActive(true));
        //deathPanel.SetActive(true);
    }

    public static bool TrySetNewHighscore(int score)
    {
        int currentHighscore = GetHighscore();
        if (score > currentHighscore)
        {
            // New Highscore
            PlayerPrefs.SetInt("time", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int GetHighscore()
    {
        return PlayerPrefs.GetInt("time");
    }

    public void DealTime()
    {
        LeanTween.value(this.gameObject, timeCount, timeCount + 1, 1).setOnComplete(DealTimeText);
    }

    public int seconds;
    public int minutes;

    private void DealTimeText()
    {
        timeCount--;
        seconds--;

        if (seconds == 0)
        {
            seconds = 60;
            minutes--;
        }

        timeTextSeconds.text = seconds.ToString();
        timeTextMinutes.text = minutes.ToString();

        DealTime();
    }

    public void GetTimeCount()
    {
        timeTextSeconds.gameObject.SetActive(false);
        //LeanTween.cancelAll();
    }

    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];

            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }

        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = UnityEngine.Input.mousePosition;

        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);

        return raysastResults;
    }


    public void SetInteractionSprite()
    {
        interactIcon.gameObject.SetActive(true);

        if (!isClosing)
        {
            leanIndex = interactIcon.gameObject.LeanScale(new Vector3(1, 1, 1), .25f).setOnComplete(() =>
           interactIcon.gameObject.LeanScale(new Vector3(.85f, .85f, .85f), .25f)).
           setLoopPingPong().id;
        }
    }

    public void SetDefaultInteractionSprite()
    {
        isClosing = true;
        LeanTween.cancel(leanIndex);

        leandDefaultIndex = interactIcon.gameObject.LeanScale(new Vector3(0, 0, 0), .1f).
            setOnComplete(OnDefaultComplete).id;
    }


    private void OnDefaultComplete()
    {
        interactIcon.gameObject.SetActive(false);
        isClosing = false;
        LeanTween.cancel(leandDefaultIndex);
        interactIcon.gameObject.transform.localScale = new Vector3(.85f, .85f, .85f);
    }

    #endregion

    #region SOUND REGION

    public AudioClip[] steps;
    public AudioClip[] attacks;
    public AudioClip[] waterSteps;
    public AudioClip jump;
    public AudioClip win;
    public AudioClip death;
    public AudioClip[] getHit;
    public AudioSource _audio;
    public AudioClip[] punchs;
    public AudioClip[] openDoor;
    public AudioClip[] swordHit;
    public AudioClip click;
    public AudioClip[] openCrate;
    public AudioClip error;
    public AudioClip potion;
    public AudioClip key;
    public void StepSound()
    {
        if (!footIsOnWater)
            _audio.PlayOneShot(steps[UnityEngine.Random.Range(0, steps.Length)]);
        else
            _audio.PlayOneShot(waterSteps[UnityEngine.Random.Range(0, waterSteps.Length)]);
    }
    public void AttackSound()
    {
        _audio.PlayOneShot(attacks[UnityEngine.Random.Range(0, attacks.Length)]);
    }

    public void PunchSound()
    {
        _audio.PlayOneShot(punchs[UnityEngine.Random.Range(0, punchs.Length)]);
    }

    public void OpenDoorSound()
    {
        _audio.PlayOneShot(openDoor[UnityEngine.Random.Range(0, openDoor.Length)]);
    }

    public void GetHitSound()
    {
        _audio.PlayOneShot(getHit[UnityEngine.Random.Range(0, getHit.Length)]);
    }

    public void SwordHitSound()
    {
        _audio.PlayOneShot(swordHit[UnityEngine.Random.Range(0, swordHit.Length)]);
    }

    public void DeathSound()
    {
        _audio.PlayOneShot(death);
    }

    public void JumpSound()
    {
        _audio.PlayOneShot(jump);
    }

    public void ClickSound()
    {
        _audio.PlayOneShot(clip: click);
    }

    public void OpenCrateSound()
    {
        _audio.PlayOneShot(openCrate[UnityEngine.Random.Range(0, openCrate.Length)]);
    }

    public void PotionSound()
    {
        _audio.PlayOneShot(potion);
    }

    public void KeySound()
    {
        _audio.PlayOneShot(key);
    }

    public void ErrorSound()
    {
        _audio.PlayOneShot(error);
    }

    public void WinSound()
    {
        _audio.PlayOneShot(win);

    }

    #endregion


    public void WinGame()
    {
        fadeCanvasGroup.alpha = 0;
        haveWin = true;
        finalCount = (time * 60) - timeCount;

        GetTimeCount();
        canMove = false;
        WinSound();

        titleText.text = "YOU ESCAPE THE MAZE!";

        if (TrySetNewHighscore(finalCount) == true)
        {
            infoText.text = $"YOU DEFETED THE MAZE BOSS AND PASSED THROUGH THE LAST DOOR IN {finalCount} SECONDS.\nIT'S YOUR NEW RECORD!";
        }

        else
        {
            infoText.text = $"YOU DEFETED THE MAZE BOSS AND PASSED THROUGH THE LAST DOOR IN {finalCount} SECONDS.\nYOUR PERSONAL RECORD IS {PlayerPrefs.GetInt("time")} SECONDS.";
        }


        fadeCanvasGroup.LeanAlpha(1, 2f).setOnComplete(() => deathPanel.SetActive(true));
    }
}
