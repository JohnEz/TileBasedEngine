using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public enum UnitSize {
	Small,
	Normal,
	Large,
	Giant
}

[System.Serializable]
public class Unit : MonoBehaviour {

	public int ID;
	public int tileX;
	public int tileY;
	public TileMap map;
    public UnitManager uManager;

	public List<Node> currentPath;
	public List<Node> reachableTiles;
	public List<Node> reachableTilesWithDash;

    //Stats

    //Base - stats that the max should return to with no modifications
    public int baseHP = 100;
    public int baseMana = 100;
    public int baseMove = 4;
    public float baseArmour = 0.16f;
    public int baseInit = 4;
    public int baseAP = 1;
	public UnitSize mySize = UnitSize.Normal;
	public int baseDodge = 5;
	public int baseBlock = 0;

    //Max - current maximum stats after effects
	public int maxHP = 100;
	public int maxMana = 100;
	public int movespeed = 4;
	public float armourDamageReduction = 0.16f;
	public int init = 5;
	public int maxAP = 1;
	public int teir = 1;
	public int dodgeChance = 5;
	public int blockChance = 0;

    //current - the current value of the stats
    public int shield = 0;
	public int hp;
	public int mana;
	public int remainingMove;
	public int actionPoints;

    //modifiers
    public float damageDealtMod = 1;
    public float damageRecievedMod = 1;
    public float healingRecievedMod = 1;

	//logic bools
	public bool isDead = false;
	public bool playable = true; // is it an npc or playable character
	public bool moving 		= false; // is it currently moving
	public bool attacking 	= false; // is it currently attacking

	//combat
	public Ability[] myAbilities = new Ability[6];
    public List<Effect> myEffects = new List<Effect>();
	public List<Effect> expiredEffects = new List<Effect> ();
	public List<Trigger> myTriggers = new List<Trigger> ();
	public int comboPoints = 0;
	public int team = 1;

	Image healthBar;
	Text hpText;
	Image manaBar;
	Text manaText;
	Image shieldBar;
	public GameObject damageCombatText;
	public GameObject healingCombatText;
	public GameObject statusCombatText;
	public GameObject manaCombatText;

	void Start() {
		hp = maxHP;
		mana = maxMana;
		remainingMove = movespeed;
		actionPoints = maxAP;
		healthBar = transform.FindChild("UnitCanvas").FindChild ("HealthBar").GetComponent<Image> ();
		hpText = transform.FindChild("UnitCanvas").FindChild ("HPText").GetComponent<Text> ();

		manaBar = transform.FindChild("UnitCanvas").FindChild ("ManaBar").GetComponent<Image> ();
		manaText = transform.FindChild("UnitCanvas").FindChild ("ManaText").GetComponent<Text> ();

		shieldBar = transform.FindChild("UnitCanvas").FindChild ("ShieldBar").GetComponent<Image> ();

		//update status bars
		UpdateHealthBar ();
		UpdateManaBar();
	}

	//when a unit starts a new turn, this function is ran
    public void StartTurn()
    {
        //set each stat to its base then apply effects
        maxHP = baseHP;
        maxMana = baseMana;
        movespeed = baseMove;
        armourDamageReduction = baseArmour;
        init = baseInit;
        maxAP = baseAP;
        damageDealtMod = 1;
        damageRecievedMod = 1;
        healingRecievedMod = 1;
		shield = 0;
		dodgeChance = baseDodge;
		blockChance = baseBlock;

		//apply the effects
        foreach (Effect eff in myEffects)
        {
            eff.RunEffect(this);
			if (eff.duration <= 0) {
				expiredEffects.Add(eff);
			}
        }

		//remove expired effects
		foreach (Effect eff in expiredEffects) {
			myEffects.Remove(eff);
		}
		
		expiredEffects = new List<Effect> ();

        // if maxhp is now lower than current hp
        if (hp > maxHP)
        {
            hp = maxHP;
        }

        // if maxmana is now lower than current mana
        if (mana > maxMana)
        {
            mana = maxMana;
        }

        // reset move and action
		remainingMove = movespeed;
		actionPoints = maxAP;

		for(int i=0; i < myAbilities.Length; ++i) {
			if (myAbilities[i] != null) {
				myAbilities[i].ReduceCooldown(1);
			}
		}

		List<Trigger> finishedTriggers = new List<Trigger> ();
		
		//check all triggers to see if this is their event
		foreach (Trigger trig in myTriggers) {
			--trig.duration;
			if (trig.duration < 0) {
				finishedTriggers.Add (trig);
			}
		}
		
		//remove finished triggers
		foreach (Trigger trig in finishedTriggers) {
			myTriggers.Remove(trig);
		}

		//update status bars
		UpdateHealthBar ();
		UpdateManaBar();
	}

	void Update() {
		//call the correct update method depending on unit
		if (playable) {
			PlayerUpdate();
		} else {
			AIUpdate();
		}
	}

	//update for playable characters
	public void PlayerUpdate() {
		if (currentPath != null) {
			if (currentPath.Count > 0) {
				int currNode = 0;
				Vector3 start = map.TileCoordToWorldCoord(tileX, tileY) +
					new Vector3(0,0,-1.5f);
				Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
					new Vector3(0,0,-1.5f);
					
				Debug.DrawLine(start, end, Color.blue);
					
				while ( currNode < currentPath.Count - 1) {
					start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
						new Vector3(0,0,-1.5f);
					end = map.TileCoordToWorldCoord(currentPath[currNode+1].x, currentPath[currNode+1].y) +
						new Vector3(0,0,-1.5f);
					
					Debug.DrawLine(start, end, Color.blue);
					
					++currNode;
				}
			}
		}
		
		if (moving) {
			map.UnhighlightTiles();
			// Have we moved our visible piece close enough to the target tile that we can
			// advance to the next step in our pathfinding?
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {
				SlideMovement ();
				if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {

					//check to see if unit is stood on a trigger
					if (map.GetNode(tileX, tileY).myTrigger != null) {
						Trigger trig = map.GetNode(tileX, tileY).myTrigger;
						// check to see the target type
						if (trig.myTargets == TargetType.All || (trig.myTargets == TargetType.Ally && trig.myCaster.team == team) || (trig.myTargets == TargetType.Enemy && trig.myCaster.team != team)) {
							//you have activated my trap card!
							trig.CheckTrigger(TriggerType.Floor, this);
							map.GetNode(tileX, tileY).myTrigger = null;
						}
					}
					
					//stopped moving

					// if the unit can still move
					if (remainingMove > 0 || actionPoints > 0) {
						DrawReachableTiles();
					}
					moving = false;
					transform.position = map.TileCoordToWorldCoord (tileX, tileY);
				}
			}
			
			// Smoothly animate towards the correct map tile.
			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);
		}

		//TODO Attacking shit
		if (attacking) {
			attacking = false;

			for (int i=0; i < 6; ++i) {
				if (myAbilities[i] != null) {
					if (!myAbilities[i].AbilityFinished) {
						attacking = true;
						myAbilities[i].UpdateAbility();
					}
				}
			}
		}
	}

	// the AI update function, used mainly for animation
	public void AIUpdate() {

		AIBehaviours myAI = GetComponent<AIBehaviours> ();

		if (moving) {
			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.01f) {
				SlideMovement ();
				if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.01f) {

					//check to see if unit is stood on a trigger
					if (map.GetNode(tileX, tileY).myTrigger != null) {
						Trigger trig = map.GetNode(tileX, tileY).myTrigger;
						// check to see the target type
						if (trig.myTargets == TargetType.All || (trig.myTargets == TargetType.Ally && trig.myCaster.team == team) || (trig.myTargets == TargetType.Enemy && trig.myCaster.team != team)) {
							//you have activated my trap card!
							trig.CheckTrigger(TriggerType.Floor, this);
							map.GetNode(tileX, tileY).myTrigger = null;
						}
					}

					//stopped moving
					moving = false;
					transform.position = map.TileCoordToWorldCoord (tileX, tileY);
				}

			}

			// Smoothly animate towards the correct map tile.
			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);

		} else if (attacking) {
			myAI.Attack();
			attacking = false;
			
			for (int i=0; i < 6; ++i) {
				if (myAbilities[i] != null) {
					if (!myAbilities[i].AbilityFinished) {
						attacking = true;
						myAbilities[i].UpdateAbility();
					}
				}
			}
		}
		else {
			remainingMove = 0;
			actionPoints = 0;
		}
	}

	public bool UnitBusy() {
		return attacking || moving;
	}

	// sets the variables for where the unit should slide to next in its path
	public void SlideMovement() {
		// if there is no path return
		if(currentPath==null)
			return;

		// get the new first node and move
		tileX = currentPath[0].x;
		tileY = currentPath[0].y;

		// remove the old tile
		currentPath.RemoveAt (0);

		//at the target tile
		if (currentPath.Count == 0) {
			//clear path finding info
			currentPath = null;
		}


	}

	//old, used to slide tile to tile in current path
	public void MoveNextTile() {
		map.UnhighlightTiles();

		while (remainingMove > 0 || actionPoints > 0) {

			if (currentPath == null || currentPath.Count == 1) {
				break;
			}

			if (remainingMove < 1 && currentPath.Count > 1) {
				remainingMove = movespeed;
				--actionPoints;
				
			}

			// remove the old tile
			currentPath.RemoveAt (0);

			remainingMove -= (int)map.CostToEnterTile(currentPath [0].x, currentPath [0].y);

			// get the new first node and move
			tileX = currentPath[0].x;
			tileY = currentPath[0].y;
			transform.position = map.TileCoordToWorldCoord(tileX, tileY);

			//at the target tile
			if (currentPath.Count == 1) {
				//clear path finding info
				currentPath = null;
			}
		}

		if (remainingMove > 0 || actionPoints > 0) {
			DrawReachableTiles();
		}
	}

	// animates the unit to slide to a specified tile
	public void SlideToTile(int x, int y) {
		map.GetNode(tileX, tileY).myUnit = null;
		map.GetNode(x, y).myUnit = this;

		//currentPath = new List<Node> ();
		//currentPath.Add (map.GetNode (x, y));

		tileX = x;
		tileY = y;
		moving = true;

	}
	
	// removes the amount of moves it took to follow path
	public void RemoveMovement() {
		//remove movement cost
		if (currentPath.Last ().cost > remainingMove) {
			remainingMove += movespeed - (int)currentPath.Last ().cost;
			--actionPoints;
		} else {
			remainingMove -= (int)currentPath.Last ().cost;
		}
	}

	// shows all the tiles the unit can walk on
	public void DrawReachableTiles() {
		map.FindReachableTilesUnit ();
		map.HighlightTiles (reachableTiles, new Color(0.3f,0.3f,0.6f), new Color(0.4f,0.4f,0.85f), 1);
		map.HighlightTiles (reachableTilesWithDash, new Color(0.6f,0.6f,0.3f), new Color(0.85f,0.85f,0.4f), 1);

		map.GetClickableTile (tileX, tileY).HighlightTile (new Color (0.75f, 0.75f, 0.75f), new Color (0.85f, 0.85f, 0.85f), 0);
	}

	// updates the visual of the unit's healthbar
	public void UpdateHealthBar() {
		if (healthBar) {
			//calculate fill ammounts
			float fullFill = (float)(hp + shield) / (float)(maxHP + shield);
			float hpFill = (float)hp / (float)(hp + shield);
			float shieldFill = (float)shield / (float)(hp + shield);

			//fill the hp Bar
			healthBar.fillAmount = hpFill * fullFill;

			//find the position of the shield
			float hpWidth = healthBar.rectTransform.rect.width;

			if (shieldFill > 0) {
				//how far the bar is past its mid point
				float pastMid = ((hpFill * fullFill) - 0.5f) * hpWidth;

				//calculate the position for the shield bar
				Vector3 sPos = new Vector3(pastMid + (hpWidth * 0.5f), shieldBar.transform.localPosition.y, shieldBar.transform.localPosition.z);
				shieldBar.transform.localPosition = sPos;


				//fill the shield bar
				shieldBar.fillAmount = shieldFill * fullFill;
			}
			else {
				shieldBar.fillAmount = 0;
			}

			//set the text
			int hpShield = hp + shield;
			hpText.text = hpShield.ToString();
		
			//if there isnt hp, show no hp
			if (hp <= 0) {
				hpText.text = "";
			}
		}
	}

	// updates the visual of the unit's manabar
	public void UpdateManaBar() {
		if (manaBar) {
			if (maxMana != 0) {
				manaBar.fillAmount = (float)mana / (float)maxMana;

				manaText.text = mana.ToString();
				
				if (mana <= 0) {
					manaText.text = "";
				}

			} else {
				manaBar.fillAmount = 0;
				manaText.text = "";
			}
		}
	}

	// unit takes parameter damage and shows in combat text
	public void TakeDamage(int dmg, AudioClip sound = null, bool canBeEvaded = true)
    {
		//calculate the actual damage after reductions
		int damage = (int)((dmg * damageRecievedMod) * (1 - armourDamageReduction));

		if (canBeEvaded) {

			//see if the unit dodges the attack
			int dodge = Random.Range (1, 100);
			if (dodge <= dodgeChance) {
				ShowCombatText ("Dodged", statusCombatText);
				GetComponent<AudioSource> ().PlayOneShot (uManager.GetComponent<PrefabLibrary> ().getSoundEffect ("Dodge"));
				return;
			}

			//see if the unit Blocks the attack
			int block = Random.Range (1, 100);
			if (block <= blockChance) {
				ShowCombatText ("Blocked", statusCombatText);
				damage = (int)(damage * 0.25f);
				GetComponent<AudioSource> ().PlayOneShot (uManager.GetComponent<PrefabLibrary> ().getSoundEffect ("Block"));
			} else if (sound != null) {
				GetComponent<AudioSource> ().PlayOneShot (sound);
			}
		} else if (sound != null) {
			GetComponent<AudioSource> ().PlayOneShot (sound);
		}


		int currentDamage = damage;

		if (shield != 0) {
			currentDamage = DamageShield(currentDamage);
		}

        hp -= currentDamage;
		CheckTriggers (TriggerType.Hit);
        ShowCombatText(damage.ToString(), damageCombatText);

		if (hp < 0) {
			hp = 0;
		}

		UpdateHealthBar ();
    }

	int DamageShield(int dmg) {
		int currentDamage = dmg;

		shield -= currentDamage;
		
		if (shield < 0) {
			shield = 0;
		}
		
		foreach (Effect eff in myEffects) {
			if (eff.description.Equals("Damage Shield")) {
				
				ShieldEffect sEff = eff as ShieldEffect;
				currentDamage = sEff.TakeDamage(currentDamage);
				
				//if it has no shield amount left, remove it
				if (sEff.ShieldIsDestroyed()) {
					expiredEffects.Add(eff);
				}
				
				// if the current damage is less than 1, stop looking through shields
				if (currentDamage <=0) {
					//dont want a negative so it doesnt heal the target
					currentDamage = 0;
					break;
				}
			}
		}

		return currentDamage;
	}

	public void AddShield(int s) {
		shield += s;
		ShowCombatText(s.ToString(), statusCombatText);
		UpdateHealthBar ();
	}

	// heals the target
	public void TakeHealing(int heal)
	{
		int healing = (int)(heal * healingRecievedMod);
		hp += healing;

		if (hp > maxHP) {
			hp = maxHP;
		}

		CheckTriggers (TriggerType.Healed);
		ShowCombatText(healing.ToString(), healingCombatText);
		UpdateHealthBar ();
	}

	//gives mana up until the current max
	public void AddRemoveMana(int m) {
		if (m != 0)
		{
			mana += m;
			if (mana > maxMana) {
				mana = maxMana;
			}
			else if (mana < 0) {
				mana = 0;
			}

			if (m > 0) {
				ShowCombatText(m.ToString(), manaCombatText);
			}
		}

		UpdateManaBar();
	}

	// created floating combat text with the specified value
	public void ShowCombatText(string txt, GameObject go) {
		GameObject temp = Instantiate (go) as GameObject;
		RectTransform tempRect = temp.GetComponent<RectTransform> ();
		temp.GetComponent<Animator> ().SetTrigger ("Hit");
		temp.transform.SetParent (transform.FindChild("UnitCanvas"));

		tempRect.transform.localPosition = damageCombatText.transform.localPosition;
		tempRect.transform.localScale = damageCombatText.transform.localScale;
		tempRect.transform.rotation = damageCombatText.transform.localRotation;
		
		temp.GetComponent<Text> ().text = txt;
		Destroy(temp.gameObject, 2); 
	}

	// applies the specified effect, or stacks and refreshes it if it is already on
    public void ApplyEffect(Effect eff)
    {
		//need to reaply all the buffs
		
		//set each stat to its base then apply effects
		maxHP = baseHP;
		maxMana = baseMana;
		movespeed = baseMove;
		armourDamageReduction = baseArmour;
		init = baseInit;
		maxAP = baseAP;
		damageDealtMod = 1;
		damageRecievedMod = 1;
		healingRecievedMod = 1;
		bool alreadyHad = false;
		shield = 0;
		dodgeChance = baseDodge;
		blockChance = baseBlock;

		foreach (Effect currentEffect in myEffects) {
			if (currentEffect.name.Contains(eff.name)) {
				currentEffect.AddStack();
				alreadyHad = true;
			}
			currentEffect.RunEffect(this, true);
		}

		if (!alreadyHad) {
			myEffects.Add(eff);
			eff.RunEffect(this, true);
		}

    }

	public void AddTrigger(Trigger trig) {
		myTriggers.Add (trig);
	}

	// finds if the unit is stunned
	public bool IsStunned() {
		foreach (Effect eff in myEffects) {
			if (eff.description.Equals("Stun")) {
				return true;
			}
		}
		return false;
	}

	// finds if the unit is snared
	public bool IsSnared() {
		foreach (Effect eff in myEffects) {
			if (eff.description.Equals("Snare")) {
				return true;
			}
		}
		return false;
	}

	// adds the parameter of combo points until up to 5
	public void AddComboPoints(int i) {
		comboPoints += i;
		if (comboPoints > 5) {
			comboPoints = 5;
		}
	}

	//uses all the combo points the unit as got and returns the value
	public int UseComboPoints() {
		int cp = comboPoints;
		comboPoints = 0;
		return cp;
	}

	public void CheckTriggers(TriggerType tt) {
		List<Trigger> finishedTriggers = new List<Trigger> ();

		//check all triggers to see if this is their event
		foreach (Trigger trig in myTriggers) {
			trig.CheckTrigger(tt, this);
			// if the trigger has finished add it to the remove list
			if (trig.triggerCount >= trig.maxTriggers) {
				finishedTriggers.Add(trig);
			}
		}

		//remove finished triggers
		foreach (Trigger trig in finishedTriggers) {
			myTriggers.Remove(trig);
		}

	}
}
