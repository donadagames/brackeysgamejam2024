using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroPlayer : MonoBehaviour
{
    public AudioClip click;
    public AudioSource _audio;
    public GameObject creditsPanel;
    public GameObject menuPanel;
    public TextMeshProUGUI infoTexet;
    private void Start()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void IncreaseParameter()
    {
    }

    public void SetParameterToZero()
    {

    }

    public void OnQuitButton()
    {
        ClickSound();
        Application.Quit();
    }

    public void OnRulesButton()
    {

        infoTexet.text = $"Movement: <color=white>WSAD</color>\r\nRotation: <color=white>MOUSE rotation</color>\r\nJump: <color=white>SPACE</color>\r\nAttack : <color=white>MOUSE LEFT click</color>\r\nInteract : <color=white>E </color>\r\nPause: <color=white>P</color>\r\nUse Potions: <color=white>Q</color>\r\n\r\nHow to Scape the MAZE? <color=white> You are poisoned. You have 10 minutes to find your way out of the maze. As time goes by, you will feel the effects of the poison on your vision and perception of the game. Defeat skeletons. Find the key to the last room with the Troll Boss. Defeat the Red Demon Boss to exit the maze.";

        creditsPanel.SetActive(!creditsPanel.activeSelf);
        menuPanel.SetActive(!menuPanel.activeSelf);
        ClickSound();
    }

    public void OnCreeditsButton()
    {
        infoTexet.text = $"3d Art and VFX: <color=white>Synty (www.syntystore.com) </color>\r\n2d Art: <color=white>Layer Lab (www.layerlab.io) </color>\r\n2d Art: <color=white>OccaSoftware (www.occasoftware.com) </color>\r\nSFX : <color=white>Zapslat (zapsplat.com) </color>\r\nSFX : <color=white>SkyRaeVoicing (skyraevoicing.com) </color>\r\nMusic: <color=white>Muz Station Productions (soundcloud.com/muzstation-game-music)</color>\r\nTools: <color=white>Dented Pixel: LeanTween (dentedpixel.com)</color>";

        creditsPanel.SetActive(!creditsPanel.activeSelf);
        menuPanel.SetActive(!menuPanel.activeSelf);
        ClickSound();
    }

    public void OnPlayButton()
    {
        ClickSound();
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ClickSound()
    {
        _audio.PlayOneShot(clip: click);
    }
}
