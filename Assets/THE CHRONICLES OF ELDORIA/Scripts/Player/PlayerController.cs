using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private PlayerMovement playerMovement = null;
    private ComboAttack comboAttack;

    [SerializeField] PlayerAnimation playerAnimation;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = new PlayerMovement(this);
        comboAttack = new ComboAttack(this);
    }

    void Update()
    {
        this.PlayerMovementUpdate();
        if (Input.GetMouseButtonDown(0))
        {
            comboAttack.OnAttackInput();
        }
        comboAttack.ComboUpdate();

    }

    #region Getter
    public CharacterController GetCharacterController()
    {
        return characterController;
    }

    public PlayerAnimation GetPlayerAnimation()
    {
        return playerAnimation;
    }
    #endregion


    #region Player update
    private void PlayerMovementUpdate()
    {
        if (playerMovement != null)
        {
            playerMovement.HandleMovement();
        }
    }

    #endregion
}
