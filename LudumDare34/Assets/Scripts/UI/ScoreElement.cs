using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreElement : MonoBehaviour
{

    public string element_name;
    public int element_score;

    public Text text_name;
    public Text text_score;

    void Start()
    {
        text_name.text = element_name;
        text_score.text = element_score.ToString();
    }

    void Update()
    {

    }
}
