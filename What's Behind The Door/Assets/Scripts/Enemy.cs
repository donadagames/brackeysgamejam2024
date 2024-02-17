using UnityEngine;

public class Enemy : MonoBehaviour, IDamageble
{
    public bool isTarget = false;
    public EnemySpawner spawner;
    public float distance;
    public float scapeDistance = 11;
    public Animator animator;
    public float distanceToAttack;
    public float chasingVelocity;
    public string IDLE;
    public string RUN;
    public string GETHIT;
    public string[] DIE;
    public string[] ATTACKS;
    public float[] attackClipTime;
    public float getHitClipTime;
    public float[] dieClipTime;
    public bool isAlive = true;
    public float health;
    public float currentHealth;
    public int minDamage;
    public int maxDamage;
    public bool isDamaged = false;
    [HideInInspector] public Rigidbody rb;
    public bool canGetHit = true;
    public Collider _collider;
    public bool isFinalBoss = false;
    public bool isOrcBoss = false;
    public FinalDoor finalDoor;

    public virtual void Update()
    {
        spawner.stateMachine.Tick();
    }

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        currentHealth = health;

        if (isFinalBoss)
        { 
            finalDoor = FindAnyObjectByType<FinalDoor>();
        }

    }

    public virtual void GetPlayerDistance(Transform target)
    {
        distance = Vector3.Distance(transform.position, target.position);

        if (distance > scapeDistance)
        {
            spawner.shouldSpawn = true;
            Destroy(this.gameObject);


            if (spawner.enemyPosition != null)
                spawner.enemyPosition.isInUse = false;
        }
    }

    public virtual void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x + 0.0001f, 0f, direction.z + 0.0001f));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public virtual void MoveToTarget(Transform target)
    {
        transform.position =
            Vector3.MoveTowards
            (transform.position, target.position, chasingVelocity * Time.deltaTime);
    }

    public float Attack()
    {
        var index = UnityEngine.Random.Range(0, ATTACKS.Length);
        animator.Play(ATTACKS[index]);

        return attackClipTime[index];
    }

    public virtual void TakeDamage(float damage, bool isCritical)
    {
        if (isAlive)
        {
            currentHealth -= damage;
            CheckIfIsDead();
            isDamaged = isCritical;
        }
    }

    public virtual void CheckIfIsDead()
    {
        if (currentHealth <= 0)
        {
            isAlive = false;
        }
    }

    public void Damage()
    {
        var damage = UnityEngine.Random.Range(minDamage, maxDamage);
        var isCritical = damage >= maxDamage * .8f;
        spawner.player.TakeDamage(damage, isCritical);
        PunchAudio();
    }

    public void DamageWithForce(float force = 10)
    {
        var damage = UnityEngine.Random.Range(minDamage, maxDamage);
        spawner.player.TakeDamage(damage, true);
        PunchAudio();
    }

    #region AUDIO CONTROLLER
    [Header("Audio Controller")]
    public AudioSource audioSource;
    public AudioClip[] walkAudioClips;
    public AudioClip[] attackAudioClips;
    public AudioClip[] deathAudioClips;
    public AudioClip[] getHitAudioClip;
    public AudioClip[] punchAudioClips;

    public virtual AudioClip GetRandomAudioClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void DeathAudio()
    {
        audioSource.PlayOneShot(GetRandomAudioClip(deathAudioClips));
    }

    public void WalkAudio()
    {
        audioSource.PlayOneShot(GetRandomAudioClip(walkAudioClips));
    }

    public void GetHitAudio()
    {
        audioSource.PlayOneShot(GetRandomAudioClip(getHitAudioClip));
    }

    public void AttackAudio()
    {
        audioSource.PlayOneShot(GetRandomAudioClip(attackAudioClips));
    }

    public void PunchAudio()
    {
        audioSource.PlayOneShot(GetRandomAudioClip(punchAudioClips));
    }
    #endregion
}

public interface IDamageble
{
    public void TakeDamage(float damage, bool isCritical);
}