
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    public void RestartGame()
    {
        // Zeit zurücksetzen, falls das Spiel pausiert wurde
        Time.timeScale = 1f;

        // Aktuelle Scene neu laden
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        Debug.Log("Spiel beendet");

    #if UNITY_EDITOR
            // Play-Modus im Unity-Editor beenden
            EditorApplication.isPlaying = false;
    #else
        // Fertigen Build beenden
        Application.Quit();
    #endif
    }
}
