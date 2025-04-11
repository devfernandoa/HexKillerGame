using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUp
{
    public string name;
    public string description;
    public string rarity;
    public Sprite icon;
    public System.Action<Player> applyEffect;
}

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    public List<PowerUp> allPowerUps = new List<PowerUp>();
    public List<PowerUp> availablePowerUps = new List<PowerUp>();

    private bool basicShootingObtained = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePowerUps();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePowerUps()
    {
        allPowerUps.Clear();

        // Basic shooting power-up (only available first time)
        allPowerUps.Add(new PowerUp
        {
            name = "Basic Shooting",
            description = "Unlocks projectile attack",
            rarity = "Special",
            icon = Resources.Load<Sprite>("Sprites/Shoot"),
            applyEffect = (player) =>
            {
                player.EnableBasicShooting();
                basicShootingObtained = true;
            }
        });

        // ===== BASIC POWER-UPS =====
        allPowerUps.Add(new PowerUp
        {
            name = "Quick Feet",
            description = "Adds 10% speed boost",
            rarity = "Common",
            icon = Resources.Load<Sprite>("Sprites/QuickFeet"),
            applyEffect = (player) =>
            {
                player.baseSpeed *= 1.1f;
            }
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Quick Draw",
            description = "Shoots 20% faster",
            icon = Resources.Load<Sprite>("Sprites/QuickDraw"),
            rarity = "Common",
            applyEffect = (player) =>
            {
                player.shootCooldown *= 0.8f;
            }
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Speed Boost",
            description = "Adds 50% speed boost for 5 seconds",
            icon = Resources.Load<Sprite>("Sprites/SpeedBoost"),
            rarity = "Common",
            applyEffect = (player) => player.ApplySpeedBoost(1.5f, 5f)
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Multi-Shot",
            description = "Shoots more bullets at once",
            icon = Resources.Load<Sprite>("Sprites/MultiShot"),
            rarity = "Common",
            applyEffect = (player) => player.EnableMultiShot()
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Split Shot",
            description = "Adds bullets in multiple directions",
            icon = Resources.Load<Sprite>("Sprites/SplitShot"),
            rarity = "Common",
            applyEffect = (player) => player.EnableSplitShot()
        });

        // ===== MOVEMENT/SPEED POWER-UPS =====
        allPowerUps.Add(new PowerUp
        {
            name = "Dash",
            description = "Gain a burst of speed when pressing space (X)",
            icon = Resources.Load<Sprite>("Sprites/Dash"),
            rarity = "Rare",
            applyEffect = (player) => player.EnableDashAbility()
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Momentum",
            description = "Build speed the longer you move in one direction",
            icon = Resources.Load<Sprite>("Sprites/Momentum"),
            rarity = "Rare",
            applyEffect = (player) => player.EnableMomentum()
        });

        // ===== PROJECTILE POWER-UPS =====

        allPowerUps.Add(new PowerUp
        {
            name = "Piercing Rounds",
            description = "Bullets pass through multiple enemies",
            icon = Resources.Load<Sprite>("Sprites/Piercing"),
            rarity = "Rare",
            applyEffect = (player) => player.EnablePiercingShots()
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Ricochet",
            description = "Bullets bounce off walls and enemies",
            icon = Resources.Load<Sprite>("Sprites/Ricochet"),
            rarity = "Rare",
            applyEffect = (player) => player.EnableRicochetShots()
        });

        allPowerUps.Add(new PowerUp
        {
            name = "Homing Missiles",
            description = "Bullets track enemies",
            icon = Resources.Load<Sprite>("Sprites/Homing"),
            rarity = "Rare",
            applyEffect = (player) => player.EnableHomingShots()
        });

        // Initially only Basic Shooting is available
        availablePowerUps.Add(allPowerUps[0]); // Index of Basic Shooting
    }

    public void ResetManager()
    {
        basicShootingObtained = false;
        availablePowerUps.Clear();
        availablePowerUps.Add(allPowerUps[0]); // Add Basic Shooting back
    }

    public List<PowerUp> GetRandomPowerUps(int count)
    {
        List<PowerUp> eligible;

        int rareCount = 1; // Number of rare power-ups to include
        int commonCount = 2; // Remaining slots for common power-ups

        int rareAvailable = allPowerUps.FindAll(p => p.rarity == "Rare").Count;
        int commonAvailable = allPowerUps.FindAll(p => p.rarity == "Common").Count;

        if (rareAvailable < rareCount)
        {
            // If not enough rare power-ups, fill with common
            commonCount += rareCount - rareAvailable;
            rareCount = rareAvailable;
        }
        else if (commonAvailable < commonCount)
        {
            // If not enough common power-ups, fill with rare
            rareCount -= commonCount - commonAvailable;
            commonCount = commonAvailable;
        }

        if (!basicShootingObtained)
        {
            // Only Basic Shooting is available for first selection
            eligible = availablePowerUps.FindAll(p => p.name == "Basic Shooting");
        }
        else
        {
            // After first selection, all power-ups except Basic Shooting are eligible
            eligible = allPowerUps.FindAll(p => p.name != "Basic Shooting");
        }

        // If not enough power-ups, just return what we have
        if (eligible.Count < count) return eligible;

        // Get random selection
        var result = new List<PowerUp>();
        for (int i = 0; i < rareCount; i++)
        {
            var rareOptions = eligible.FindAll(p => p.rarity == "Rare");
            var randomRare = rareOptions[Random.Range(0, rareOptions.Count)];
            result.Add(randomRare);
            eligible.Remove(randomRare);
        }
        for (int i = 0; i < commonCount; i++)
        {
            var commonOptions = eligible.FindAll(p => p.rarity == "Common");
            var randomCommon = commonOptions[Random.Range(0, commonOptions.Count)];
            result.Add(randomCommon);
            eligible.Remove(randomCommon);
        }

        return result;
    }
}