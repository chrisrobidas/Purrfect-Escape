using System.Text;
using UnityEngine;
using TMPro;

public class Screen : MonoBehaviour
{
    [SerializeField] private Puzzle _puzzle;
    [SerializeField] private int _index;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private bool HideUnderscores;
    void Start()
    {
        if (HideUnderscores)
        {
            _text.text = _puzzle.GetCode()[_index].ToString();
            return;
        }
        
        string codePiece = "_ _ _ _";
        StringBuilder sb = new StringBuilder(codePiece);
        sb[_index] = _puzzle.GetCode()[_index];
        _text.text = sb.ToString();
    }
}
