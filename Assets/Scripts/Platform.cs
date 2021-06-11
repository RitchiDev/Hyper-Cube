using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private int m_ItemDropChance = 5;
    [SerializeField] private Item m_BoostPrefab;
    [SerializeField] private Item m_BombPrefab;
    private Item m_Item;
    private float m_Speed;
    private float m_ChangeTime;
    private float m_CurrentTime;
    private float m_Size;
    private Rigidbody2D m_Rigidbody;
    public Rigidbody2D Rigidbody => m_Rigidbody;

    private Vector2 m_Direction;
    private bool m_Move;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Move = true;
    }


    public void SetUp(bool doMove = true)
    {
        m_Move = doMove;

        if(!doMove)
        {
            if(m_Item)
            {
                m_Item.gameObject.SetActive(false);
            }
        }
        else
        {
            transform.position = new Vector3(RandomHorizontalPosition(), transform.position.y, transform.position.z);
        }
    }

    public void SetUp(float size, float speed, bool doMove = true)
    {
        m_Move = doMove;
        m_Speed = speed;
        m_Size = size;
        transform.DOScale(new Vector3(size, transform.localScale.y, transform.localScale.z), 0f);
        ActivateBoost();

        if (!doMove)
        {
            if(m_Item)
            {
                m_Item.gameObject.SetActive(false);
            }
        }
        else
        {
            transform.position = new Vector3(RandomHorizontalPosition(), transform.position.y, transform.position.z);
        }
    }

    private void ActivateBoost()
    {
        int rng = Random.Range(0, 100);
        //Debug.Log(rng);
        if(rng < m_ItemDropChance)
        {
            if(!m_Item)
            {
                if(rng < m_ItemDropChance / 2)
                {
                    m_Item = Instantiate(m_BoostPrefab, transform.position, m_BoostPrefab.transform.rotation);
                }
                else
                {
                    if(m_Size <= 3.5f)
                    {
                        return;
                    }

                    m_Item = Instantiate(m_BombPrefab, transform.position, m_BombPrefab.transform.rotation);
                }
            }
            m_Item.gameObject.SetActive(true);
        }
        else
        {
            if(m_Item)
            {
                m_Item.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        SetRandomDirection();
    }

    private void OnDisable()
    {
        if(m_Item)
        {
            m_Item.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(!m_Move)
        {
            return;
        }

        if(m_Item)
        {
            m_Item.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

        m_Rigidbody.velocity = new Vector2(m_Direction.x * m_Speed, 0);
    }

    private void ChangeTimer()
    {
        if(m_CurrentTime <= 0)
        {
            m_Direction = m_Direction == Vector2.right ? Vector2.left : Vector2.right;

            m_CurrentTime = m_ChangeTime;
        }

        if(m_CurrentTime > 0)
        {
            m_CurrentTime = Mathf.Clamp(m_CurrentTime - Time.deltaTime, 0, m_ChangeTime);
        }
    }

    private void SetRandomDirection()
    {
        int rng = Random.Range(-3, 2);
        if (rng >= 0)
        {
            m_Direction = Vector2.right;
        }
        else
        {
            m_Direction = Vector2.left;
        }
    }

    private float RandomHorizontalPosition()
    {
        return Random.Range(-1f, 1f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Wall>())
        {
            m_Direction = m_Direction == Vector2.right ? Vector2.left : Vector2.right;
        }
    }
}
