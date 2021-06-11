using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask m_PlatformLayer;
    [SerializeField] private float m_JumpForce = 12;
    [SerializeField] private float m_PoundForce = 12;

    [SerializeField] private float m_MinRandomForce = 12f;
    [SerializeField] private float m_MaxRandomForce = 16f;

    [SerializeField] private float m_Torque = 10f;

    [SerializeField] private float m_FallingThreshold = 0;

    [SerializeField] private Vector2 m_SpawnRange = new Vector2(4f, -6f);

    [SerializeField] private ParticleSystem m_DeathEffect;
    [SerializeField] private ParticleSystem m_LineEffect;
    [SerializeField] private ParticleSystem m_PoundEffect;
    public ParticleSystem DeathEffect => m_DeathEffect;

    private Rigidbody2D m_Rigidbody;
    public Rigidbody2D Rigidbody => m_Rigidbody;

    private Platform m_PreviousPlatform;
    private Platform m_CurrentPlatform;

    private bool m_Falling;
    private bool m_PressedPound;

    public bool Falling => m_Falling;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_PreviousPlatform = null;
        Platform firstPlatform = SpawnManager.Instance.InstantiatePlatform();
        firstPlatform.SetUp(false);
        m_CurrentPlatform = firstPlatform;
        CameraController.Instance.SetNewTarget(firstPlatform.transform, true);
        GameManager.Instance.SpawnPlatform();

        //Pound();
    }

    private void Update()
    {
        TouchScreenInput();
        KeyBoardInput();

        m_Falling = m_Rigidbody.velocity.y < m_FallingThreshold;

        //m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, ClampedVelocityY());
    }

    private void TouchScreenInput()
    {
        if (Input.touchCount > 0)
        {
            if(IsPointerOverUIbject())
            {
                return;
            }

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if(m_CurrentPlatform)
                {
                    Jump();

                    return;
                }
                else
                {
                    Pound();
                }
            }
        }
    }

    private void KeyBoardInput()
    {
        if(Input.touchCount > 0)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(m_CurrentPlatform)
            {
                Jump();

                return;
            }
            else
            {
                Pound();
            }
        }
    }

    private void Jump()
    {
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x, 0);
        m_Rigidbody.AddForce(JumpForce(), ForceMode2D.Impulse);
        m_Rigidbody.AddTorque(RandomTorque(), ForceMode2D.Impulse);

        Instantiate(m_LineEffect, m_Rigidbody.position, m_LineEffect.transform.rotation);
    }

    private void Pound()
    {
        m_PressedPound = true;

        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.velocity = new Vector2(0, PoundForce().y);
        //m_Rigidbody.AddForce(PoundForce(), ForceMode2D.Impulse);

        Instantiate(m_PoundEffect, m_Rigidbody.position, m_LineEffect.transform.rotation);
    }

    private float ClampedVelocityY()
    {
        return Mathf.Clamp(m_Rigidbody.velocity.y, -m_PoundForce, m_JumpForce);
    }

    private void BreakObject()
    {
        Destroy(gameObject);
    }

    private bool IsPointerOverUIbject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
    private Vector3 JumpForce()
    {
        return Vector3.up * m_JumpForce;
    }

    private Vector3 PoundForce()
    {
        return Vector3.down * m_PoundForce;
    }

    private float RandomTorque()
    {
        return Random.Range(-m_Torque, m_Torque);
    }

    private float RandomRoundedTorque()
    {
        return Mathf.RoundToInt(Random.Range(-m_Torque, m_Torque));
    }

    private Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(m_MinRandomForce, m_MaxRandomForce);
    }

    private Vector3 RandomSpawnPosition()
    {
        return new Vector3(Random.Range(-m_SpawnRange.x, m_SpawnRange.x), m_SpawnRange.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_PressedPound)
        {
            return;
        }

        if (m_Falling)
        {
            BoostItem boost = collision.gameObject.GetComponent<BoostItem>();
            if (boost)
            {
                m_CurrentPlatform = null;
                return;
            }

            if (!m_CurrentPlatform)
            {
                m_CurrentPlatform = collision.gameObject.GetComponent<Platform>();
                if (m_CurrentPlatform)
                {
                    if(m_PressedPound)
                    {
                        Vector2 effectSpawnPosition = new Vector2(m_Rigidbody.position.x, m_Rigidbody.position.y - 0.4f);
                        Instantiate(m_LineEffect, effectSpawnPosition, m_LineEffect.transform.rotation);
                        m_PressedPound = false;
                    }

                    Debug.Log("On Platform");
                    m_Rigidbody.velocity = Vector2.zero;
                    m_Rigidbody.position = new Vector2(m_Rigidbody.position.x, m_CurrentPlatform.Rigidbody.position.y + m_Rigidbody.transform.localScale.y);
                    CameraController.Instance.SetNewTarget(m_CurrentPlatform.transform);

                    if (m_PreviousPlatform != m_CurrentPlatform)
                    {
                        m_PreviousPlatform = m_CurrentPlatform;
                        ScoreManager.Instance.AddScore(1);
                        GameManager.Instance.SpawnPlatform();
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform)
        {
            m_CurrentPlatform = null;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        BoostItem boost = collision.gameObject.GetComponent<BoostItem>();
        if (boost)
        {
            if(Falling)
            {
                m_CurrentPlatform = null;
            }
            return;
        }

        if (m_PressedPound)
        {
            if(!m_CurrentPlatform)
            {
                m_CurrentPlatform = collision.gameObject.GetComponent<Platform>();
                if (m_CurrentPlatform)
                {
                    if (m_PressedPound)
                    {
                        Vector2 effectSpawnPosition = new Vector2(m_Rigidbody.position.x, m_Rigidbody.position.y - 0.4f);
                        Instantiate(m_LineEffect, effectSpawnPosition, m_LineEffect.transform.rotation);
                        m_PressedPound = false;
                    }

                    //Debug.Log("On Platform");
                    m_Rigidbody.velocity = Vector2.zero;
                    m_Rigidbody.position = new Vector2(m_Rigidbody.position.x, m_CurrentPlatform.Rigidbody.position.y + m_Rigidbody.transform.localScale.y);
                    CameraController.Instance.SetNewTarget(m_CurrentPlatform.transform);

                    if (m_PreviousPlatform != m_CurrentPlatform)
                    {
                        m_PreviousPlatform = m_CurrentPlatform;
                        ScoreManager.Instance.AddScore(1);
                        GameManager.Instance.SpawnPlatform();
                    }
                }
            }
        }

        if (m_CurrentPlatform)
        {
            m_Rigidbody.velocity = new Vector2(m_CurrentPlatform.Rigidbody.velocity.x, ClampedVelocityY());
        }
    }
}
