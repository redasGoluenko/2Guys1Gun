using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueTypewriter : MonoBehaviour
{
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    public float typingSpeed = 0.05f;
    public float eraseSpeed = 0.04f;
    public float pauseBetweenLines = 1.2f;
    public float initialDelay = 3.0f;
    public float dramaticPause = 0.6f;
    public bool dialogueActive = true;

    private string[] dialogueLines = new string[]
    {
        "Where did you come from?",
        "I'm not sure, but I think I'm... you...",
        "This is very strange...",
        "This must be what happens when you enter a black hole.",
        "We need to figure out how to get out of this place.",
        "I do not think we're alone here...",
        "Unfortunately, only one of us has the gun, so let's try to share."
    };

    private void Start()
    {
        player1Text.text = "";
        player2Text.text = "";
        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue()
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            bool isPlayer1 = i % 2 == 0;
            TextMeshProUGUI currentSpeaker = isPlayer1 ? player1Text : player2Text;

            if (dialogueLines[i].Contains("I think I'm... you..."))
            {
                // Special pause before "you..."
                string part1 = "I'm not sure, but I think I'm...";
                string part2 = " you...";
                yield return TypeLine(currentSpeaker, part1);
                yield return new WaitForSeconds(dramaticPause);
                yield return TypeLine(currentSpeaker, part1 + part2);
            }
            else
            {
                yield return TypeLine(currentSpeaker, dialogueLines[i]);
            }

            yield return new WaitForSeconds(pauseBetweenLines);
            yield return EraseLine(currentSpeaker);

        }
        dialogueActive = false;
    }

    IEnumerator TypeLine(TextMeshProUGUI textElement, string fullLine)
    {
        string currentText = textElement.text;
        for (int i = currentText.Length; i <= fullLine.Length; i++)
        {
            textElement.text = fullLine.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator EraseLine(TextMeshProUGUI textElement)
    {
        string currentText = textElement.text;
        for (int i = currentText.Length; i >= 0; i--)
        {
            textElement.text = currentText.Substring(0, i);
            yield return new WaitForSeconds(eraseSpeed);
        }
    }
}
