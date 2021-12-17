using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    public static Energy Instance;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);            
        }
    }

    private const int MAXENERGY = 6;
    private const int ENERGYPOINT = 1;
    [SerializeField]
    private float RegenRate;
    [SerializeField]
    private float RegenTime;
    private float Regen;
    [SerializeField]
    Slider[] AttackerEnergyBar = new Slider[MAXENERGY];
    [SerializeField]
    private int atkCurrEnergy;
    [SerializeField]
    private int atkEnergyCost;
    [SerializeField]
    Slider[] DefenderEnergyBar = new Slider[MAXENERGY];
    [SerializeField]
    private int defCurrEnergy;
    [SerializeField]
    private int defEnergyCost;

    Color player1Color;
    Color player2Color;

    private void Start() {
        player1Color = Color.blue;
        player2Color = Color.red;
    }

    #region Init
    public void InitEnergy() {
        Regen = 0;
        atkCurrEnergy = 0;
        defCurrEnergy = 0;
        foreach (Slider energy in AttackerEnergyBar) 
        {
            energy.value = 0; 
            if (MatchController.Instance.matchCount % 2 == 0)
            {
                energy.fillRect.GetComponent<Image>().color = player1Color;
                energy.fillRect.GetComponent<Image>().color = StartingEnergyPoints(energy);
            }
            else 
            {
                energy.fillRect.GetComponent<Image>().color = player2Color;
                energy.fillRect.GetComponent<Image>().color = StartingEnergyPoints(energy);
            } 
        }
        foreach (Slider energy in DefenderEnergyBar) 
        { 
            energy.value = 0;
            if (MatchController.Instance.matchCount % 2 == 0)
            {
                energy.fillRect.GetComponent<Image>().color = player2Color;
                energy.fillRect.GetComponent<Image>().color = StartingEnergyPoints(energy);
            }
            else 
            {
                energy.fillRect.GetComponent<Image>().color = player1Color;
                energy.fillRect.GetComponent<Image>().color = StartingEnergyPoints(energy);
            } 
        }
    }
    #endregion

    #region Colors Adjustment
    private Color StartingEnergyPoints(Slider _energy)
    {
        return new Color(_energy.fillRect.GetComponent<Image>().color.r, 
                            _energy.fillRect.GetComponent<Image>().color.g,
                            _energy.fillRect.GetComponent<Image>().color.b,0.5f);
    }

    private Color MaxEnergyPoints(Slider _energy)
    {
        return new Color(_energy.fillRect.GetComponent<Image>().color.r, 
                            _energy.fillRect.GetComponent<Image>().color.g,
                            _energy.fillRect.GetComponent<Image>().color.b,1f);
    }
    #endregion

    #region Energy Regeneration
    private void Update() {
        if (MatchController.Instance.IsPlaying)
        {
            if (atkCurrEnergy < MAXENERGY || defCurrEnergy < MAXENERGY)
            {
                Regen += Time.deltaTime;
                if (Regen >= RegenTime)
                {
                    EnergyRegen();
                    Regen = 0;
                }
            }
        }
    }

    private void EnergyRegen() {

        if (atkCurrEnergy < MAXENERGY)
        {
            AttackerEnergyBar[atkCurrEnergy].value += RegenRate;
            if (AttackerEnergyBar[atkCurrEnergy].value < 1)
            {
                AttackerEnergyBar[atkCurrEnergy].fillRect.GetComponent<Image>().color = StartingEnergyPoints(AttackerEnergyBar[atkCurrEnergy]);
            }
            else
            {
                AttackerEnergyBar[atkCurrEnergy].fillRect.GetComponent<Image>().color = MaxEnergyPoints(AttackerEnergyBar[atkCurrEnergy]);
            }

            if (CheckEnergyPoints(AttackerEnergyBar[atkCurrEnergy]))
            {
                atkCurrEnergy++;
            }
        } 

        if (defCurrEnergy < MAXENERGY)
        {
            DefenderEnergyBar[defCurrEnergy].value += RegenRate;
            if (DefenderEnergyBar[defCurrEnergy].value < 1)
            {
                DefenderEnergyBar[defCurrEnergy].fillRect.GetComponent<Image>().color = StartingEnergyPoints(DefenderEnergyBar[defCurrEnergy]);
            }
            else
            {
                DefenderEnergyBar[defCurrEnergy].fillRect.GetComponent<Image>().color = MaxEnergyPoints(DefenderEnergyBar[defCurrEnergy]);
            }

            if (CheckEnergyPoints(DefenderEnergyBar[defCurrEnergy]))
            {
                defCurrEnergy++;
            }
        } 
    }

    private bool CheckEnergyPoints(Slider _energyPoint)
    {
        if (_energyPoint.value == ENERGYPOINT)
        {
            _energyPoint.fillRect.GetComponent<Image>().color = MaxEnergyPoints(_energyPoint);
            return true;
        }

        return false;
    }
    #endregion

    #region Energy Consumption
    public void UseEnergyPoints(Type type)
    {
        switch (type)
        {
            case Type.Attacker : 
            for (int i = atkCurrEnergy-1; i >= atkCurrEnergy-atkEnergyCost; i--)
            {
                AttackerEnergyBar[i].value = 0;
            }
            atkCurrEnergy -= atkEnergyCost;
            break;
            case Type.Defender :
            for (int i = defCurrEnergy-1; i >= defCurrEnergy-defEnergyCost; i--)
            {
                DefenderEnergyBar[i].value = 0;
            }
            defCurrEnergy -= defEnergyCost;
            break;
        }
    }

    public bool IsAtkEnergyAvailable() {
        return atkCurrEnergy >= atkEnergyCost;
    }

    public bool IsDefEnergyAvailable() {
        return defCurrEnergy >= defEnergyCost;
    }
    #endregion



}
