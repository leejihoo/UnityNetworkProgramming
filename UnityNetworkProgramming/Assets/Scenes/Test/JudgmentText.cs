using UnityEngine;
using TMPro;
using DG.Tweening;

public class JudgmentText : MonoBehaviour
{
    public TMP_Text text;
    private float fadeTime = 1f; // 페이드 아웃 시간

    public void Show(string message, Color color)
    {
        text.text = message;
        text.color = color;
        text.alpha = 1; // 처음에는 완전히 보이게 설정

        // DOTween 애니메이션 실행
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.2f, 0.2f));  // 처음에 살짝 커졌다가
        seq.Append(transform.DOScale(1f, 0.2f));   // 원래 크기로 돌아온 후
        seq.Join(transform.DOLocalMoveY(50f, 0.5f));
        seq.Join(text.DOFade(0, fadeTime));
        seq.OnComplete(() => Destroy(gameObject));
        //seq.Play();
    }
}