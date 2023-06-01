using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Image))]
public class RadialProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Radial Progress Bar")]
    public static void AddRadialProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Radial Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif

    [SerializeField] private float minimum = 0f, maximum = 100f;
    [SerializeField] private float rotateSpeed = 450f;
    [SerializeField] private float initialFill = 0f;

    private Image image;
    private int multiplier = 1;

    private void Start()
    {
        image = GetComponent<Image>();
        image.type = Image.Type.Filled;

        GetCurrentFill();
    }

    private void Update()
    {
        AnimateRadial();
    }

    private void GetCurrentFill()
    {
        float currentOffset = initialFill - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        image.fillAmount = fillAmount;
    }

    private void AnimateRadial()
    {
        if (image.fillMethod == Image.FillMethod.Radial360)
        {
            if (image.fillAmount >= 0.85f)
            {
                multiplier = -1;
            }
            else if (image.fillAmount <= 0.075f)
            {
                multiplier = 1;
            }
            image.fillAmount += Time.deltaTime * multiplier;
            image.transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.forward);
        }
    }
}
