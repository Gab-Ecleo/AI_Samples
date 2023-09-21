using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum OffmeshLinkMoveMethod
{

    Teleport,
    NormalSpeed,
    Parabola,
    Curve
    
}

public class Offlink_Jump : MonoBehaviour
{

    #region variables

    NavMeshAgent agent;

    public OffmeshLinkMoveMethod _method;
    public AnimationCurve _aCurve;

    [SerializeField] private float _jumpTime;
    [SerializeField] private float _angle;
    [SerializeField] private float _jumpHeight;

    #endregion

    #region Unity Method

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    IEnumerator Start()
    {
        while (true)
        {
            agent.autoTraverseOffMeshLink = false;
            if (agent.isOnOffMeshLink)
            {
                switch (_method)
                {
                    case OffmeshLinkMoveMethod.Teleport:

                        break;

                    case OffmeshLinkMoveMethod.NormalSpeed:
                        StartCoroutine(NormalSpeed(agent));
                        break;

                    case OffmeshLinkMoveMethod.Parabola:
                        StartCoroutine(Parabola(agent, _jumpHeight, _jumpTime));
                        break;

                    case OffmeshLinkMoveMethod.Curve:
                        StartCoroutine(Curve(agent, _jumpTime));
                        break;

                    default:
                        break;
                }
                agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    #endregion

    #region Methods

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        while(agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;

        while(normalizedTime < 1.0f)
        {
            float yOffset = height * 4f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;

            yield return null;
        }
    }

    IEnumerator Curve(NavMeshAgent agent, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            float yOffset = _aCurve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;

            yield return null;
        }
    }

    #endregion
}
